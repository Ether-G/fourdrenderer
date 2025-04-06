using System;
using System.Collections.Generic;
using FourDRenderer.Mathematics;
using FourDRenderer.Rendering;

namespace FourDRenderer.Objects
{
    public abstract class Object4D
    {
        public Vector4D Position { get; set; }
        public List<Vector4D> Vertices { get; protected set; }
        public List<Vector4D> OriginalVertices { get; protected set; }
        public List<Edge4D> Edges { get; protected set; }
        public Matrix4D Transformation { get; protected set; }
        public string Name { get; set; }
        
        public Vector4D Center { get; protected set; }

        protected Object4D()
        {
            Position = new Vector4D(0, 0, 0, 0);
            OriginalVertices = new List<Vector4D>();
            Vertices = new List<Vector4D>();
            Edges = new List<Edge4D>();
            Transformation = Matrix4D.CreateIdentity();
            Center = new Vector4D(0, 0, 0, 0);
            Name = GetType().Name;
        }

        // center of the object from its vertices
        protected void CalculateCenter()
        {
            // Reset center
            Center = new Vector4D(0, 0, 0, 0);
            
            // Early exit if no vertices
            if (Vertices.Count == 0) return;
            
            // Sum all vertex positions
            foreach (Vector4D v in Vertices)
            {
                Center.X += v.X;
                Center.Y += v.Y;
                Center.Z += v.Z;
                Center.W += v.W;
            }
            
            // Divide by count to get avg
            float count = Vertices.Count;
            Center.X /= count;
            Center.Y /= count;
            Center.Z /= count;
            Center.W /= count;
        }

        // Store a copy of current vertices as original vertices
        public void StoreOriginalVertices()
        {
            OriginalVertices.Clear();
            foreach (Vector4D vertex in Vertices)
            {
                OriginalVertices.Add(new Vector4D(vertex.X, vertex.Y, vertex.Z, vertex.W));
            }
            
            // Calculate center when storing original vertices
            CalculateCenter();
        }
        
        // Reset vertices to original positions
        public void ResetTransformation()
        {
            Transformation = Matrix4D.CreateIdentity();
            
            // Reset vertices to original positions
            Vertices.Clear();
            foreach (Vector4D originalVertex in OriginalVertices)
            {
                Vertices.Add(new Vector4D(originalVertex.X, originalVertex.Y, originalVertex.Z, originalVertex.W));
            }
        }

        public void ApplyTransformation(Matrix4D transform)
        {
            Transformation = transform;
            
            // Apply transformation to vertices around the object's center:
            // 1. Translate to origin
            // 2. Apply rotation
            // 3. Translate back to original position
            Vertices.Clear();
            
            foreach (Vector4D originalVertex in OriginalVertices)
            {
                // Translate to origin by subtracting center
                Vector4D centered = originalVertex.Subtract(Center);
                
                // Apply rotation transformation
                Vector4D rotated = transform.Transform(centered);
                
                // Translate back to original position by adding center
                Vector4D transformed = rotated.Add(Center);
                
                Vertices.Add(transformed);
            }
        }

        // Project all vertices to 3D space
        public List<Vector3D> ProjectTo3D(float viewerDistance)
        {
            List<Vector3D> projected3D = new List<Vector3D>();
            
            foreach (Vector4D vertex in Vertices)
            {
                Vector3D proj = vertex.ProjectTo3D(viewerDistance);
                projected3D.Add(proj);
            }
            
            return projected3D;
        }

        // Abstract method to generate the object's geometry
        public abstract void GenerateGeometry();

        // Update object state (for animations or movements)
        public virtual void Update(float deltaTime)
        {
            // Default implementation does nothing
            // Override in derived classes for custom behavior
        }

        // Render the object using the provided renderer
        public virtual void Render(Renderer renderer)
        {
            // Project vertices to 3D space
            List<Vector3D> projected3D = ProjectTo3D(renderer.Camera.ViewerDistance);
            
            // Project 3D vertices to 2D screen space
            List<Vector2D> projected2D = new List<Vector2D>();
            foreach (Vector3D vertex3D in projected3D)
            {
                Vector2D proj = renderer.Camera.ProjectTo2D(vertex3D);
                projected2D.Add(proj);
            }
            
            // Render all edges
            foreach (Edge4D edge in Edges)
            {
                edge.Render(renderer, projected2D);
            }
            
            // render the object's center point
            Vector3D center3D = Center.ProjectTo3D(renderer.Camera.ViewerDistance);
            Vector2D center2D = renderer.Camera.ProjectTo2D(center3D);
            renderer.DrawPoint(center2D, Color.Magenta, 5);
        }
    }
}