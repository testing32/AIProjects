using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeworkTwo
{
    /// <summary>
    /// Detects collitions between points and obstacles
    /// Library is implemented as a singlton
    /// </summary>
    public class CollisionDetection
    {
        #region Singleton
        private static object _lock = new object();
        private static CollisionDetection _instance = null;
        private CollisionDetection() {}
        public static CollisionDetection TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new CollisionDetection();

                return _instance;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obstacles"></param>
        /// <param name="sourcePoint"></param>
        /// <param name="destinationPoint"></param>
        /// <returns></returns>
        public bool Link(Obstacles obstacles, Point sourcePoint, Point destinationPoint)
        {
            LineSegment ourLine = new LineSegment(sourcePoint, destinationPoint);
            return ourLine.Link(obstacles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obstacles"></param>
        /// <param name="testPoint"></param>
        /// <returns></returns>
        public bool Clear(Obstacles obstacles, Point testPoint)
        {
            return testPoint.Clear(obstacles);
        }
        #endregion
    }
}
