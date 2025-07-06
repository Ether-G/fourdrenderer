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
        
        // Camera rotation state
        public float[] CameraRotationAngles { get; private set; }
        public float CameraRotationSpeed { get; set; }
        public bool[] ActiveCameraRotations { get; private set; }
        
        // Animation state
        private bool _isAnimating = true;
        private DateTime _lastUpdateTime;
        
        // Added to fix acceleration issue
        private bool _resetEachFrame = true;  // Add this flag to reset objects each frame
        
        // Debug frame counter
        private int _debugFrameCount = 0;

        // Add a field to track camera animation state
        private bool _cameraAnimating = true;

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
            RotationSpeed = 1.0f; // Changed from 0.02f to 1.0f
            
            // Initialize camera rotation state
            CameraRotationAngles = new float[6]; // 6 rotation planes in 4D
            ActiveCameraRotations = new bool[6] { false, false, false, false, false, false };
            CameraRotationSpeed = 1.0f;
            
            _lastUpdateTime = DateTime.Now;
        }

        // Initialize the engine with default objects
        public void Initialize()
        {
            // Create a demo scene
            Scene = Scene4D.CreateDemoScene(Renderer.Camera);
            
            // List objects in the scene for debugging
            Console.WriteLine("Initialized engine with objects:");
            for (int i = 0; i < Scene.Objects.Count; i++)
            {
                Object4D obj = Scene.Objects[i];
                Console.WriteLine($"{i}: {obj.Name} with {obj.Vertices.Count} vertices");
            }
        }

        // Add a method to reset the simulation
        public void ResetSimulation()
        {
            // Reset all object transformations and rotations
            foreach (var obj in Scene.Objects)
            {
                obj.ResetTransformation();
            }
            for (int i = 0; i < RotationAngles.Length; i++) RotationAngles[i] = 0f;
            for (int i = 0; i < ActiveRotations.Length; i++) ActiveRotations[i] = false;
            RotationSpeed = 1.0f;
            
            // Reset camera orientation and distances
            Renderer.Camera.Orientation = Matrix4D.CreateIdentity();
            Renderer.Camera.Position = new Vector4D(0, 0, 0, -5.0f);
            Renderer.Camera.ViewerDistance4D = 5.0f;
            Renderer.Camera.ViewerDistance3D = 5.0f;
            for (int i = 0; i < CameraRotationAngles.Length; i++) CameraRotationAngles[i] = 0f;
            for (int i = 0; i < ActiveCameraRotations.Length; i++) ActiveCameraRotations[i] = false;
            CameraRotationSpeed = 1.0f;
            
            // Reset animation state
            _isAnimating = true;
            _cameraAnimating = true;
            _resetEachFrame = true;
        }

        // Update main update loop to use _cameraAnimating for camera rotation
        public void Update(float deltaTime)
        {
            // Update scene
            Scene.Update(deltaTime);
            
            // Apply rotations if animating
            if (_isAnimating)
            {
                RotateObjects(deltaTime);
            }
            // Apply camera rotations if animating
            if (_cameraAnimating)
            {
                RotateCamera(deltaTime);
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
            
            // Reset the current object if needed
            if (_resetEachFrame && Scene.SelectedObject != null)
            {
                Scene.SelectedObject.ResetTransformation();
            }
            
            // Apply the rotation to the selected object only
            Scene.ApplyRotation(rotation);
        }

        // Apply rotations to camera
        public void RotateCamera(float deltaTime = 1.0f)
        {
            // XY plane rotation
            if (ActiveCameraRotations[0])
            {
                CameraRotationAngles[0] += CameraRotationSpeed * deltaTime;
                Renderer.Camera.Rotate(0, CameraRotationSpeed * deltaTime);
            }
            
            // XZ plane rotation
            if (ActiveCameraRotations[1])
            {
                CameraRotationAngles[1] += CameraRotationSpeed * deltaTime;
                Renderer.Camera.Rotate(1, CameraRotationSpeed * deltaTime);
            }
            
            // XW plane rotation (4D specific)
            if (ActiveCameraRotations[2])
            {
                CameraRotationAngles[2] += CameraRotationSpeed * deltaTime;
                Renderer.Camera.Rotate(2, CameraRotationSpeed * deltaTime);
            }
            
            // YZ plane rotation
            if (ActiveCameraRotations[3])
            {
                CameraRotationAngles[3] += CameraRotationSpeed * deltaTime;
                Renderer.Camera.Rotate(3, CameraRotationSpeed * deltaTime);
            }
            
            // YW plane rotation (4D specific)
            if (ActiveCameraRotations[4])
            {
                CameraRotationAngles[4] += CameraRotationSpeed * deltaTime;
                Renderer.Camera.Rotate(4, CameraRotationSpeed * deltaTime);
            }
            
            // ZW plane rotation (4D specific)
            if (ActiveCameraRotations[5])
            {
                CameraRotationAngles[5] += CameraRotationSpeed * deltaTime;
                Renderer.Camera.Rotate(5, CameraRotationSpeed * deltaTime);
            }
        }

        // Toggle animation on/off
        public void ToggleAnimation()
        {
            _isAnimating = !_isAnimating;
            // Also pause/resume camera rotations
            _cameraAnimating = _isAnimating;
        }

        // Toggle specific rotation plane
        public void ToggleRotationPlane(int planeIndex)
        {
            if (planeIndex >= 0 && planeIndex < ActiveRotations.Length)
            {
                ActiveRotations[planeIndex] = !ActiveRotations[planeIndex];
            }
        }

        // Toggle specific camera rotation plane
        public void ToggleCameraRotationPlane(int planeIndex)
        {
            if (planeIndex >= 0 && planeIndex < ActiveCameraRotations.Length)
            {
                ActiveCameraRotations[planeIndex] = !ActiveCameraRotations[planeIndex];
            }
        }
        
        // Toggle reset each frame option (added to fix acceleration)
        public void ToggleResetEachFrame()
        {
            _resetEachFrame = !_resetEachFrame;
            
            // If turned on, reset all objects immediately
            if (_resetEachFrame && Scene.SelectedObject != null)
            {
                Scene.SelectedObject.ResetTransformation();
            }
        }

        // Process keyboard input
        public void ProcessInput(Keys key, bool isKeyDown)
        {
            if (isKeyDown)
            {
                if (key == Keys.D1) ToggleRotationPlane(0); // XY
                if (key == Keys.D2) ToggleRotationPlane(1); // XZ
                if (key == Keys.D3) ToggleRotationPlane(2); // XW
                if (key == Keys.D4) ToggleRotationPlane(3); // YZ
                if (key == Keys.D5) ToggleRotationPlane(4); // YW
                if (key == Keys.D6) ToggleRotationPlane(5); // ZW
                if (key == Keys.Space) ToggleAnimation();
                if (key == Keys.T) ToggleResetEachFrame();
                if (key == Keys.Tab)
                {
                    int currentIndex = Scene.SelectedObject != null 
                        ? Scene.Objects.IndexOf(Scene.SelectedObject) 
                        : -1;
                    int nextIndex = (currentIndex + 1) % Scene.Objects.Count;
                    Scene.SelectObject(nextIndex);
                    if (Scene.SelectedObject != null)
                    {
                        Scene.SelectedObject.ResetTransformation();
                    }
                }
                if (key == Keys.Up) RotationSpeed += 0.005f;
                if (key == Keys.Down) RotationSpeed = Math.Max(0.001f, RotationSpeed - 0.005f);
                // Remove camera movement controls (W/A/S/D/Q/E/R/F)
                // Camera rotation controls (4D space) - using I,J,K,L,U,O keys
                if (key == Keys.I) ToggleCameraRotationPlane(0); // XY rotation
                if (key == Keys.J) ToggleCameraRotationPlane(1); // XZ rotation
                if (key == Keys.K) ToggleCameraRotationPlane(2); // XW rotation
                if (key == Keys.L) ToggleCameraRotationPlane(3); // YZ rotation
                if (key == Keys.U) ToggleCameraRotationPlane(4); // YW rotation
                if (key == Keys.O) ToggleCameraRotationPlane(5); // ZW rotation
                if (key == Keys.Add || key == Keys.OemPlus) 
                {
                    Renderer.Camera.AdjustViewerDistance4D(0.2f);
                    Console.WriteLine($"4D Distance adjusted to: {Renderer.Camera.ViewerDistance4D:F2}");
                }
                if (key == Keys.Subtract || key == Keys.OemMinus) 
                {
                    Renderer.Camera.AdjustViewerDistance4D(-0.2f);
                    Console.WriteLine($"4D Distance adjusted to: {Renderer.Camera.ViewerDistance4D:F2}");
                }
                // Add Backspace to reset the simulation
                if (key == Keys.Backspace) ResetSimulation();
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
            // Key tags for object and camera rotations
            string[] objectKeys = { "1", "2", "3", "4", "5", "6" };
            string[] cameraKeys = { "I", "J", "K", "L", "U", "O" };

            string rotationInfo = "Object Rotations: ";
            for (int i = 0; i < ActiveRotations.Length; i++)
            {
                if (ActiveRotations[i])
                {
                    rotationInfo += $"{GetRotationPlaneName(i)}({objectKeys[i]}) ";
                }
            }
            
            string cameraRotationInfo = "Camera Rotations: ";
            for (int i = 0; i < ActiveCameraRotations.Length; i++)
            {
                if (ActiveCameraRotations[i])
                {
                    cameraRotationInfo += $"{GetRotationPlaneName(i)}({cameraKeys[i]}) ";
                }
            }
            
            Renderer.DrawText(rotationInfo, new Vector2D(10, 10), Color.Yellow);
            Renderer.DrawText(cameraRotationInfo, new Vector2D(10, 30), Color.Orange);
            Renderer.DrawText("Speed: " + RotationSpeed.ToString("F3"), new Vector2D(10, 50), Color.Yellow);
            
            string animationStatus = _isAnimating ? "Running" : "Paused";
            Renderer.DrawText("Animation: " + animationStatus, new Vector2D(10, 70), Color.Yellow);
            
            // Add reset mode status to debug info
            string resetMode = _resetEachFrame ? "Reset Each Frame" : "Cumulative Rotations";
            Renderer.DrawText("Mode: " + resetMode, new Vector2D(10, 90), Color.Yellow);
            
            // Add selected object info
            string selectedObjectInfo = Scene.SelectedObject != null 
                ? $"Selected: {Scene.SelectedObject.Name}" 
                : "No object selected";
            Renderer.DrawText(selectedObjectInfo, new Vector2D(10, 110), Color.Cyan);
            
            // Add camera info
            float current4DDistance = Renderer.Camera.ViewerDistance4D;
            float current3DDistance = Renderer.Camera.ViewerDistance3D;
            Renderer.DrawText($"Camera 4D Distance: {current4DDistance:F2}", new Vector2D(10, 130), Color.Green);
            Renderer.DrawText($"Camera 3D Distance: {current3DDistance:F2}", new Vector2D(10, 150), Color.Green);
            
            // Debug output to console every 60 frames (about once per second)
            _debugFrameCount++;
            if (_debugFrameCount % 60 == 0)
            {
                Console.WriteLine($"Debug - 4D Distance: {current4DDistance:F2}, 3D Distance: {current3DDistance:F2}");
            }
            
            Renderer.DrawDebugInfo(new Point(10, 170), Scene.SelectedObject);
            
            // Control information
            Renderer.DrawText("Controls: 1-6=Toggle Object Rotations, I/J/K/L/U/O=Toggle Camera Rotations", 
                new Vector2D(10, Renderer.Height - 80), Color.LightGray);
            Renderer.DrawText("Space=Pause, T=Toggle Reset, Tab=Switch Object, Backspace=Full Reset", 
                new Vector2D(10, Renderer.Height - 60), Color.LightGray);
            Renderer.DrawText("+/-=4D Distance, Up/Down=Speed", 
                new Vector2D(10, Renderer.Height - 40), Color.LightGray);
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
        W, A, S, D, Q, E, R, F, T, Tab,
        I, J, K, L, U, O, // Camera rotation keys
        Up, Down, Left, Right,
        Add, Subtract, OemPlus, OemMinus,
        Backspace
    }
}