using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NoofLab.VKBot.Core.Messages.Response
{
    public class ResponseMessages_En : ResponseMessages
    {
        public ResponseMessages_En(ILogger<ResponseMessages> logger) : base(logger)
        {
        }

        public override string GetCulture() => "";

        public override string WillSchedule => "Will schedule";
    }
}
