using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkTwo
{
    /// <summary>
    /// Obstacle in a two dimentional plane
    /// </summary>
    public abstract class Obstacle
    {
        #region Properties
        public Point Location { get; private set; }
        public abstract double MaxX { get; }
        public abstract double MaxY { get; }
        public abstract double MinX { get; }
        public abstract double MinY { get; }
        public double Width { get { return MaxX - MinX; } }
        public double Height { get { return MaxY - MinY; } }
        #endregion

        #region Constructor
        public Obstacle(Point location)
        {
            Location = location;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Checks to see whether a point collides with this obstacle
        /// </summary>
        /// <param name="testPoint">The point we are testing against the obstacle</param>
        /// <returns>True if the point collides, False if we do not collide</returns>
        public abstract bool Collides(Point testPoint);
        #endregion
    }
}
