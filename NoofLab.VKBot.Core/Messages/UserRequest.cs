using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NoofLab.VKBot.Core.Commands;
using VkNet.Model;

namespace NoofLab.VKBot.Core.Messages
{
    public record UserRequest(long? PeerId, Command Command)
    {

    }
}
