using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NoofLab.VKBot.Core.Messages.MessagesValues
{
    public class RequestMessages_En : RequestMessages
    {
        public RequestMessages_En(ILogger<RequestMessages> logger) : base(logger)
        {
        }

        public override string GetCulture() => "";

        public override string Schedule => "Schedule";
    }
}
