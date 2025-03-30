using System;
using System.Collections.Generic;
using FourDRenderer.Mathematics;
using FourDRenderer.Objects;
using FourDRenderer.Rendering;

namespace FourDRenderer.Scene
{
    public class Scene4D
    {
        public List<Object4D> Objects { get; private set; }
        public Camera4D Camera { get; set; }
        public Object4D? SelectedObject { get; set; } // Made nullable

        public Scene4D()
        {
            Objects = new List<Object4D>();
            Camera = new Camera4D();
        }

        public Scene4D(Camera4D camera)
        {
            Objects = new List<Object4D>();
            Camera = camera;
        }

        // Add an object to the scene
        public void AddObject(Object4D obj)
        {
            Objects.Add(obj);
            
            // Auto-select the first object if none is selected
            if (SelectedObject == null)
            {
                SelectedObject = obj;
            }
        }

        // Remove an object from the scene
        public void RemoveObject(Object4D obj)
        {
            Objects.Remove(obj);
            
            // Update selected object if needed
            if (SelectedObject == obj)
            {
                SelectedObject = Objects.Count > 0 ? Objects[0] : null;
            }
        }

        // Select an object by index
        public void SelectObject(int index)
        {
            if (index >= 0 && index < Objects.Count)
            {
                SelectedObject = Objects[index];
            }
        }

        // Update all objects in the scene
        public void Update(float deltaTime)
        {
            foreach (Object4D obj in Objects)
            {
                obj.Update(deltaTime);
            }
        }

        // Apply a 4D rotation to the selected object or all objects
        public void ApplyRotation(Matrix4D rotation, bool applyToAll = false)
        {
            if (applyToAll)
            {
                foreach (Object4D obj in Objects)
                {
                    obj.ApplyTransformation(rotation);
                }
            }
            else if (SelectedObject != null)
            {
                SelectedObject.ApplyTransformation(rotation);
            }
        }

        // Render all objects in the scene
        public void Render(Renderer renderer)
        {
            foreach (Object4D obj in Objects)
            {
                renderer.RenderObject(obj);
            }
        }

        // Create a demo scene with basic 4D objects
        public static Scene4D CreateDemoScene(Camera4D? camera = null)
        {
            Scene4D scene = new Scene4D(camera ?? new Camera4D());
            
            // Add a tesseract
            Tesseract tesseract = new Tesseract(1.0f);
            scene.AddObject(tesseract);
            
            // Add a hypersphere
            Hypersphere hypersphere = new Hypersphere(0.7f, 8);
            // Position it off to the side
            foreach (Vector4D vertex in hypersphere.Vertices)
            {
                vertex.X += 2.0f;
            }
            scene.AddObject(hypersphere);
            
            return scene;
        }
    }
}