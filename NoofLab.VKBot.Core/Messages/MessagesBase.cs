using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NoofLab.VKBot.Core.Messages
{
    public abstract class MessagesBase
    {
        protected MessagesBase(ILogger<MessagesBase> logger)
        {
            Logger = logger;
        }

        protected ILogger<MessagesBase> Logger { get; }

        // should not be property
        public abstract string GetCulture();

        public virtual IReadOnlyDictionary<string, string> ToDictionary() =>
            new Dictionary<string, string>(GetProperties());

        protected virtual IEnumerable<KeyValuePair<string, string>> GetProperties()
        {
            foreach (var property in GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.PropertyType != typeof(string))
                    continue;
                string value;
                try
                {
                    value = property.GetValue(this) as string;
                }
                catch (Exception e)
                {
                    Logger.LogWarning(e, "Cannot read property {Property}", property);
                    continue;
                }

                yield return new(property.Name, value);
            }
        }
    }
}
