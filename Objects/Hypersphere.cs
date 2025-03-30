using System;
using System.Collections.Generic;
using System.Drawing;
using FourDRenderer.Mathematics;
using FourDRenderer.Rendering;

namespace FourDRenderer.Objects
{
    public class Hypersphere : Object4D
    {
        public float Radius { get; private set; }
        public int Resolution { get; private set; }

        public Hypersphere(float radius = 1.0f, int resolution = 12)
        {
            Radius = radius;
            Resolution = Math.Max(4, resolution); // Ensure minimum resolution
            Name = "Hypersphere";
            GenerateGeometry();
            StoreOriginalVertices(); // Store original vertices after generation
        }

        public override void GenerateGeometry()
        {
            Vertices.Clear();
            Edges.Clear();

            // Generate vertices on a 4D hypersphere
            // simplified approach with 4D spherical coordinates
            
            // Generate vertices using tesseract vertices projected onto a sphere
            // This approach uses the fact that the corners of a tesseract form a good distribution
            // when normalized to the sphere surface
            
            // create a tesseract
            Tesseract tesseract = new Tesseract(2.0f);
            
            // project its vertices onto the hypersphere
            foreach (Vector4D vertex in tesseract.Vertices)
            {
                Vector4D normalized = vertex.Normalize();
                Vector4D sphereVertex = normalized.Scale(Radius);
                Vertices.Add(sphereVertex);
            }

            // Add more points using 4D spherical coordinates
            AddSphericalPoints();
            
            // Create edges using nearest neighbor connections
            CreateEdges();
        }

        private void AddSphericalPoints()
        {
            // Add more points using a combination of angles
            
            int angularSteps = Resolution / 2;
            
            for (int i = 0; i < angularSteps; i++)
            {
                double theta1 = i * Math.PI / angularSteps;
                double sin1 = Math.Sin(theta1);
                double cos1 = Math.Cos(theta1);
                
                for (int j = 0; j < angularSteps; j++)
                {
                    double theta2 = j * Math.PI / angularSteps;
                    double sin2 = Math.Sin(theta2);
                    double cos2 = Math.Cos(theta2);
                    
                    for (int k = 0; k < angularSteps; k++)
                    {
                        double theta3 = k * 2 * Math.PI / angularSteps;
                        double sin3 = Math.Sin(theta3);
                        double cos3 = Math.Cos(theta3);
                        
                        // calc 4D point on hypersphere
                        float x = (float)(Radius * sin1 * sin2 * cos3);
                        float y = (float)(Radius * sin1 * sin2 * sin3);
                        float z = (float)(Radius * sin1 * cos2);
                        float w = (float)(Radius * cos1);
                        
                        // Add the point if it's not too close to an existing point
                        Vector4D newPoint = new Vector4D(x, y, z, w);
                        
                        if (!HasNearbyPoint(newPoint, 0.2f * Radius))
                        {
                            Vertices.Add(newPoint);
                        }
                    }
                }
            }
        }

        private bool HasNearbyPoint(Vector4D point, float threshold)
        {
            foreach (Vector4D vertex in Vertices)
            {
                if (vertex.Subtract(point).Magnitude() < threshold)
                {
                    return true;
                }
            }
            return false;
        }

        private void CreateEdges()
        {
            // Connect nearest neighbors
            // This approach creates an approximation of the hypersphere's "wireframe"
            
            // Define maximum connection distance based on radius
            float connectionDistance = Radius * 0.5f;
            
            for (int i = 0; i < Vertices.Count; i++)
            {
                // Find closest vertices to connect
                List<int> nearestIndices = new List<int>();
                
                for (int j = 0; j < Vertices.Count; j++)
                {
                    if (i != j)
                    {
                        float distance = Vertices[i].Subtract(Vertices[j]).Magnitude();
                        if (distance < connectionDistance)
                        {
                            nearestIndices.Add(j);
                        }
                    }
                }
                
                // Sort by distance if there are too many connections
                if (nearestIndices.Count > 4)
                {
                    nearestIndices.Sort((a, b) =>
                    {
                        float distA = Vertices[i].Subtract(Vertices[a]).Magnitude();
                        float distB = Vertices[i].Subtract(Vertices[b]).Magnitude();
                        return distA.CompareTo(distB);
                    });
                    
                    // Keep only the closest 4 connections
                    nearestIndices = nearestIndices.GetRange(0, 4);
                }
                
                // Create edges to nearest vertices
                foreach (int j in nearestIndices)
                {
                    // Only add edge if i < j to avoid duplicates
                    if (i < j)
                    {
                        // edge with a color based on the distance from the center of the hypersphere
                        float distanceFromCenter = Vertices[i].Magnitude() / Radius;
                        Color edgeColor = GetColorFromDistance(distanceFromCenter);
                        
                        Edges.Add(new Edge4D(i, j, edgeColor));
                    }
                }
            }
        }

        private Color GetColorFromDistance(float normalizedDistance)
        {
            // Map distance to a color
            // creates a gradient effect based on distance from the center
            
            int r = (int)(255 * Math.Min(1.0f, normalizedDistance * 2));
            int g = (int)(255 * (1.0f - Math.Abs(normalizedDistance - 0.5f) * 2));
            int b = (int)(255 * Math.Min(1.0f, (1.0f - normalizedDistance) * 2));
            
            return Color.FromArgb(r, g, b);
        }

        public override void Render(Renderer renderer)
        {
            base.Render(renderer);
            
            // Custom rendering for hypersphere if needed
        }
    }
}