using Microsoft.Extensions.DependencyInjection;
using NoofLab.VKBot.Core.Messages;
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
            services.AddSingleton<IRequestParser, RequestParser>();
            services.AddSingleton<IResponseBuilder, ResponseBuilder>();
        }
    }
}
