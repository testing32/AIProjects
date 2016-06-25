using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HomeworkTwo
{
    public class GroundPursuer : RobotBase
    {
        #region Constant
        const double SENSOR_RANGE = 20.0;
        const double LINE_OF_SIGHT_STEPS = 10.0;
        #endregion

        #region Properties
        public StateMachine<GroundPursuer> StateMachine { get; set; }
        private RobotBase AssistanceFrom { get; set; }

        public bool TargetWithinLineOfSightSteps
        {
            get
            {
                return CurrentTarget != null &&
                    TraversabilityMap.ShortestStepsToLineOfSight(Location, CurrentTarget.Location) <= LINE_OF_SIGHT_STEPS;
            }
        }

        public bool TargetWithinSensorRange
        {
            get
            {
                return Group.TargetRobots.Count > 0 &&
                    Location.Distance(Group.TargetRobots.First().Location) <= SENSOR_RANGE;
            }
        }

        #endregion

        #region Constructor

        public GroundPursuer(State<GroundPursuer> startState,
            Point position,
            float radius,
            Vector2D velocity,
            float maxSpeed,
            Vector2D heading,
            float mass,
            Vector2D scale,
            float turnRate,
            float maxForce,
            List<Node> patrolPath,
            Workspace traversabilityMap,
            Workspace visibilityMap)
            : base(position, radius, velocity, maxSpeed, heading, mass, scale, turnRate, maxForce, patrolPath, traversabilityMap, visibilityMap)
        {
            StateMachine = new StateMachine<GroundPursuer>(this);

            if (startState != null)
            {
                StateMachine.SetCurrentState(startState);
                StateMachine.ChangeState(startState);
                StateMachine.SetGlobalState(GlobalPursuerStates.TheInstance);
            }
        }

        #endregion

        #region Overrides
        public override void Update(UInt16 milliseconds)
        {
            // Update the state machine
            StateMachine.Update();

            base.Update(milliseconds);
        }

        public override bool HandleMessage(Telegram message)
        {
            // At first we aren't going to handle any messages
            return true;
        }

        #endregion

        #region Public Methods
        
        public void FindSupport()
        {
            AssistanceFrom = Group.GetBestSupportingRobot(this);
            MessageDispatcher.TheInstance.DispatchMsg(
                0, 
                this, 
                AssistanceFrom, 
                new Telegram() { ExtraInfo = CurrentTarget, MsgType = MessageType.RequestAssistance });

        }

        // Send the support back to patrolling
        public void RelieveSupport()
        {
            if (AssistanceFrom == null)
                return;

            MessageDispatcher.TheInstance.DispatchMsg(
                0,
                this,
                AssistanceFrom,
                new Telegram() { ExtraInfo = null, MsgType = MessageType.AssistanceNoLongerRequired });
            AssistanceFrom = null;
        }

        public bool HasSupport()
        {
            return AssistanceFrom != null;
        }

        internal bool NoLongerNeedSupport(List<Node> possibleTargetLocations)
        {
            return VisibilityMap.PercentageInLineOfSight(Location, possibleTargetLocations) >= .7
                || Location.Distance(CurrentTarget.Location) < LINE_OF_SIGHT_STEPS * .5;
        }

        internal bool InDangerOfLosingTarget(List<Node> possibleTargetLocations)
        {
            return VisibilityMap.PercentageInLineOfSight(Location, possibleTargetLocations) <= .5
                && Location.Distance(CurrentTarget.Location) > LINE_OF_SIGHT_STEPS * .75;
        }
        #endregion
    }
}
