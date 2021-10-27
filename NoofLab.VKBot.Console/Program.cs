using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NoofLab.VKBot.Core.Configuration;

namespace NoofLab.VKBot.Console
{
    class Program
    {
        static Task Main(string[] args)
        {
            return CreateHostBuilder(args).RunConsoleAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            //builder.ConfigureLogging(ctx => ctx.ClearProviders().AddJsonConsole());

            builder.ConfigureServices(ServiceRegistration.Register);
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<Config>(provider => provider
                        .GetService<IConfiguration>()
                        .GetSection("Config")
                        .Get<Config>());
            });

            return builder;
        }
    }
}
