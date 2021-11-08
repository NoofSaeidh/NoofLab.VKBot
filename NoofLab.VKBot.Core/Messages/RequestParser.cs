using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NoofLab.VKBot.Core.Configuration;
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
        private readonly Regex _mentionRegex;

        public RequestParser(Config config)
        {
            _mentionRegex = new Regex($@"^(?:\[-{config.GroupName}\|[^\]]+\])(?:[\s.,\'\""!?\-+]+|$)");
        }

        public UserRequest TryParseUserRequest(Message message)
        {
            if (BotMentioned(message.Text))
            {

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

        private bool BotMentioned(string text) => _mentionRegex.IsMatch(text);
    }
}
