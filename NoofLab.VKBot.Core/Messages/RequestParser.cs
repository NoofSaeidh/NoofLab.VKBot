using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NoofLab.VKBot.Core.Commands;
using NoofLab.VKBot.Core.Configuration;
using VkNet.Enums;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.GroupUpdate;

namespace NoofLab.VKBot.Core.Messages
{
    public interface IRequestParser
    {
        UserRequest TryParseUserRequest(MessageNew message);
        CallbackRequest TryParseCallbackRequest(MessageEvent message);
    }

    public class RequestParser : IRequestParser
    {
        private readonly IMessagesProvider _messages;
        private readonly Regex _mentionRegex;

        public RequestParser(Config config, IMessagesProvider messages)
        {
            _messages = messages;
            _mentionRegex = new Regex($@"^(?:\[{config.GroupName}\|[^\]]+\])(?:[\s.,\'\""!?\-+]+|$)");
        }

        public UserRequest TryParseUserRequest(MessageNew message)
        {
            // todo: make it normal
            string lang = message.ClientInfo.LangId == Language.Ru ? "ru-RU" : "";
            if (BotMentioned(message.Message.Text))
            {
                if (message.Message.Text.Contains(_messages.Requests(lang).Schedule))
                    return new UserRequest(message.Message.PeerId,
                        new ReminderCommand(default, _messages.Responses(lang).WillSchedule));
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
