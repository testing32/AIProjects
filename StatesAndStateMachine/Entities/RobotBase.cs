using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public enum Task
    {
        Patrolling,
        EngagingTarget,
        EngagedWithTarget,
        GoingHome
    }

    public abstract class RobotBase : MovingEntity
    {
        #region Constants
        const double MIN_FOLLOW_DISTANCE = 2.0;
        #endregion

        #region Member Variables
        UInt16 _currentWayPointIndex;
        List<Node> _currentPath;
        List<Node> _patrolPath;
        #endregion

        #region Property
        public RobotGroup Group { get; set; }
        public List<Node> CurrentPath 
        {
            get
            {
                return _currentPath;
            }
            set
            {
                _currentPath = value;
                _currentWayPointIndex = 0;
            }
        }
        public Node CurrentWayPoint { 
            get
            {
                return CurrentPath != null && CurrentPath.Count > _currentWayPointIndex 
                    ? CurrentPath[_currentWayPointIndex]
                    : null; 
            }
        }
        public Node HomeNode
        {
            get
            {
                if (_patrolPath == null || _patrolPath.Count == 0)
                    return null;

                Node closestNode = null;
                double minDistance = double.MaxValue;
                foreach (Node node in _patrolPath)
                {
                    double currentDistance = Location.Distance(node);
                    if (currentDistance < minDistance)
                    {
                        minDistance = currentDistance;
                        closestNode = node;
                    }
                }

                return closestNode;
            }
        }
        public Task CurrentTask { get; set; }

        public Target CurrentTarget { get; set; }
        public bool NeedToRunTimeStep { get; set; }

        protected DateTime LastDecisionPoint { get; set; }
        protected bool HasPatrolPath
        {
            get { return _patrolPath != null && _patrolPath.Count > 0; }
        }
        #endregion

        #region Constructor
        public RobotBase(Point position,
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
            : base(position, radius, velocity, maxSpeed, heading, mass, scale, turnRate, maxForce, traversabilityMap, visibilityMap)
        {
            CurrentTarget = null;
            NeedToRunTimeStep = false;
            LastDecisionPoint = DateTime.MinValue;
            CurrentPath = null;
            _patrolPath = patrolPath;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// True if withing the min follow distance of the target
        /// </summary>
        public bool CloseToTarget
        {
            get { return CurrentTarget == null ? false : Location.Distance(CurrentTarget.Location) < MIN_FOLLOW_DISTANCE; }
        }

        /// <summary>
        /// Returns milliseconds since the last path update
        /// </summary>
        /// <returns></returns>
        public UInt16 TimeSinceLastDecision()
        {
            return (UInt16)(DateTime.UtcNow - LastDecisionPoint).Milliseconds;
        }

        public void SetTargetPath()
        {
            if (CurrentTarget == null)
                return;

            // If the target is in a valid region go toward the target
            // If the target is in an invalid region go toward the closest point on the target's
            // path that you can get to
            Point validTargetLocation = TraversabilityMap.IsPointValid(CurrentTarget.Location)
                ? CurrentTarget.Location
                : TraversabilityMap.GetValidNode(CurrentTarget.GetFuturePath());

            // If we are currently too close to the target or the target is in an untrackable location, stop approaching
            if (validTargetLocation == null || Location.Distance(validTargetLocation) < MIN_FOLLOW_DISTANCE)
                CurrentPath = new List<Node>() { new Node(Location.X, Location.Y) };
            else
                CurrentPath = TraversabilityMap.GetShortestPath(Location, validTargetLocation);

            LastDecisionPoint = DateTime.UtcNow;
        }

        /// <summary>
        /// Returns true if the robot is near a given point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool NearPoint(Point point)
        {
            return point == null
                ? false
                : Location.Distance(point) < .3;
        }

        /// <summary>
        /// Returns the future path of the robot
        /// </summary>
        /// <returns></returns>
        public List<Node> GetFuturePath()
        {
            if (CurrentPath == null)
                return new List<Node>();
            else
                return CurrentPath.GetRange(_currentWayPointIndex, CurrentPath.Count - _currentWayPointIndex);
        }

        public void GoBackToHomeRegion()
        {
            CurrentPath = TraversabilityMap.GetShortestPath(Location, HomeNode);
        }

        public void ResumePatrol()
        {
            CurrentPath = _patrolPath;
            _currentWayPointIndex = (ushort)CurrentPath.IndexOf(HomeNode);
        }

        /// <summary>
        /// Updates the current way point
        /// </summary>
        public virtual void UpdateWayPoint()
        {
            if (CurrentPath != null && CurrentPath.Count > 1 && CurrentWayPoint != null && NearPoint(CurrentWayPoint))
                _currentWayPointIndex = (UInt16)((_currentWayPointIndex + 1) % CurrentPath.Count);
        }

        /// <summary>
        /// Updates the location of the robot and the current waypoint
        /// </summary>
        /// <param name="milliseconds"></param>
        public override void Update(ushort milliseconds)
        {
            UpdateWayPoint();
            UpdateLocation(milliseconds);
        }

        /// <summary>
        /// Updates the location of the robot based on the speed and heading
        /// </summary>
        /// <param name="milliseconds"></param>
        public void UpdateLocation(UInt16 milliseconds)
        {
            if (CurrentWayPoint != null)
            {
                Heading = GetSteeringVector(CurrentWayPoint).Normalize();
                Heading.Length = Heading.Length * (MaxSpeed * milliseconds / 1000f);
                Location.X += Heading.X;
                Location.Y += Heading.Y;
            }
        }
        #endregion

    }
}
