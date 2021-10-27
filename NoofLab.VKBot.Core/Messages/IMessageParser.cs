using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoofLab.VKBot.Core.Messages
{
    public interface IMessageParser
    {
        bool TryParse(string messageText, out Instruction instruction);
    }
}
