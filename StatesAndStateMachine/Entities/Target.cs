using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class Target : RobotBase
    {
        
        #region Properties
        public StateMachine<Target> StateMachine { get; set; }
        #endregion

        #region Constructor
        public Target(State<Target> startState,
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
            StateMachine = new StateMachine<Target>(this);

            if (startState != null)
            {
                StateMachine.SetCurrentState(startState);
                StateMachine.ChangeState(startState);
                StateMachine.SetGlobalState(GlobalTargetStates.TheInstance);
            }
        }
        #endregion

        #region Public Methods
        public List<Node> GetPossibleLocations(float timeHorizon)
        {
            //return TraversabilityMap.GetNodes(Location, timeHorizon * Speed);
            return TraversabilityMap.GetNodes(Location, timeHorizon);
        }
        #endregion

        #region Overrides
        public override void Update(UInt16 milliseconds)
        {
            // Currenty our target is just going to sit there
            StateMachine.Update();

            base.Update(milliseconds);
        }

        public override bool HandleMessage(Telegram message)
        {
            // Currently the target will not handle any messages
            return true;
        }

        public override void UpdateWayPoint()
        {
            if ( (CurrentPath == null || CurrentWayPoint == null) || NearPoint(CurrentWayPoint) && CurrentWayPoint == CurrentPath.Last())
            {
                CurrentPath = TraversabilityMap.GetShortestPath(Location, TraversabilityMap.GenerateValidNode(null));
                return;
            }

            base.UpdateWayPoint();

        }

        #endregion
    }
}
