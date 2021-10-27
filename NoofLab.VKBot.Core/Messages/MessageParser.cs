using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model;

namespace NoofLab.VKBot.Core.Messages
{
    public class MessageParser : IMessageParser
    {
        public bool TryParse(Message message, out Instruction instruction)
        {
            if (message?.Text?.Contains("reply", StringComparison.InvariantCultureIgnoreCase) is true)
            {
                instruction = new Instruction(true);
                return true;
            }

            instruction = null;
            return false;
        }
    }
}
