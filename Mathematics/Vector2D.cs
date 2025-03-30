using System;
using System.Drawing;

namespace FourDRenderer.Mathematics
{
    public class Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2D() : this(0, 0) { }

        public Vector2D Add(Vector2D other)
        {
            return new Vector2D(X + other.X, Y + other.Y);
        }

        public static Vector2D operator +(Vector2D a, Vector2D b)
        {
            return a.Add(b);
        }

        public Vector2D Subtract(Vector2D other)
        {
            return new Vector2D(X - other.X, Y - other.Y);
        }

        public static Vector2D operator -(Vector2D a, Vector2D b)
        {
            return a.Subtract(b);
        }

        public Vector2D Scale(float scalar)
        {
            return new Vector2D(X * scalar, Y * scalar);
        }

        public static Vector2D operator *(Vector2D v, float scalar)
        {
            return v.Scale(scalar);
        }

        public static Vector2D operator *(float scalar, Vector2D v)
        {
            return v.Scale(scalar);
        }

        public float DotProduct(Vector2D other)
        {
            return X * other.X + Y * other.Y;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public Vector2D Normalize()
        {
            float mag = Magnitude();
            if (mag < float.Epsilon)
                return new Vector2D(0, 0);
            return new Vector2D(X / mag, Y / mag);
        }

        public Point ToPoint()
        {
            return new Point((int)Math.Round(X), (int)Math.Round(Y));
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }
}