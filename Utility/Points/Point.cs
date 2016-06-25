using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkTwo
{
    /// <summary>
    /// Point in a two dimentional plane
    /// </summary>
    public class Point
    {
        #region Constructors
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Point(double x, double y) : this((float)x, (float)y) { }
        #endregion

        #region Properties
        public float X { get; set; }
        public float Y { get; set; }
        #endregion

        #region Public Methods

        public double Distance(Point other)
        {
            return Math.Sqrt((X - other.X) * (X - other.X) + 
                (Y - other.Y) * (Y - other.Y));
        }

        public bool Clear(Obstacles obstacles)
        {
            foreach (Obstacle obstacle in obstacles)
            {
                if (obstacle.Collides(this))
                    return false;
            }

            return true;
        }

        #endregion

        #region Overrides

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is Point))
                return false;

            Point other = (Point)obj;

            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
