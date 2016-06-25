using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class CircleObstacle : Obstacle
    {
        #region Properties
        public double Radius { get; set; }

        public override double MaxX
        {
            get { return Location.X + Radius; }
        }

        public override double MaxY
        {
            get { return Location.Y + Radius; }
        }

        public override double MinX
        {
            get { return Location.X - Radius; }
        }

        public override double MinY
        {
            get { return Location.Y - Radius; }
        }

        #endregion

        #region Constructor
        public CircleObstacle(Point location, Double radius) : base(location)
        {
            Radius = radius;
        }
        #endregion

        #region Overrides
        public override bool Collides(Point testPoint)
        {
            return testPoint.X >= MinX &&
                testPoint.X <= MaxX &&
                testPoint.Y <= Location.Y + YHeight(testPoint.X) &&
                testPoint.Y >= Location.Y - YHeight(testPoint.X);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Gets the height of the y coordiante of the obstacle as a given x coordiante
        /// </summary>
        /// <param name="xLocation">The x location where we want to know the 
        /// height of the obstalce in the y direction </param>
        /// <returns>The y height at a given x coordinate</returns>
        private double YHeight(double xLocation)
        {
            double xDifference = Location.X - xLocation;
            return Math.Sqrt((Radius * Radius) - (xDifference * xDifference));
        }
        #endregion
    }
}
