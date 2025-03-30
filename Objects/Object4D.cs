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
        public List<Vector4D> OriginalVertices { get; protected set; } // Store original vertices
        public List<Edge4D> Edges { get; protected set; }
        public Matrix4D Transformation { get; protected set; }
        public string Name { get; set; }

        protected Object4D()
        {
            Position = new Vector4D(0, 0, 0, 0);
            OriginalVertices = new List<Vector4D>(); // Initialize original vertices list
            Vertices = new List<Vector4D>();
            Edges = new List<Edge4D>();
            Transformation = Matrix4D.CreateIdentity();
            Name = GetType().Name;
        }

        // Store a copy of current vertices as original vertices
        protected void StoreOriginalVertices()
        {
            OriginalVertices.Clear();
            foreach (Vector4D vertex in Vertices)
            {
                OriginalVertices.Add(new Vector4D(vertex.X, vertex.Y, vertex.Z, vertex.W));
            }
        }
        
        // Reset vertices to original positions
        public void ResetTransformation()
        {
            Transformation = Matrix4D.CreateIdentity();
            
            // reset vertices to original positions
            Vertices.Clear();
            foreach (Vector4D originalVertex in OriginalVertices)
            {
                Vertices.Add(new Vector4D(originalVertex.X, originalVertex.Y, originalVertex.Z, originalVertex.W));
            }
        }

        public void ApplyTransformation(Matrix4D transform)
        {
            Transformation = transform;
            
            // apply transformation to vertices based on original positions
            Vertices.Clear();
            
            foreach (Vector4D originalVertex in OriginalVertices)
            {
                Vector4D transformedVertex = transform.Transform(originalVertex);
                Vertices.Add(transformedVertex);
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
        }

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
        }
    }
}