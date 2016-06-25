using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class AerialPursuer : RobotBase
    {
        #region Member Variables
        #endregion

        #region Properties
        public StateMachine<AerialPursuer> StateMachine { get; set; }
        #endregion

        #region Constructor
        public AerialPursuer(State<AerialPursuer> startState,
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
            StateMachine = new StateMachine<AerialPursuer>(this);

            if (startState != null)
            {
                StateMachine.SetCurrentState(startState);
                StateMachine.ChangeState(startState);
                StateMachine.SetGlobalState(GlobalAerialPursuerStates.TheInstance);
            }
        }
        #endregion

        #region Overrides
        public override void Update(UInt16 milliseconds)
        {
            StateMachine.Update();

            base.Update(milliseconds);
        }

        public override bool HandleMessage(Telegram message)
        {
            return StateMachine.HandleMessage(message);
        }

        #endregion
    }
}
