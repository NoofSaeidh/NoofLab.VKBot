using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace NoofLab.VKBot.Core.Messages
{
    public interface IRequestParser
    {
        UserRequest TryParseUserRequest(Message message);
        CallbackRequest TryParseCallbackRequest(MessageEvent message);
    }

    public class RequestParser : IRequestParser
    {
        public UserRequest TryParseUserRequest(Message message)
        {
            if (message?.Text?.Contains("reply", StringComparison.InvariantCultureIgnoreCase) is true)
            {
                return new UserRequest(message.PeerId);
            }
            return null;
        }

        public CallbackRequest TryParseCallbackRequest(MessageEvent message)
        {
            return new CallbackRequest(
                message.EventId,
                message.UserId.GetValueOrDefault(),
                message.PeerId.GetValueOrDefault());
        }
    }
}
