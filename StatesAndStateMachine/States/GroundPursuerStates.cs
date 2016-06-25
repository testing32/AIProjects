using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HomeworkTwo
{
    public class GlobalPursuerStates : State<GroundPursuer>
    {
        #region Singleton
        private static GlobalPursuerStates _instance = null;
        private static object _lock = new object();
        private GlobalPursuerStates() { }

        public static GlobalPursuerStates TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new GlobalPursuerStates();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(GroundPursuer entity)
        {
            // Currently no global enter action
        }

        public override void Execute(GroundPursuer entity)
        {
            // Currently no global execute action
        }

        public override void Exit(GroundPursuer entity)
        {
            // Currently no global exit action
        }

        public override bool OnMessage(GroundPursuer entity, Telegram telegram)
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
    
    public class GroundFollowTarget : State<GroundPursuer>
    {
        #region Singleton
        private static GroundFollowTarget _instance = null;
        private static object _lock = new object();
        private GroundFollowTarget() {
        }

        public static GroundFollowTarget TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new GroundFollowTarget();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(GroundPursuer entity)
        {
            Debug.Assert(entity.CurrentTarget != null);
            entity.CurrentTask = Task.EngagingTarget;
        }

        public override void Execute(GroundPursuer entity)
        {
            // The target escaped us, return to our home
            if (false && !entity.HasSupport() && !entity.TargetWithinLineOfSightSteps)
            {
                // We aren't going to go home any more
                //entity.StateMachine.ChangeState(GroundGoHome.TheInstance);
            }
            // If moving towards the target and we have just gotten close enough to engage, change our state
            else if (entity.CurrentTask != Task.EngagedWithTarget && entity.CloseToTarget)
            {
                entity.CurrentTask = Task.EngagedWithTarget;
            }
            // If we are currently engaged with the target and we need to make a decision in this time step
            else if (entity.CurrentTask == Task.EngagedWithTarget && entity.NeedToRunTimeStep)
            {
                Debug.Assert(entity.CurrentTarget != null);

                Stopwatch timer = Stopwatch.StartNew();
                // Get the list of possible locations the target could be by the time horizon point
                List<Node> possibleTargetLocations = entity.CurrentTarget.GetPossibleLocations(entity.Group.TimeHorizon);
                
                // if the robot has support then check to see if it's no longer necessary
                if(entity.HasSupport())
                {
                    if(entity.NoLongerNeedSupport(possibleTargetLocations))
                        entity.RelieveSupport();                    
                }
                else if (entity.InDangerOfLosingTarget(possibleTargetLocations))
                {
                    entity.FindSupport();
                }
            }

            // If we haven't made a targeting decision in .1 seconds refresh our path
            if(entity.TimeSinceLastDecision() >= 200)
                entity.SetTargetPath();

            entity.NeedToRunTimeStep = false;
        }

        public override void Exit(GroundPursuer entity)
        {
            entity.CurrentTarget = null;
        }

        public override bool OnMessage(GroundPursuer entity, Telegram telegram)
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

    public class GroundPursuerPatrol : State<GroundPursuer>
    {
        #region Singleton
        private static GroundPursuerPatrol _instance = null;
        private static object _lock = new object();
        private GroundPursuerPatrol() { }

        public static GroundPursuerPatrol TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new GroundPursuerPatrol();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(GroundPursuer entity)
        {
            // We are patrolling, not following a target
            entity.ResumePatrol();
            entity.CurrentTask = Task.Patrolling;
        }

        public override void Execute(GroundPursuer entity)
        {
            // We want to look for a target to follow
            if(entity.TargetWithinSensorRange)
            {
                // We found a target, go get it
                entity.CurrentTarget = entity.Group.TargetRobots.First();
                entity.StateMachine.ChangeState(GroundFollowTarget.TheInstance);
                return;
            }
        }

        public override void Exit(GroundPursuer entity)
        {
            // Currently no exit action
        }

        public override bool OnMessage(GroundPursuer entity, Telegram telegram)
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

    public class GroundGoHome : State<GroundPursuer>
    {   
        #region Singleton
        private static GroundGoHome _instance = null;
        private static object _lock = new object();
        private GroundGoHome() { }

        public static GroundGoHome TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new GroundGoHome();

                return _instance;
            }
        }
        #endregion

        #region Overrides
        public override void Enter(GroundPursuer entity)
        {
            // Nothing here yet
            entity.GoBackToHomeRegion();
            entity.CurrentTask = Task.GoingHome;
        }

        public override void Execute(GroundPursuer entity)
        {
            if (entity.HomeNode == null || entity.NearPoint(entity.HomeNode))
            {
                entity.StateMachine.ChangeState(GroundPursuerPatrol.TheInstance);
                return;
            }
        }

        public override void Exit(GroundPursuer entity)
        {
            // Currently no exit action
        }

        public override bool OnMessage(GroundPursuer entity, Telegram telegram)
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
