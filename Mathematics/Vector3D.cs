using System;

namespace FourDRenderer.Mathematics
{
    public class Vector3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3D() : this(0, 0, 0) { }

        public Vector3D Add(Vector3D other)
        {
            return new Vector3D(X + other.X, Y + other.Y, Z + other.Z);
        }

        public static Vector3D operator +(Vector3D a, Vector3D b)
        {
            return a.Add(b);
        }

        public Vector3D Subtract(Vector3D other)
        {
            return new Vector3D(X - other.X, Y - other.Y, Z - other.Z);
        }

        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return a.Subtract(b);
        }

        public Vector3D Scale(float scalar)
        {
            return new Vector3D(X * scalar, Y * scalar, Z * scalar);
        }

        public static Vector3D operator *(Vector3D v, float scalar)
        {
            return v.Scale(scalar);
        }

        public static Vector3D operator *(float scalar, Vector3D v)
        {
            return v.Scale(scalar);
        }

        public float DotProduct(Vector3D other)
        {
            return X * other.X + Y * other.Y + Z * other.Z;
        }

        public Vector3D CrossProduct(Vector3D other)
        {
            return new Vector3D(
                Y * other.Z - Z * other.Y,
                Z * other.X - X * other.Z,
                X * other.Y - Y * other.X
            );
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public Vector3D Normalize()
        {
            float mag = Magnitude();
            if (mag < float.Epsilon)
                return new Vector3D(0, 0, 0);
            return new Vector3D(X / mag, Y / mag, Z / mag);
        }

        public Vector2D ProjectTo2D(float viewerDistance)
        {
            // Simple perspective projection
            if (Z + viewerDistance < float.Epsilon)
                return new Vector2D(0, 0); // Prevent division by zero or negative values

            float factor = viewerDistance / (Z + viewerDistance);
            return new Vector2D(X * factor, Y * factor);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
}