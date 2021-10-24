using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NoofLab.VKBot.Core.Configuration;

namespace NoofLab.VKBot.Core
{
    public class VkService : IHostedService
    {
        private readonly Config _config;

        public VkService(Config config)
        {
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
