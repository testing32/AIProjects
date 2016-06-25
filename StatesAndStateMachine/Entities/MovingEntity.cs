using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public abstract class MovingEntity : Entity
    {
        #region Properties
        public Vector2D Velocity { get; set; }
        public Vector2D Heading { get; set; }
        public float Mass { get; protected set; }
        public float MaxSpeed { get; set; }
        public float MaxForce { get; set; }
        public float MaxTurnRate { get; set; }
        public float Speed { get { return Velocity.Length; } }
        public float SpeedSq { get { return Velocity.LengthSq; } }
        public bool IsMaxSpeed { get { return MaxSpeed * MaxSpeed >= SpeedSq; } }
        protected Workspace TraversabilityMap { get; private set; }
        protected Workspace VisibilityMap { get; private set; }
        #endregion

        #region Constructor
        public MovingEntity(Point position,
            float radius,
            Vector2D velocity,
            float maxSpeed,
            Vector2D heading,
            float mass,
            Vector2D scale,
            float turnRate,
            float maxForce,
            Workspace traversabilityMap,
            Workspace visibilityMap)
            : base(Entity.GetNextValidID())
        {
            Location = position;
            BoundingRadius = radius;
            Velocity = velocity;
            MaxSpeed = maxSpeed;
            Heading = heading;
            Mass = mass;
            MaxTurnRate = turnRate;
            MaxForce = maxForce;
            TraversabilityMap = traversabilityMap;
            VisibilityMap = visibilityMap;
        }
        #endregion

        #region Protected Methods

        protected Vector2D GetSteeringVector(Point target)
        {
            return  Vector2D.GetVector(Location, target);
        }

        #endregion
    }
}
