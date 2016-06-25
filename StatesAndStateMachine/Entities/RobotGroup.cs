using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class RobotGroup
    {
        #region Member Variables
        float _timeStepLength;
        DateTime _lastTimeStepUpdate;
        float _visiblityCriteria;
        #endregion

        #region Properties
        public List<Target> TargetRobots { get; private set; }
        public List<RobotBase> PursuitRobots { get; private set; }
        public float TimeHorizon { get; set; } //private set; } changed for timing reasons
        #endregion

        #region Constructor
        public RobotGroup(List<RobotBase> pursuerRobots, List<Target> targetRobots, float timeStepLength, float timeHorizon, float visibilityCriteria)
        {
            PursuitRobots = pursuerRobots;
            TargetRobots = targetRobots;
            TimeHorizon = timeHorizon;

            _timeStepLength = timeStepLength;
            _lastTimeStepUpdate = DateTime.UtcNow;
            _visiblityCriteria = visibilityCriteria;
            
            foreach (RobotBase robot in PursuitRobots)
                robot.Group = this;

            foreach (Target robot in TargetRobots)
                robot.Group = this;
        }
        #endregion

        #region Public Methods

        public void Update(UInt16 milliseconds)
        {
            bool resetTimeSteps = false;
            if ((DateTime.UtcNow - _lastTimeStepUpdate).Milliseconds * 1000 > _timeStepLength)
            {
                _lastTimeStepUpdate = DateTime.UtcNow;
                resetTimeSteps = true;
            }

            foreach (RobotBase robot in PursuitRobots)
            {
                if (resetTimeSteps)
                    robot.NeedToRunTimeStep = true;
                robot.Update(milliseconds);
            }

            foreach (RobotBase target in TargetRobots)
                target.Update(milliseconds);
        }

        public RobotBase GetBestSupportingRobot(GroundPursuer groundPursuer)
        {
            RobotBase bestMatch = null;
            foreach(RobotBase robot in PursuitRobots.Where(o => o != groundPursuer))
            {
                bestMatch = robot;
            }

            return bestMatch;

        }
        #endregion
    }
}
