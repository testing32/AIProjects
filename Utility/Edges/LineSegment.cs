using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkTwo
{
    /// <summary>
    /// Line in the two dimentional plane
    /// </summary>
    public class LineSegment
    {
        #region Properties
        public virtual Point FirstPoint { get; private set; }
        public virtual Point SecondPoint { get; private set; }

        /// <summary>
        /// Returns the length of the line segment
        /// </summary>
        public virtual double Length
        {
            get
            {
                return FirstPoint.Distance(SecondPoint);
            }
        }

        #endregion

        #region Constructor
        protected LineSegment() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="firstPoint">First intersection point</param>
        /// <param name="secondPoint">Second intersection point</param>
        public LineSegment(Point firstPoint, Point secondPoint)
        {
            // Sets the left most point to be the first point and the right most point to the second point
            FirstPoint = firstPoint.X < secondPoint.X ? firstPoint : secondPoint;
            SecondPoint = firstPoint.X >= secondPoint.X ? firstPoint : secondPoint;
        }
        #endregion

        #region Public Methods
        
        public bool Link(Obstacles obstacles)
        {
            List<Point> testPoints = GetPoints(50); 
            // We are going to sample 50 points along our line to look for collisions
            foreach (Obstacle obstacle in obstacles)
            {
                // Checks to see if the point has any chance of colliding with the obstacle
                // If there isn't a chance then don't test all of the points
                if((FirstPoint.X > obstacle.MaxX && SecondPoint.X > obstacle.MaxX) ||
                    (FirstPoint.X < obstacle.MinX && SecondPoint.X < obstacle.MinX) ||
                    (FirstPoint.Y > obstacle.MaxY && SecondPoint.Y > obstacle.MaxY) ||
                    (FirstPoint.Y < obstacle.MinY && SecondPoint.Y < obstacle.MinY))
                    continue;

                foreach(Point point in testPoints)
                    if(obstacle.Collides(point))
                        return false;
            }
            return true;
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Returns a list of evenly distributed points along the line segment
        /// </summary>
        /// <param name="numberOfPoints">The number of points we want</param>
        /// <returns>List of evenly distributed points</returns>
        private List<Point> GetPoints(UInt16 numberOfPoints)
        {
            numberOfPoints++;
            List<Point> points = new List<Point>();
            double xIncrement = (SecondPoint.X - FirstPoint.X) / (double)numberOfPoints;
            double yIncrement = (SecondPoint.Y - FirstPoint.Y) / (double)numberOfPoints;
            
            for (UInt16 i = 1; i < numberOfPoints; i++)
            {
                points.Add(new Point(FirstPoint.X + i * xIncrement, FirstPoint.Y + i * yIncrement));
            }
            return points;
        }

        #endregion
    }
}
