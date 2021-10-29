using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NoofLab.VKBot.Core.Extensions.Json
{
    public static class JsonExtensions
    {
        public static string ToJson(this object instance) => JsonConvert.SerializeObject(instance, Formatting.None);
    }
}
