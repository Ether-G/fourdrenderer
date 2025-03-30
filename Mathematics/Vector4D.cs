using System;

namespace FourDRenderer.Mathematics
{
    public class Vector4D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector4D(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector4D() : this(0, 0, 0, 0) { }

        public Vector4D Add(Vector4D other)
        {
            return new Vector4D(X + other.X, Y + other.Y, Z + other.Z, W + other.W);
        }

        public static Vector4D operator +(Vector4D a, Vector4D b)
        {
            return a.Add(b);
        }

        public Vector4D Subtract(Vector4D other)
        {
            return new Vector4D(X - other.X, Y - other.Y, Z - other.Z, W - other.W);
        }

        public static Vector4D operator -(Vector4D a, Vector4D b)
        {
            return a.Subtract(b);
        }

        public Vector4D Scale(float scalar)
        {
            return new Vector4D(X * scalar, Y * scalar, Z * scalar, W * scalar);
        }

        public static Vector4D operator *(Vector4D v, float scalar)
        {
            return v.Scale(scalar);
        }

        public static Vector4D operator *(float scalar, Vector4D v)
        {
            return v.Scale(scalar);
        }

        public float DotProduct(Vector4D other)
        {
            return X * other.X + Y * other.Y + Z * other.Z + W * other.W;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
        }

        public Vector4D Normalize()
        {
            float mag = Magnitude();
            if (mag < float.Epsilon)
                return new Vector4D(0, 0, 0, 0);
            return new Vector4D(X / mag, Y / mag, Z / mag, W / mag);
        }

        public Vector3D ProjectTo3D(float viewerDistance)
        {
            // Simple perspective projection from 4D to 3D
            if (W + viewerDistance < float.Epsilon)
                return new Vector3D(0, 0, 0); // Prevent division by zero or negative values

            float factor = viewerDistance / (W + viewerDistance);
            return new Vector3D(X * factor, Y * factor, Z * factor);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}