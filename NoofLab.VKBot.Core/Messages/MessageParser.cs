using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoofLab.VKBot.Core.Messages
{
    public class MessageParser : IMessageParser
    {
        public bool TryParse(string messageText, out Instruction instruction)
        {
            if (messageText?.Contains("reply", StringComparison.InvariantCultureIgnoreCase) is true)
            {
                instruction = new Instruction(true);
                return true;
            }

            instruction = null;
            return false;
        }
    }
}
