using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NoofLab.VKBot.Core.Configuration;
using NoofLab.VKBot.Core.Exceptions;
using NoofLab.VKBot.Core.Extensions;
using NoofLab.VKBot.Core.Messages;
using VkNet;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace NoofLab.VKBot.Core
{
    public class VkService : BackgroundService
    {
        private readonly Config _config;
        private readonly VkApi _api;
        private readonly ILogger<VkService> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IMessageParser _messageParser;

        private const int TimeOutMilliseconds = 100; // todo: research
        private const int LongPollApiVersion = 2;

        public VkService(Config config, VkApi api, ILogger<VkService> logger, IHostApplicationLifetime applicationLifetime, IMessageParser messageParser)
        {
            _config = config;
            _api = api;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _messageParser = messageParser;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Authorize();
            await BackgroundWork(cancellationToken);
        }

        private async Task BackgroundWork(CancellationToken cancellationToken)
        {
            var settings = await GetLongPollingServer();
            ulong? newPts = null;

            while (cancellationToken.IsCancellationRequested is false)
            {
                try
                {
                    //var res = await _api.Messages.GetLongPollHistoryAsync(
                    //    new MessagesGetLongPollHistoryParams
                    //    {
                    //        Ts = ulong.Parse(settings.Ts),
                    //        Pts = newPts ?? settings.Pts,
                    //        LpVersion = LongPollApiVersion,
                    //    }
                    //);

                    //newPts = res.NewPts;

                    //await ProcessUpdates(res.Messages);

                    var res2 = await _api.Groups.GetBotsLongPollHistoryAsync(
                        new BotsLongPollHistoryParams
                        {
                            Key = settings.Key,
                            Server = $"https://{settings.Server}",
                            Ts = (newPts ?? settings.Pts).ToString(),
                            Wait = 1,
                        });

                    newPts = ulong.Parse(res2.Ts);
                    if (res2.Updates.Any())
                    {

                    }
                }
                catch (LongPollOutdateException e)
                {
                    if (ulong.TryParse(e.Ts, out var newts))
                        newPts = newts;
                    _logger.LogInformation(e, "Timestamp outdated");
                }
                catch (Exception e)
                {
                    _logger.LogNotHandledException(e);
                }

                await Task.Delay(TimeOutMilliseconds, cancellationToken);
            }
        }

        private async Task ProcessUpdates(IReadOnlyCollection<Message> messages)
        {
            foreach (var message in messages)
            {
                if (message.Out is true || !_messageParser.TryParse(message.Text, out var instruction))
                    continue;

                if(instruction.Reply)
                {
                    try
                    {
                        await _api.Messages.SendAsync(new MessagesSendParams
                        {
                            Message = "I can reply!",
                            PeerId = message.PeerId,
                            RandomId = DateTime.UtcNow.Ticks,
                        });
                    }
                    catch (Exception e)
                    {
                        _logger.LogNotHandledException(e);
                    }
                }
            }
        }

        private async Task Authorize()
        {
            try
            {
                await _api.AuthorizeAsync(new ApiAuthParams { AccessToken = _config.AccessToken });
                _logger.LogTrace("Token: {Token}", _api.Token);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error during authorization");
            }
        }

        private async Task<LongPollServerResponse> GetLongPollingServer()
        {
            try
            {
                return await _api.Messages.GetLongPollServerAsync(needPts: true, lpVersion: LongPollApiVersion);
            }
            catch (VkApiException e)
            {
                var message = e.Message switch
                {
                    VkExceptionMessages.GroupMessagesDisabled =>
                        "The bot is not properly configured. Turn on group messages",
                    _ => "Unexpected exception during configuring long polling",
                };
                _logger.LogCritical(e, message);
                StopApplication();
                throw;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Unexpected exception during configuring long polling");
                StopApplication();
                throw;
            }
        }

        private void StopApplication() => _applicationLifetime.StopApplication();
    }
}
