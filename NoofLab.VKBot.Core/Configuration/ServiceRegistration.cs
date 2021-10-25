using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VkNet;

namespace NoofLab.VKBot.Core.Configuration
{
    public static class ServiceRegistration
    {
        public static void Register(IServiceCollection services)
        {
            services.AddHostedService<VkService>();
            var api = new VkApi(services);
            services.AddSingleton(_ => api);
        }
    }
}
