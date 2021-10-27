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
using NoofLab.VKBot.Core.Extensions.Logger;
using NoofLab.VKBot.Core.Extensions.Validation;
using NoofLab.VKBot.Core.Messages;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
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
            if (TryValidate() is false)
                return;
            await Authorize();
            await BackgroundWork(cancellationToken);
        }

        private bool TryValidate()
        {
            try
            {
                _config.Validate();
                return true;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Config is not valid");
                StopApplication();
                return false;
            }
        }

        private async Task BackgroundWork(CancellationToken cancellationToken)
        {
            var settings = await GetLongPollingServer();
            string newTs = settings.Ts;

            while (cancellationToken.IsCancellationRequested is false)
            {
                try
                {
                    var res = await _api.Groups.GetBotsLongPollHistoryAsync(
                        new BotsLongPollHistoryParams
                        {
                            Key = settings.Key,
                            Server = settings.Server,
                            Ts = newTs,
                            Wait = 1,
                        });

                    newTs = res.Ts;
                    await ProcessUpdates(res.Updates);
                }
                catch (LongPollOutdateException e)
                {
                    newTs = e.Ts;
                    _logger.LogInformation(e, "Timestamp outdated");
                }
                catch (LongPollKeyExpiredException e)
                {
                    _logger.LogInformation(e, "Key expired");
                    settings = await GetLongPollingServer();
                }
                catch (Exception e)
                {
                    _logger.LogNotHandledException(e);
                }

                await Task.Delay(TimeOutMilliseconds, cancellationToken);
            }
        }

        private async Task ProcessUpdates(IEnumerable<GroupUpdate> updates)
        {
            foreach (var update in updates)
            {
                if (update.Type != GroupUpdateType.MessageNew)
                    continue;

                var message = update.MessageNew?.Message;
                if (message == null
                    || message.Out != false
                    || !_messageParser.TryParse(message, out var instruction))
                    continue;


                if (instruction.Reply)
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
                return await _api.Groups.GetLongPollServerAsync(_config.GroupId);
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
