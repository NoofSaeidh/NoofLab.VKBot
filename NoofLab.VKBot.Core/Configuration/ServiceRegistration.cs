using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NoofLab.VKBot.Core.Messages;
using NoofLab.VKBot.Core.Messages.MessagesValues;
using NoofLab.VKBot.Core.Messages.Response;
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
            services.AddSingleton<RequestMessages, RequestMessages_En>();
            services.AddSingleton<RequestMessages, RequestMessages_Ru>();
            services.AddSingleton<ResponseMessages, ResponseMessages_En>();
            services.AddSingleton<ResponseMessages, ResponseMessages_Ru>();
            services.AddSingleton<IMessagesProvider, MessagesProvider>();
        }
    }
}
