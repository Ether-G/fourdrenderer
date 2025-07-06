using System;
using FourDRenderer.Mathematics;

namespace FourDRenderer.Rendering
{
    public class Camera4D
    {
        // Camera position in 4D space
        private Vector4D _position;
        
        // Camera orientation matrix for 4D rotations
        private Matrix4D _orientation;
        
        // Projection distances
        public float ViewerDistance4D { get; set; }  // 4D to 3D projection distance
        public float ViewerDistance3D { get; set; }  // 3D to 2D projection distance

        // Screen center coordinates
        public int ScreenCenterX { get; set; }
        public int ScreenCenterY { get; set; }

        // Scale factors for display
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        // Public property for position
        public Vector4D Position 
        { 
            get => _position; 
            set => _position = value; 
        }

        // Public property for orientation
        public Matrix4D Orientation 
        { 
            get => _orientation; 
            set => _orientation = value; 
        }

        // Legacy property for backward compatibility
        public float ViewerDistance 
        { 
            get => ViewerDistance4D; 
            set => ViewerDistance4D = value; 
        }

        // Legacy property for backward compatibility
        public float Screen3DDistance 
        { 
            get => ViewerDistance3D; 
            set => ViewerDistance3D = value; 
        }

        public Camera4D(float viewerDistance4D = 5.0f, float viewerDistance3D = 5.0f)
        {
            _position = new Vector4D(0, 0, 0, -5.0f); // Position the camera along the W axis
            _orientation = Matrix4D.CreateIdentity(); // Start with identity orientation
            ViewerDistance4D = viewerDistance4D;
            ViewerDistance3D = viewerDistance3D;
            ScreenCenterX = 0;
            ScreenCenterY = 0;
            ScaleX = 100.0f; // Default scaling
            ScaleY = 100.0f;
        }

        // Get the camera's view matrix (inverse of camera transformation)
        public Matrix4D GetViewMatrix()
        {
            // For a view matrix, need to:
            // Translate the world so the camera is at origin
            // Rotate the world so the camera's orientation becomes identity
            
            // Create translation matrix to move camera to origin
            Matrix4D translationMatrix = Matrix4D.CreateTranslation(_position.Scale(-1));
            
            // Get the inverse of the orientation matrix (transpose for rotation matrices)
            Matrix4D orientationInverse = _orientation.Transpose();
            
            // Combine: first translate, then rotate
            return orientationInverse * translationMatrix;
        }

        // Project a 4D world point to 3D camera space
        public Vector3D Project4DToCamera3DSpace(Vector4D worldPoint)
        {
            // Transform the world point by the camera's view matrix
            Vector4D transformedPoint = GetViewMatrix().Transform(worldPoint);
            
            // Implement 4D to 3D perspective projection
            // Project onto the W=0 plane in camera space
            if (transformedPoint.W + ViewerDistance4D < float.Epsilon)
                return new Vector3D(0, 0, 0); // Prevent division by zero or negative values

            float factor = ViewerDistance4D / (ViewerDistance4D + transformedPoint.W);
            return new Vector3D(
                transformedPoint.X * factor, 
                transformedPoint.Y * factor, 
                transformedPoint.Z * factor
            );
        }

        // Project a 3D point to 2D screen space
        public Vector2D Project3DToScreen2D(Vector3D point)
        {
            // Project to 2D
            Vector2D projected = point.ProjectTo2D(ViewerDistance3D);
            
            // Scale and translate to screen coordinates
            projected.X = ScreenCenterX + projected.X * ScaleX;
            projected.Y = ScreenCenterY - projected.Y * ScaleY; // Invert Y for screen coordinates
            
            return projected;
        }

        // Legacy method for backward compatibility
        public Vector3D ProjectTo3D(Vector4D point)
        {
            return Project4DToCamera3DSpace(point);
        }

        // Legacy method for backward compatibility
        public Vector2D ProjectTo2D(Vector3D point)
        {
            return Project3DToScreen2D(point);
        }

        // Rotate the camera in 4D space
        public void Rotate(int planeIndex, float angle)
        {
            Matrix4D rotationMatrix = GetRotationMatrix(planeIndex, angle);
            _orientation = _orientation * rotationMatrix;
        }

        // Get rotation matrix for specific 4D plane
        private Matrix4D GetRotationMatrix(int planeIndex, float angle)
        {
            switch (planeIndex)
            {
                case 0: return Matrix4D.CreateRotationXY(angle);
                case 1: return Matrix4D.CreateRotationXZ(angle);
                case 2: return Matrix4D.CreateRotationXW(angle);
                case 3: return Matrix4D.CreateRotationYZ(angle);
                case 4: return Matrix4D.CreateRotationYW(angle);
                case 5: return Matrix4D.CreateRotationZW(angle);
                default: return Matrix4D.CreateIdentity();
            }
        }

        // Move camera in 4D space
        public void Move(Vector4D direction)
        {
            _position = _position.Add(direction);
        }

        // Set screen center and scale based on screen dimensions
        public void SetScreenParameters(int screenWidth, int screenHeight, float scale = 1.0f)
        {
            ScreenCenterX = screenWidth / 2;
            ScreenCenterY = screenHeight / 2;
            ScaleX = scale * Math.Min(screenWidth, screenHeight) / 4;
            ScaleY = scale * Math.Min(screenWidth, screenHeight) / 4;
        }

        // Adjust 4D viewer distance (moving closer or farther from 4D objects)
        public void AdjustViewerDistance4D(float deltaDistance)
        {
            ViewerDistance4D = Math.Max(0.1f, ViewerDistance4D + deltaDistance);
        }

        // Adjust 3D to 2D projection distance
        public void AdjustViewerDistance3D(float deltaDistance)
        {
            ViewerDistance3D = Math.Max(0.1f, ViewerDistance3D + deltaDistance);
        }

        // Legacy method for backward compatibility
        public void AdjustViewerDistance(float deltaDistance)
        {
            AdjustViewerDistance4D(deltaDistance);
        }

        // Legacy method for backward compatibility
        public void AdjustScreen3DDistance(float deltaDistance)
        {
            AdjustViewerDistance3D(deltaDistance);
        }
    }
}