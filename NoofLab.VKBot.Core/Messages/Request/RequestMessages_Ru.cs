using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NoofLab.VKBot.Core.Messages.MessagesValues
{
    public class RequestMessages_Ru : RequestMessages
    {
        public RequestMessages_Ru(ILogger<RequestMessages> logger) : base(logger)
        {
        }

        public override string GetCulture() => "ru-RU";

        public override string Schedule => "напомни";
    }
}
