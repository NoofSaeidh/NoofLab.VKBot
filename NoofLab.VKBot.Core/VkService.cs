using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NoofLab.VKBot.Core.Configuration;
using NoofLab.VKBot.Core.Exceptions;
using NoofLab.VKBot.Core.Extensions.Json;
using NoofLab.VKBot.Core.Extensions.Logger;
using NoofLab.VKBot.Core.Extensions.Validation;
using NoofLab.VKBot.Core.Messages;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace NoofLab.VKBot.Core
{
    public class VkService : BackgroundService
    {
        private readonly Config _config;
        private readonly VkApi _api;
        private readonly ILogger<VkService> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IRequestParser _requestParser;
        private readonly IResponseBuilder _responseBuilder;

        private VkValues _values;

        private const int TimeOutMilliseconds = 100; // todo: research

        public VkService(
            Config config,
            VkApi api,
            ILogger<VkService> logger,
            IHostApplicationLifetime applicationLifetime,
            IRequestParser requestParser,
            IResponseBuilder responseBuilder)
        {
            _config = config;
            _api = api;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            _requestParser = requestParser;
            _responseBuilder = responseBuilder;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                Validate();
                await Authorize();
                await InitializeValues();
                await BackgroundWork(cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error prevents Vk Bot execution. Application will be stopped.");
                _applicationLifetime.StopApplication();
            }
        }

        private async Task InitializeValues()
        {
            // ReSharper disable once PossibleInvalidOperationException
            var userId = _api.UserId.Value;
            //"[club208214573|Noof Bot]"
            //var res = await InvokeUnhandled(_api.Friends.GetAsync(new FriendsGetParams { }));
        }

        private void Validate()
        {
            try
            {
                _config.Validate();
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Config is not valid");
                throw;
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
                    _logger.LogUnhandledException(e);
                }

                await Task.Delay(TimeOutMilliseconds, cancellationToken);
            }
        }

        private async Task ProcessUpdates(IEnumerable<GroupUpdate> updates)
        {
            foreach (var update in updates)
            {
                if (update.Type == GroupUpdateType.MessageNew)
                {
                    var request = _requestParser.TryParseUserRequest(update.MessageNew?.Message);
                    if (request == null)
                        continue;

                    var response = _responseBuilder.BuildResponse(request);
                    // todo: check response
                    await InvokeUnhandled(_api.Messages.SendAsync(response));
                }

                else if (update.Type == GroupUpdateType.MessageEvent)
                {
                    var request = _requestParser.TryParseCallbackRequest(update.MessageEvent);
                    if (request == null)
                        continue;

                    var response = _responseBuilder.BuildResponse(request);
                    // todo: check response
                    await InvokeUnhandled(_api.Messages.SendMessageEventAnswerAsync(
                        request.EventId, request.UserId,
                        request.PeerId, response));
                }
            }
        }

        private async Task InvokeUnhandled(Task task, bool throwOnException = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            // ReSharper disable ExplicitCallerInfoArgument
            await InvokeUnhandled(
                Task.Run(async () =>
                {
                    await task;
                    return true;
                }),
                throwOnException,
                memberName,
                sourceFilePath,
                sourceLineNumber);
                // ReSharper restore ExplicitCallerInfoArgument
        }

        private async Task<TResult> InvokeUnhandled<TResult>(Task<TResult> task, bool throwOnException = false,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                return await task;
            }
            catch (Exception e)
            {
                // ReSharper disable ExplicitCallerInfoArgument
                _logger.LogUnhandledException(e, memberName, sourceFilePath, sourceLineNumber);
                // ReSharper restore ExplicitCallerInfoArgument
                if (throwOnException)
                    throw;
                return default;
            }
        }

        private async Task Authorize()
        {
            try
            {
                await _api.AuthorizeAsync(new ApiAuthParams { AccessToken = _config.AccessToken });
                //_logger.LogTrace("Token: {Token}", _api.Token);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error during authorization");
                throw;
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
                throw;
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Unexpected exception during configuring long polling");
                throw;
            }
        }
    }
}
