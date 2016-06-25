using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class Vector2D : Point
    {
        #region Constructors
        public Vector2D() : this(0f, 0f) { }
        public Vector2D(float x, float y) : base(x, y) { }
        public Vector2D(double x, double y) : base(x, y) { }
        #endregion

        #region Properties
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(LengthSq);
            }
            set
            {
                Vector2D normalizeVector = Normalize();
                X = (float)(normalizeVector.X * value);
                Y = (float)(normalizeVector.Y * value);
            }
        }

        public float LengthSq
        {
            get
            {
                return X * X + Y * Y;
            }
        }
        #endregion

        #region Public Methods

        public static Vector2D GetVector(Point p1, Point p2)
        {
            if(p1 == null || p2 == null)
                return new Vector2D(0, 0);

            return new Vector2D(p2.X - p1.X, p2.Y - p1.Y);
        }

        public Vector2D Normalize()
        {
            Vector2D normalizedVector;
            double vectorLength = Length;

            if (vectorLength != 0)
            {
                normalizedVector = new Vector2D(X / vectorLength, Y / vectorLength);
            }
            else
            {
                normalizedVector = new Vector2D();
            }

            return normalizedVector;
        }
        #endregion
    }
}
