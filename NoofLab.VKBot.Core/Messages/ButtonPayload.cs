using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NoofLab.VKBot.Core.Extensions.Json;

namespace NoofLab.VKBot.Core.Messages
{
    public record ButtonPayload(string ButtonId)
    {
        public static implicit operator string(ButtonPayload payload) => payload.ToJson();
        public static ButtonPayload FromJson(string json) => JsonConvert.DeserializeObject<ButtonPayload>(json);
    }
}
