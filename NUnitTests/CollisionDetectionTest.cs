using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HomeworkTwo
{
    [TestFixture]
    class CollisionDetectionTest
    {
        #region Properties

        private Obstacles currentObstacles { get; set; }

        #endregion

        [TestFixtureSetUp]
        public void TestPreProcessing()
        {
            currentObstacles = new Obstacles();
            currentObstacles.Add(new CircleObstacle(new Point(4,4), 2));
        }

        [Test]
        public void LinkTest()
        {
            Point nonCollisionPointOne = new Point(1, 1);
            Point nonCollisionPointTwo = new Point(-1, 1);

            Point collisionPointOne = new Point(-1, 1);
            Point collisionPointTwo = new Point(3, 4);

            Assert.True(CollisionDetection.TheInstance.Link(currentObstacles, nonCollisionPointOne, nonCollisionPointTwo));
            Assert.False(CollisionDetection.TheInstance.Link(currentObstacles, collisionPointOne, collisionPointTwo));
        }

        [Test]
        public void ClearTest()
        {
            List<Point> nonCollisionPoints = new List<Point>()
            {
                new Point(1, 1),
                new Point(-1,1)
            };

            List<Point> collisionPoints = new List<Point>()
            {
                new Point(4,3),
                new Point(3,4)
            };

            foreach (Point point in nonCollisionPoints)
            {
                NUnit.Framework.Assert.True(CollisionDetection.TheInstance.Clear(currentObstacles, point));
            }

            foreach (Point point in collisionPoints)
            {
                NUnit.Framework.Assert.False(CollisionDetection.TheInstance.Clear(currentObstacles, point));
            }
        }
    }
}
