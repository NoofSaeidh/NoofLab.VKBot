using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NoofLab.VKBot.Core;
using NoofLab.VKBot.Core.Configuration;

namespace NoofLab.VKBot.Console
{
    class Program
    {
        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            builder.ConfigureServices(services =>
            {
                services.AddHostedService<VkService>();
                services.AddSingleton<Config>(provider =>
                    provider
                        .GetService<IConfiguration>()
                        .GetSection("Config")
                        .Get<Config>());
            });

            return builder;
        }
    }
}
