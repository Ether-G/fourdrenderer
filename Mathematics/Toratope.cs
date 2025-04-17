using System;
using System.Collections.Generic;
using System.Drawing;
using FourDRenderer.Mathematics;
using FourDRenderer.Rendering;

namespace FourDRenderer.Objects
{
    public class Toratope : Object4D
    {
        public float MajorRadius { get; private set; }
        public float MinorRadius { get; private set; }
        public int Resolution { get; private set; }

        /// <summary>
        /// Creates a 4D torus (toratope)
        /// </summary>
        /// <param name="majorRadius">The distance from the center of the torus to the center of the tube</param>
        /// <param name="minorRadius">The radius of the tube</param>
        /// <param name="resolution">The resolution of the torus mesh</param>
        public Toratope(float majorRadius = 1.5f, float minorRadius = 0.5f, int resolution = 12)
        {
            MajorRadius = majorRadius;
            MinorRadius = minorRadius;
            Resolution = Math.Max(8, resolution); // Ensure minimum resolution
            Name = "Toratope";
            
            GenerateGeometry();
            StoreOriginalVertices();
        }

        public override void GenerateGeometry()
        {
            Vertices.Clear();
            Edges.Clear();

            // A 4D torus (toratope) is generated using three angular parameters
            // Think of it as a circle in one plane, swept around a circle in another plane,
            // and that entire configuration swept around a circle in yet another plane
            GenerateTorusVertices();
            CreateEdges();
        }

        private void GenerateTorusVertices()
        {
            // For a 4D torus, we need three angular parameters
            // theta1: angle in the XY plane (major circle)
            // theta2: angle in the ZW plane (second circle)
            // theta3: angle that rotates between these planes (third rotation)
            
            int steps1 = Resolution;  // Major circle steps
            int steps2 = Resolution;  // Second circle steps
            int steps3 = 6;           // Third rotation steps (fewer to avoid too many vertices)
            
            // Generate vertices for the full 4D torus
            for (int i = 0; i < steps1; i++)
            {
                double theta1 = 2.0 * Math.PI * i / steps1;
                double cos1 = Math.Cos(theta1);
                double sin1 = Math.Sin(theta1);
                
                for (int j = 0; j < steps2; j++)
                {
                    double theta2 = 2.0 * Math.PI * j / steps2;
                    double cos2 = Math.Cos(theta2);
                    double sin2 = Math.Sin(theta2);
                    
                    for (int k = 0; k < steps3; k++)
                    {
                        double theta3 = 2.0 * Math.PI * k / steps3;
                        double cos3 = Math.Cos(theta3);
                        double sin3 = Math.Sin(theta3);
                        
                        // Calculate 4D point using all three angles
                        // This is the parametric equation for a 4D torus with true 4D rotation
                        float x = (float)((MajorRadius + MinorRadius * cos2 * cos3) * cos1);
                        float y = (float)((MajorRadius + MinorRadius * cos2 * cos3) * sin1);
                        float z = (float)(MinorRadius * sin2 * cos3);
                        float w = (float)(MinorRadius * sin3);
                        
                        Vertices.Add(new Vector4D(x, y, z, w));
                    }
                }
            }
        }

        private void CreateEdges()
        {
            // Create edges connecting vertices to form a wireframe structure
            int steps1 = Resolution;   // Major circle steps
            int steps2 = Resolution;   // Second circle steps
            int steps3 = 6;            // Third rotation steps
            
            // Connect vertices in each of the three circular directions
            for (int i = 0; i < steps1; i++)
            {
                for (int j = 0; j < steps2; j++)
                {
                    for (int k = 0; k < steps3; k++)
                    {
                        // Calculate current vertex index
                        int current = (i * steps2 + j) * steps3 + k;
                        
                        // Calculate indices of vertices in each of the three circular directions
                        int next1 = ((i + 1) % steps1 * steps2 + j) * steps3 + k;
                        int next2 = (i * steps2 + (j + 1) % steps2) * steps3 + k;
                        int next3 = (i * steps2 + j) * steps3 + (k + 1) % steps3;
                        
                        // Create edges with different colors for each circular direction
                        Edges.Add(new Edge4D(current, next1, Color.FromArgb(255, 100, 100)));  // Major circle - red
                        Edges.Add(new Edge4D(current, next2, Color.FromArgb(100, 255, 100)));  // Second circle - green
                        Edges.Add(new Edge4D(current, next3, Color.FromArgb(100, 100, 255)));  // Third rotation - blue
                    }
                }
            }
        }

        public override void Render(Renderer renderer)
        {
            base.Render(renderer);
            
            // Draw label
            Vector3D center3D = Center.ProjectTo3D(renderer.Camera.ViewerDistance);
            Vector2D labelPos = renderer.Camera.ProjectTo2D(center3D);
            labelPos.Y -= 30;
            renderer.DrawText("4D Torus", labelPos, Color.Cyan);
        }
    }
}