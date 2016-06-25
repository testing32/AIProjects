using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public enum EntityType
    {
        ArialPursuer,
        GroundPursuer,
        Target
    }

    public abstract class Entity
    {
        #region Member Variables
        static UInt16 _nextValidID = 0;
        #endregion

        #region Properties
        public UInt16 ID { get; private set; }
        public Point Location { get; set; }
        public double BoundingRadius { get; set; }
        public EntityType Type { get; set; }
        #endregion

        #region Constructor
        public Entity(UInt16 id) { ID = id; }
        #endregion

        #region Public Methods
        public static UInt16 GetNextValidID()
        {
            return _nextValidID++;
        }

        public static void ResetNextValidID()
        {
            _nextValidID = 0;
        }
        #endregion

        #region Abstract Methods
        public abstract void Update(UInt16 milliseconds);
        public abstract bool HandleMessage(Telegram message);
        #endregion
    }
}
