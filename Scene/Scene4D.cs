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
        public int SelectedIndex { get; private set; } = 0;

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
                SelectedIndex = 0;
            }
        }

        // Remove an object from the scene
        public void RemoveObject(Object4D obj)
        {
            int index = Objects.IndexOf(obj);
            Objects.Remove(obj);
            
            // Update selected object if needed
            if (SelectedObject == obj)
            {
                SelectedIndex = Objects.Count > 0 ? 0 : -1;
                SelectedObject = SelectedIndex >= 0 ? Objects[SelectedIndex] : null;
            }
            else if (index < SelectedIndex && SelectedIndex > 0)
            {
                // Adjust selected index if we removed an object before it
                SelectedIndex--;
            }
        }

        // Select an object by index
        public void SelectObject(int index)
        {
            if (index >= 0 && index < Objects.Count)
            {
                SelectedObject = Objects[index];
                SelectedIndex = index;
                Console.WriteLine($"Selected object: {SelectedObject.Name} (index {index})");
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

        // Apply a 4D rotation to the selected object
        public void ApplyRotation(Matrix4D rotation)
        {
            if (SelectedObject != null)
            {
                SelectedObject.ApplyTransformation(rotation);
            }
        }

        // Render only the selected object in the scene
        public void Render(Renderer renderer)
        {
            // Only render the selected object
            if (SelectedObject != null)
            {
                SelectedObject.Render(renderer);
            }
        }

        // Create a demo scene with basic 4D objects
        public static Scene4D CreateDemoScene(Camera4D? camera = null)
        {
            Scene4D scene = new Scene4D(camera ?? new Camera4D());
            
            // Add a tesseract
            Tesseract tesseract = new Tesseract(1.0f);
            scene.AddObject(tesseract);
            
            // Add a hypersphere with a different size
            Hypersphere hypersphere = new Hypersphere(1.2f, 12);
            scene.AddObject(hypersphere);
            
            // Add a 4D torus (toratope)
            Toratope toratope = new Toratope(1.5f, 0.5f, 12);
            scene.AddObject(toratope);
            
            // Initially select the tesseract
            scene.SelectedObject = tesseract;
            scene.SelectedIndex = 0;
            
            Console.WriteLine($"Created scene with {scene.Objects.Count} objects:");
            foreach (Object4D obj in scene.Objects)
            {
                Console.WriteLine($"- {obj.Name} with {obj.Vertices.Count} vertices and {obj.Edges.Count} edges");
            }
            
            return scene;
        }
    }
}