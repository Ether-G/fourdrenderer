using System;
using FourDRenderer.Mathematics;

namespace FourDRenderer.Rendering
{
    public class Camera4D
    {
        public Vector4D Position { get; set; }
        public float ViewerDistance { get; set; }
        public float Screen3DDistance { get; set; }

        // Screen center coordinates
        public int ScreenCenterX { get; set; }
        public int ScreenCenterY { get; set; }

        // Scale factors for display
        public float ScaleX { get; set; }
        public float ScaleY { get; set; }

        public Camera4D(float viewerDistance = 5.0f, float screen3DDistance = 5.0f)
        {
            Position = new Vector4D(0, 0, 0, -5.0f); // Position the camera along the W axis
            ViewerDistance = viewerDistance;
            Screen3DDistance = screen3DDistance;
            ScreenCenterX = 0;
            ScreenCenterY = 0;
            ScaleX = 100.0f; // Default scaling
            ScaleY = 100.0f;
        }

        // Project a 4D point to 3D, accounting for camera position
        public Vector3D ProjectTo3D(Vector4D point)
        {
            // Translate point relative to camera position
            Vector4D relativePoint = point.Subtract(Position);
            
            // Project to 3D
            return relativePoint.ProjectTo3D(ViewerDistance);
        }

        // Project a 3D point to 2D screen space
        public Vector2D ProjectTo2D(Vector3D point)
        {
            // Project to 2D
            Vector2D projected = point.ProjectTo2D(Screen3DDistance);
            
            // Scale and translate to screen coordinates
            projected.X = ScreenCenterX + projected.X * ScaleX;
            projected.Y = ScreenCenterY - projected.Y * ScaleY; // Invert Y for screen coordinates
            
            return projected;
        }

        // Move camera in 4D space
        public void Move(Vector4D direction)
        {
            Position = Position.Add(direction);
        }

        // Set screen center and scale based on screen dimensions
        public void SetScreenParameters(int screenWidth, int screenHeight, float scale = 1.0f)
        {
            ScreenCenterX = screenWidth / 2;
            ScreenCenterY = screenHeight / 2;
            ScaleX = scale * Math.Min(screenWidth, screenHeight) / 4;
            ScaleY = scale * Math.Min(screenWidth, screenHeight) / 4;
        }

        // Adjust viewer distance (moving closer or farther from 4D objects)
        public void AdjustViewerDistance(float deltaDistance)
        {
            ViewerDistance = Math.Max(0.1f, ViewerDistance + deltaDistance);
        }

        // Adjust 3D to 2D projection distance
        public void AdjustScreen3DDistance(float deltaDistance)
        {
            Screen3DDistance = Math.Max(0.1f, Screen3DDistance + deltaDistance);
        }
    }
}