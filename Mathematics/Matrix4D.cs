using System;

namespace FourDRenderer.Mathematics
{
    public class Matrix4D
    {
        // 5x5 matrix for 4D transformations (4D space + homogeneous coordinate)
        private float[,] _matrix;

        public Matrix4D()
        {
            _matrix = new float[5, 5];
            SetIdentity();
        }

        public Matrix4D(float[,] matrix)
        {
            if (matrix.GetLength(0) != 5 || matrix.GetLength(1) != 5)
                throw new ArgumentException("Matrix must be 5x5");

            _matrix = (float[,])matrix.Clone();
        }

        // Access to the internal matrix
        public float this[int row, int col]
        {
            get { return _matrix[row, col]; }
            set { _matrix[row, col] = value; }
        }

        // Set this matrix to identity
        public void SetIdentity()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    _matrix[i, j] = (i == j) ? 1 : 0;
                }
            }
        }

        // Create a new identity matrix
        public static Matrix4D CreateIdentity()
        {
            return new Matrix4D();
        }

        // Matrix multiplication
        public Matrix4D Multiply(Matrix4D other)
        {
            Matrix4D result = new Matrix4D();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < 5; k++)
                    {
                        sum += _matrix[i, k] * other[k, j];
                    }
                    result[i, j] = sum;
                }
            }

            return result;
        }

        public static Matrix4D operator *(Matrix4D a, Matrix4D b)
        {
            return a.Multiply(b);
        }

        // Transform a 4D vector by this matrix
        public Vector4D Transform(Vector4D vector)
        {
            float x = vector.X * _matrix[0, 0] + vector.Y * _matrix[0, 1] + vector.Z * _matrix[0, 2] + vector.W * _matrix[0, 3] + _matrix[0, 4];
            float y = vector.X * _matrix[1, 0] + vector.Y * _matrix[1, 1] + vector.Z * _matrix[1, 2] + vector.W * _matrix[1, 3] + _matrix[1, 4];
            float z = vector.X * _matrix[2, 0] + vector.Y * _matrix[2, 1] + vector.Z * _matrix[2, 2] + vector.W * _matrix[2, 3] + _matrix[2, 4];
            float w = vector.X * _matrix[3, 0] + vector.Y * _matrix[3, 1] + vector.Z * _matrix[3, 2] + vector.W * _matrix[3, 3] + _matrix[3, 4];
            
            return new Vector4D(x, y, z, w);
        }

        // Create rotation matrix for rotation in XY plane
        public static Matrix4D CreateRotationXY(float angle)
        {
            Matrix4D matrix = CreateIdentity();
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            
            matrix[0, 0] = cos;
            matrix[0, 1] = -sin;
            matrix[1, 0] = sin;
            matrix[1, 1] = cos;
            
            return matrix;
        }

        // Create rotation matrix for rotation in XZ plane
        public static Matrix4D CreateRotationXZ(float angle)
        {
            Matrix4D matrix = CreateIdentity();
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            
            matrix[0, 0] = cos;
            matrix[0, 2] = -sin;
            matrix[2, 0] = sin;
            matrix[2, 2] = cos;
            
            return matrix;
        }

        // Create rotation matrix for rotation in XW plane
        public static Matrix4D CreateRotationXW(float angle)
        {
            Matrix4D matrix = CreateIdentity();
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            
            matrix[0, 0] = cos;
            matrix[0, 3] = -sin;
            matrix[3, 0] = sin;
            matrix[3, 3] = cos;
            
            return matrix;
        }

        // Create rotation matrix for rotation in YZ plane
        public static Matrix4D CreateRotationYZ(float angle)
        {
            Matrix4D matrix = CreateIdentity();
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            
            matrix[1, 1] = cos;
            matrix[1, 2] = -sin;
            matrix[2, 1] = sin;
            matrix[2, 2] = cos;
            
            return matrix;
        }

        // Create rotation matrix for rotation in YW plane
        public static Matrix4D CreateRotationYW(float angle)
        {
            Matrix4D matrix = CreateIdentity();
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            
            matrix[1, 1] = cos;
            matrix[1, 3] = -sin;
            matrix[3, 1] = sin;
            matrix[3, 3] = cos;
            
            return matrix;
        }

        // Create rotation matrix for rotation in ZW plane
        public static Matrix4D CreateRotationZW(float angle)
        {
            Matrix4D matrix = CreateIdentity();
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            
            matrix[2, 2] = cos;
            matrix[2, 3] = -sin;
            matrix[3, 2] = sin;
            matrix[3, 3] = cos;
            
            return matrix;
        }

        // Create translation matrix
        public static Matrix4D CreateTranslation(Vector4D translation)
        {
            Matrix4D matrix = CreateIdentity();
            
            matrix[0, 4] = translation.X;
            matrix[1, 4] = translation.Y;
            matrix[2, 4] = translation.Z;
            matrix[3, 4] = translation.W;
            
            return matrix;
        }

        // Create scaling matrix
        public static Matrix4D CreateScaling(float scaleX, float scaleY, float scaleZ, float scaleW)
        {
            Matrix4D matrix = CreateIdentity();
            
            matrix[0, 0] = scaleX;
            matrix[1, 1] = scaleY;
            matrix[2, 2] = scaleZ;
            matrix[3, 3] = scaleW;
            
            return matrix;
        }
    }
}