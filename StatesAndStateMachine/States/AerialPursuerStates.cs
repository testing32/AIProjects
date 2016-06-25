using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class GlobalAerialPursuerStates : State<AerialPursuer>
    {
        #region Singleton
        private static GlobalAerialPursuerStates _instance = null;
        private static object _lock = new object();
        private GlobalAerialPursuerStates() { }

        public static GlobalAerialPursuerStates TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new GlobalAerialPursuerStates();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(AerialPursuer entity)
        {
            // no enter logic yet
        }

        public override void Execute(AerialPursuer entity)
        {
            // NO LOGIC HERE YET
        }

        public override void Exit(AerialPursuer entity)
        {
            // NO LOGIC HERE YET
        }

        public override bool OnMessage(AerialPursuer entity, Telegram telegram)
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

    public class AerialFollowTarget : State<AerialPursuer>
    {
        #region Singleton
        private static AerialFollowTarget _instance = null;
        private static object _lock = new object();
        private AerialFollowTarget() { }

        public static AerialFollowTarget TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new AerialFollowTarget();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(AerialPursuer entity)
        {
            entity.CurrentTask = Task.EngagingTarget;
        }

        public override void Execute(AerialPursuer entity)
        {
            // If we haven't made a targeting decision in .1 seconds refresh our path
            if (entity.TimeSinceLastDecision() >= 200)
                entity.SetTargetPath();
        }

        public override void Exit(AerialPursuer entity)
        {
            // NOT IMPLEMENTED YET
        }

        public override bool OnMessage(AerialPursuer entity, Telegram telegram)
        {
            switch (telegram.MsgType)
            {
                case MessageType.GoHome:
                    break;
                case MessageType.RequestAssistance:
                    break;
                case MessageType.AssistanceNoLongerRequired:
                    entity.CurrentTarget = null;
                    entity.StateMachine.ChangeState(AerialPursuerGoHome.TheInstance);
                    break;
                default:
                    return false;
            }
            return false;
        }
        #endregion
    }

    public class AerialPursuerPatrol : State<AerialPursuer>
    {
        #region Singleton
        private static AerialPursuerPatrol _instance = null;
        private static object _lock = new object();
        private AerialPursuerPatrol() { }

        public static AerialPursuerPatrol TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new AerialPursuerPatrol();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(AerialPursuer entity)
        {
            entity.ResumePatrol();
            entity.CurrentTask = Task.Patrolling;
        }

        public override void Execute(AerialPursuer entity)
        {
        }

        public override void Exit(AerialPursuer entity)
        {
            // NO LOGIC HERE YET
        }

        public override bool OnMessage(AerialPursuer entity, Telegram telegram)
        {
            switch (telegram.MsgType)
            {
                case MessageType.GoHome:
                    break;
                case MessageType.RequestAssistance:
                    entity.CurrentTarget = (Target)telegram.ExtraInfo;
                    entity.StateMachine.ChangeState(AerialFollowTarget.TheInstance);
                    return true;
                default:
                    break;
            }
            return false;
        }
        #endregion
    }

    public class AerialPursuerGoHome : State<AerialPursuer>
    {
        #region Singleton
        private static AerialPursuerGoHome _instance = null;
        private static object _lock = new object();
        private AerialPursuerGoHome() { }

        public static AerialPursuerGoHome TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new AerialPursuerGoHome();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(AerialPursuer entity)
        {
            entity.GoBackToHomeRegion();
            entity.CurrentTask = Task.GoingHome;
        }

        public override void Execute(AerialPursuer entity)
        {
            if (entity.NearPoint(entity.HomeNode))
            {
                entity.StateMachine.ChangeState(AerialPursuerPatrol.TheInstance);
                return;
            }
        }

        public override void Exit(AerialPursuer entity)
        {
            // NO LOGIC HERE YET
        }

        public override bool OnMessage(AerialPursuer entity, Telegram telegram)
        {
            switch (telegram.MsgType)
            {
                case MessageType.GoHome:
                    break;
                case MessageType.RequestAssistance:
                    entity.CurrentTarget = (Target)telegram.ExtraInfo;
                    entity.StateMachine.ChangeState(AerialFollowTarget.TheInstance);
                    return true;
                default:
                    break;
            }
            return false;
        }
        #endregion
    }
}
