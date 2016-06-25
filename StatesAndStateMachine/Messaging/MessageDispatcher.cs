using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class MessageDispatcher
    {
        #region Singleton
        private static MessageDispatcher _instance = null;
        private static object _lock = new object();
        private MessageDispatcher() { }

        public static MessageDispatcher TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new MessageDispatcher();

                return _instance;
            }
        }
        #endregion

        #region Public Methods
        public void DispatchMsg(double delay, Entity sender, Entity receiver, Telegram msg)
        {
            if(receiver != null)
                receiver.HandleMessage(msg);
        }

        public void DispatchDelayedMessages()
        {
        }
        #endregion

        #region Private Methods
        private void Discharge(Entity entity, Telegram msg)
        {
        }
        #endregion
    }
}
