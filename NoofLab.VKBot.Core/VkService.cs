using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NoofLab.VKBot.Core.Configuration;
using VkNet;
using VkNet.Model;

namespace NoofLab.VKBot.Core
{
    public class VkService : BackgroundService
    {
        private readonly Config _config;
        private readonly VkApi _api;
        private readonly ILogger<VkService> _logger;
        private readonly IHostApplicationLifetime _applicationLifetime;

        public VkService(Config config, VkApi api, ILogger<VkService> logger, IHostApplicationLifetime applicationLifetime)
        {
            _config = config;
            _api = api;
            _logger = logger;
            _applicationLifetime = applicationLifetime;
            //_applicationLifetime.ApplicationStarted.Register(() => { });
            //_applicationLifetime.ApplicationStopped.Register(() => { });
            //_applicationLifetime.ApplicationStopping.Register(() => { });
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await Authorize();
            await Test();
        }


        //public override Task StopAsync(CancellationToken cancellationToken)
        //{
        //    return Task.CompletedTask;
        //}

        public async Task Authorize()
        {
            try
            {
                await _api.AuthorizeAsync(new ApiAuthParams { AccessToken = _config.ApiKey });
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error during authorization");
                _applicationLifetime.StopApplication();
            }
        }

        private async Task Test()
        {
        }

        public override void Dispose()
        {
            _api?.Dispose();
        }
    }
}
