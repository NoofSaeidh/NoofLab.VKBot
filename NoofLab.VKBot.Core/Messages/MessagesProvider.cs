using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NoofLab.VKBot.Core.Configuration;
using NoofLab.VKBot.Core.Messages.MessagesValues;
using NoofLab.VKBot.Core.Messages.Response;

namespace NoofLab.VKBot.Core.Messages
{
    public interface IMessagesProvider
    {
        RequestMessages Requests(string culture);
        ResponseMessages Responses(string culture);
    }

    public class MessagesProvider : IMessagesProvider
    {
        private readonly ILogger<MessagesProvider> _logger;
        private readonly Dictionary<string, RequestMessages> _requests;
        private readonly Dictionary<string, ResponseMessages> _responses;

        public MessagesProvider(Config config, ILogger<MessagesProvider> logger,
            IEnumerable<RequestMessages> requests,
            IEnumerable<ResponseMessages> responses)
        {
            _logger = logger;
            if (config.DefaultCulture != null)
            {
                try
                {
                    DefaultCulture = CultureInfo.GetCultureInfo(config.DefaultCulture).Name;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Default Culture {Culture} not found", config.DefaultCulture);
                    DefaultCulture = CultureInfo.InvariantCulture.Name;
                }
            }
            else
            {
                DefaultCulture = CultureInfo.InvariantCulture.Name;
            }

            _requests = requests.ToDictionary(m => m.GetCulture(), m => m);
            _responses = responses.ToDictionary(m => m.GetCulture(), m => m);
        }

        private string DefaultCulture { get; }

        public RequestMessages Requests(string culture)
        {
            if (culture != null && _requests.TryGetValue(culture, out var messages))
                return messages;
            _logger.LogWarning("Culture {Culture} not found.", culture);
            if (_requests.TryGetValue(DefaultCulture, out messages))
                return messages;
            _logger.LogError("Default Culture {DefaultCulture} not found", DefaultCulture);
            throw new ArgumentException("Culture not found", culture);
        }

        public ResponseMessages Responses(string culture)
        {
            if (culture != null && _responses.TryGetValue(culture, out var messages))
                return messages;
            _logger.LogWarning("Culture {Culture} not found.", culture);
            if (_responses.TryGetValue(DefaultCulture, out messages))
                return messages;
            _logger.LogError("Default Culture {DefaultCulture} not found", DefaultCulture);
            throw new ArgumentException("Culture not found", culture);
        }
    }
}
