using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NoofLab.VKBot.Core.Messages.Response
{
    public abstract class ResponseMessages : MessagesBase
    {
        protected ResponseMessages(ILogger<ResponseMessages> logger) : base(logger)
        {
        }

        public abstract string WillSchedule { get; }
    }
}
