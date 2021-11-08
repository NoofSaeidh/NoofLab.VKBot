using System;
using System.Collections.Generic;
using System.Linq;
using NoofLab.VKBot.Core.Commands;
using NoofLab.VKBot.Core.Extensions.Json;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace NoofLab.VKBot.Core.Messages
{
    public interface IResponseBuilder
    {
        MessagesSendParams BuildResponse(UserRequest request);
        EventData BuildResponse(CallbackRequest request);
    }

    public class ResponseBuilder : IResponseBuilder
    {
        private readonly IMessagesProvider _messages;

        public ResponseBuilder(IMessagesProvider messages)
        {
            _messages = messages;
        }

        public MessagesSendParams BuildResponse(UserRequest request)
        {
            return new MessagesSendParams
            {
                PeerId = request.PeerId,
                RandomId = DateTime.UtcNow.Ticks,
                Message = request.Command switch
                {
                    ReminderCommand reminder => BuildText(reminder), 
                    _                        => throw new NotImplementedException(),
                }
            };
        }

        public EventData BuildResponse(CallbackRequest request)
        {
            return new EventData
            {
                Type = MessageEventType.SnowSnackbar,
                Text = "Clicked!",
            };
        }

        private string BuildText(ReminderCommand reminder)
        {
            return reminder.ReminderText;
        }
    }
}
