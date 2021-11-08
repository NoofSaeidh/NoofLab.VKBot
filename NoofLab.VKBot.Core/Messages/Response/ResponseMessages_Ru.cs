using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NoofLab.VKBot.Core.Messages.Response
{
    public class ResponseMessages_Ru : ResponseMessages
    {
        public ResponseMessages_Ru(ILogger<ResponseMessages> logger) : base(logger)
        {
        }

        public override string GetCulture() => "ru-RU";

        public override string WillSchedule => "Напоминание добавленно";
    }
}
