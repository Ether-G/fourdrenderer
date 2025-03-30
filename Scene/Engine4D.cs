using System;
using System.Drawing;
using FourDRenderer.Mathematics;
using FourDRenderer.Objects;
using FourDRenderer.Rendering;

namespace FourDRenderer.Scene
{
    public class Engine4D
    {
        public Scene4D Scene { get; private set; }
        public Renderer Renderer { get; private set; }
        
        // Rotation state
        public float[] RotationAngles { get; private set; }
        public float RotationSpeed { get; set; }
        public bool[] ActiveRotations { get; private set; }
        
        // Animation state
        private bool _isAnimating = true;
        private DateTime _lastUpdateTime;
        
        // Added to fix acceleration issue
        private bool _resetEachFrame = true;  // flag to reset objects each frame

        public Engine4D(int width, int height)
        {
            // Create renderer
            Renderer = new Renderer(width, height);
            
            // Create camera and scene
            Camera4D camera = new Camera4D();
            camera.SetScreenParameters(width, height);
            Scene = new Scene4D(camera);
            
            // Initialize rotation state
            RotationAngles = new float[6]; // 6 rotation planes in 4D
            ActiveRotations = new bool[6] { true, false, false, false, false, false };
            RotationSpeed = 0.02f;
            
            _lastUpdateTime = DateTime.Now;
        }

        // Initialize the engine with default objects
        public void Initialize()
        {
            // Create a demo scene
            Scene = Scene4D.CreateDemoScene(Renderer.Camera);
        }

        // Main update loop
        public void Update(float deltaTime)
        {
            // Update scene
            Scene.Update(deltaTime);
            
            // Apply rotations if animating
            if (_isAnimating)
            {
                RotateObjects(deltaTime);
            }
        }

        // Apply rotations to objects
        public void RotateObjects(float deltaTime = 1.0f)
        {
            // Create combined rotation matrix
            Matrix4D rotation = Matrix4D.CreateIdentity();
            
            // XY plane rotation (traditional 2D rotation)
            if (ActiveRotations[0])
            {
                RotationAngles[0] += RotationSpeed * deltaTime;
                rotation = rotation * Matrix4D.CreateRotationXY(RotationAngles[0]);
            }
            
            // XZ plane rotation
            if (ActiveRotations[1])
            {
                RotationAngles[1] += RotationSpeed * deltaTime;
                rotation = rotation * Matrix4D.CreateRotationXZ(RotationAngles[1]);
            }
            
            // XW plane rotation (4D specific)
            if (ActiveRotations[2])
            {
                RotationAngles[2] += RotationSpeed * deltaTime;
                rotation = rotation * Matrix4D.CreateRotationXW(RotationAngles[2]);
            }
            
            // YZ plane rotation
            if (ActiveRotations[3])
            {
                RotationAngles[3] += RotationSpeed * deltaTime;
                rotation = rotation * Matrix4D.CreateRotationYZ(RotationAngles[3]);
            }
            
            // YW plane rotation (4D specific)
            if (ActiveRotations[4])
            {
                RotationAngles[4] += RotationSpeed * deltaTime;
                rotation = rotation * Matrix4D.CreateRotationYW(RotationAngles[4]);
            }
            
            // ZW plane rotation (4D specific)
            if (ActiveRotations[5])
            {
                RotationAngles[5] += RotationSpeed * deltaTime;
                rotation = rotation * Matrix4D.CreateRotationZW(RotationAngles[5]);
            }
            
            // Apply rotation to selected object or all objects
            if (Scene.SelectedObject != null)
            {
                foreach (Object4D obj in Scene.Objects)
                {
                    // Reset to original position before applying new rotation
                    if (_resetEachFrame)
                    {
                        obj.ResetTransformation();
                    }
                    obj.ApplyTransformation(rotation);
                }
            }
        }

        // Toggle animation on/off
        public void ToggleAnimation()
        {
            _isAnimating = !_isAnimating;
        }

        // Toggle specific rotation plane
        public void ToggleRotationPlane(int planeIndex)
        {
            if (planeIndex >= 0 && planeIndex < ActiveRotations.Length)
            {
                ActiveRotations[planeIndex] = !ActiveRotations[planeIndex];
            }
        }
        
        // Toggle reset each frame option (added to fix acceleration)
        public void ToggleResetEachFrame()
        {
            _resetEachFrame = !_resetEachFrame;
            
            // If turned on, reset all objects immediately
            if (_resetEachFrame)
            {
                foreach (Object4D obj in Scene.Objects)
                {
                    obj.ResetTransformation();
                }
            }
        }

        // Process keyboard input
        public void ProcessInput(Keys key, bool isKeyDown)
        {
            // Toggle rotation planes
            if (isKeyDown)
            {
                if (key == Keys.D1) ToggleRotationPlane(0); // XY
                if (key == Keys.D2) ToggleRotationPlane(1); // XZ
                if (key == Keys.D3) ToggleRotationPlane(2); // XW
                if (key == Keys.D4) ToggleRotationPlane(3); // YZ
                if (key == Keys.D5) ToggleRotationPlane(4); // YW
                if (key == Keys.D6) ToggleRotationPlane(5); // ZW
                
                // Toggle animation
                if (key == Keys.Space) ToggleAnimation();
                
                // Toggle reset each frame
                if (key == Keys.T) ToggleResetEachFrame();
                
                // Adjust speed
                if (key == Keys.Up) RotationSpeed += 0.005f;
                if (key == Keys.Down) RotationSpeed = Math.Max(0.001f, RotationSpeed - 0.005f);
                
                // Camera controls
                float step = 0.1f;
                if (key == Keys.W) Renderer.Camera.Position.Y += step;
                if (key == Keys.S) Renderer.Camera.Position.Y -= step;
                if (key == Keys.A) Renderer.Camera.Position.X -= step;
                if (key == Keys.D) Renderer.Camera.Position.X += step;
                if (key == Keys.Q) Renderer.Camera.Position.Z += step;
                if (key == Keys.E) Renderer.Camera.Position.Z -= step;
                if (key == Keys.R) Renderer.Camera.Position.W += step;
                if (key == Keys.F) Renderer.Camera.Position.W -= step;
                
                // Adjust projection parameters
                if (key == Keys.Add || key == Keys.OemPlus) Renderer.Camera.AdjustViewerDistance(0.2f);
                if (key == Keys.Subtract || key == Keys.OemMinus) Renderer.Camera.AdjustViewerDistance(-0.2f);
            }
        }

        // Main render loop
        public void Render()
        {
            // Calculate delta time
            DateTime now = DateTime.Now;
            float deltaTime = (float)(now - _lastUpdateTime).TotalSeconds;
            _lastUpdateTime = now;

            // Update state
            Update(deltaTime);
            
            // Clear the screen
            Renderer.Clear();
            
            // Render the scene
            Scene.Render(Renderer);
            
            // Draw debug information
            DrawDebugInfo();
        }

        // Render debug information
        private void DrawDebugInfo()
        {
            string rotationInfo = "Active Rotations: ";
            for (int i = 0; i < ActiveRotations.Length; i++)
            {
                if (ActiveRotations[i])
                {
                    rotationInfo += GetRotationPlaneName(i) + " ";
                }
            }
            
            Renderer.DrawText(rotationInfo, new Vector2D(10, 10), Color.Yellow);
            Renderer.DrawText("Speed: " + RotationSpeed.ToString("F3"), new Vector2D(10, 30), Color.Yellow);
            
            string animationStatus = _isAnimating ? "Running" : "Paused";
            Renderer.DrawText("Animation: " + animationStatus, new Vector2D(10, 50), Color.Yellow);
            
            // Add reset mode status to debug info
            string resetMode = _resetEachFrame ? "Reset Each Frame" : "Cumulative Rotations";
            Renderer.DrawText("Mode: " + resetMode, new Vector2D(10, 70), Color.Yellow);
            
            Renderer.DrawDebugInfo(new Point(10, 90), Scene.SelectedObject);
            
            // Control information
            Renderer.DrawText("Controls: 1-6=Toggle Rotations, Space=Pause, T=Toggle Reset Mode", 
                new Vector2D(10, Renderer.Height - 40), Color.LightGray);
            Renderer.DrawText("W/S/A/D/Q/E/R/F=Move Camera, +/-=Viewer Distance, Up/Down=Speed", 
                new Vector2D(10, Renderer.Height - 20), Color.LightGray);
        }

        // Helper to get rotation plane name
        private string GetRotationPlaneName(int index)
        {
            switch (index)
            {
                case 0: return "XY";
                case 1: return "XZ";
                case 2: return "XW";
                case 3: return "YZ";
                case 4: return "YW";
                case 5: return "ZW";
                default: return "?";
            }
        }

        // Resize the rendering surface
        public void Resize(int width, int height)
        {
            Renderer.Resize(width, height);
        }

        // Clean up resources
        public void Dispose()
        {
            Renderer.Dispose();
        }
    }

    // Basic keys enum for input handling
    public enum Keys
    {
        Space, 
        D1, D2, D3, D4, D5, D6,
        W, A, S, D, Q, E, R, F, T, // Added T key
        Up, Down, Left, Right,
        Add, Subtract, OemPlus, OemMinus
    }
}