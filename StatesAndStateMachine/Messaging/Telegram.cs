using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public enum MessageType
    {
        RequestAssistance,
        AssistanceNoLongerRequired,
        GoHome
    }

    public class Telegram
    {
        public object ExtraInfo { get; set; }
        public MessageType MsgType { get; set; }
    }
}
