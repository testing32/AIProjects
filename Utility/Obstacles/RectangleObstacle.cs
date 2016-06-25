using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class RectangleObstacle : Obstacle
    {
        #region Member Variables
        double _maxX;
        double _minX;

        double _maxY;
        double _minY;
        #endregion

        #region Properties
        public override double MaxX { get { return _maxX; } }
        public override double MinX { get { return _minX; } }

        public override double MaxY { get { return _maxY; } }
        public override double MinY { get { return _minY; } }
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        /// <param name="height">Height of the rectangle obstacle</param>
        /// <param name="width">Width of the rectangle obstacle</param>
        /// <param name="location">Center of the object</param>
        public RectangleObstacle(Point location, double width, double height) : base(location)
        {
            _maxX = Location.X + width / 2.0;
            _minX = Location.X - width / 2.0;
            _maxY = Location.Y + height / 2.0;
            _minY = Location.Y - height / 2.0;
        }
        #endregion
        
        #region Overrides
        public override bool Collides(Point testPoint)
        {
            return testPoint.X <= Location.X + (Width / 2.0) &&
                testPoint.X >= Location.X - (Width / 2.0) &&
                testPoint.Y >= Location.Y - (Height / 2.0) &&
                testPoint.Y <= Location.Y + (Height / 2.0);
        }
        #endregion

    }
}
