using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class GlobalTargetStates : State<Target>
    {
        #region Singleton
        private static GlobalTargetStates _instance = null;
        private static object _lock = new object();
        private GlobalTargetStates() { }

        public static GlobalTargetStates TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new GlobalTargetStates();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(Target entity)
        {
            // no enter case yet
        }

        public override void Execute(Target entity)
        {
            // NOT IMPLEMENTED YET
        }

        public override void Exit(Target entity)
        {
            // no exit case yet
        }

        public override bool OnMessage(Target entity, Telegram telegram)
        {
            switch (telegram.MsgType)
            {
                case MessageType.GoHome:
                    break;
                case MessageType.RequestAssistance:
                    break;
                default:
                    return false;
            }
            return false;
        }
        #endregion
    }

    public class TargetRandomWalk : State<Target>
    {
        #region Singleton
        private static TargetRandomWalk _instance = null;
        private static object _lock = new object();
        private TargetRandomWalk() { }

        public static TargetRandomWalk TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new TargetRandomWalk();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(Target entity)
        {
            // no enter logic yet
        }

        public override void Execute(Target entity)
        {
        }

        public override void Exit(Target entity)
        {
            // no exit case yet
        }

        public override bool OnMessage(Target entity, Telegram telegram)
        {
            switch (telegram.MsgType)
            {
                case MessageType.GoHome:
                    break;
                case MessageType.RequestAssistance:
                    break;
                default:
                    return false;
            }
            return false;
        }
        #endregion
    }

    public class TargetPatrol : State<Target>
    {
        #region Singleton
        private static TargetPatrol _instance = null;
        private static object _lock = new object();
        private TargetPatrol() { }

        public static TargetPatrol TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new TargetPatrol();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(Target entity)
        {
            // no enter logic yet
            entity.GoBackToHomeRegion();
        }

        public override void Execute(Target entity)
        {
        }

        public override void Exit(Target entity)
        {
            // no exit case yet
        }

        public override bool OnMessage(Target entity, Telegram telegram)
        {
            switch (telegram.MsgType)
            {
                case MessageType.GoHome:
                    break;
                case MessageType.RequestAssistance:
                    break;
                default:
                    return false;
            }
            return false;
        }
        #endregion
    }
}
