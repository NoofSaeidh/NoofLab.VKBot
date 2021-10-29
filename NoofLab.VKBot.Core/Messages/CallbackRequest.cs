using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model;

namespace NoofLab.VKBot.Core.Messages
{
    public record CallbackRequest(string EventId, long UserId, long PeerId)
    {

    }
}
