using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NoofLab.VKBot.Core.Messages.MessagesValues
{
    public abstract class RequestMessages : MessagesBase
    {
        protected RequestMessages(ILogger<RequestMessages> logger) : base(logger)
        {
        }

        public abstract string Schedule { get; }
    }
}
