using System;
using System.Collections.Generic;
using System.Linq;
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
        public MessagesSendParams BuildResponse(UserRequest request)
        {
            return new MessagesSendParams
            {
                Message = "I can reply!",
                PeerId = request.PeerId,
                RandomId = DateTime.UtcNow.Ticks,
                Keyboard = new MessageKeyboard
                {
                    Inline = true,
                    Buttons = new[]
                    {
                        new[]
                        {
                            new MessageKeyboardButton
                            {
                                Color = KeyboardButtonColor.Positive,
                                Action = new MessageKeyboardButtonAction
                                {
                                    Type = KeyboardButtonActionType.Callback,
                                    Label = "Click It",
                                    Payload = new { button = "a1" }.ToJson(),
                                },
                            },
                            new MessageKeyboardButton
                            {
                                Color = KeyboardButtonColor.Default,
                                Action = new MessageKeyboardButtonAction
                                {
                                    Type = KeyboardButtonActionType.Callback,
                                    Label = "Don't click it",
                                    Payload = new { button = "a2" }.ToJson(),
                                },
                            },
                        }
                    }
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
    }
}
