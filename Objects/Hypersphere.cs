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
            Resolution = Math.Max(8, resolution);
            Name = "Hypersphere";
            
            // Generate geometry and store original vertices
            GenerateGeometry();
            StoreOriginalVertices();
        }

        public override void GenerateGeometry()
        {
            Vertices.Clear();
            Edges.Clear();

            // Generate vertices using proper 4D spherical coordinates
            Generate4DSphereVertices();
            
            // Create edges to form a wireframe
            CreateEdges();
        }

        private void Generate4DSphereVertices()
        {
            // 3 angular coordinates plus radius
            // 4D spherical coordinates (rho, phi, theta, psi)
            
            // 4D spherical coordinates:
            // x = rho * sin(psi) * sin(theta) * cos(phi)
            // y = rho * sin(psi) * sin(theta) * sin(phi)
            // z = rho * sin(psi) * cos(theta)
            // w = rho * cos(psi)
            
            int psiSteps = Resolution;      // (w-coordinate)
            int thetaSteps = Resolution;    // (z-coordinate)
            int phiSteps = Resolution;      // (xy-plane)
            
            // Generate vertices at the 8 "poles" of the coordinate system
            AddPoles();
            
            // nested loops to generate vertices across the 4D sphere surface
            for (int p = 1; p < psiSteps; p++) // Skip 0 and psiSteps which are poles
            {
                double psi = Math.PI * p / psiSteps;
                double sinPsi = Math.Sin(psi);
                double cosPsi = Math.Cos(psi);
                
                for (int t = 1; t < thetaSteps; t++) // Skip 0 and thetaSteps... handled separately
                {
                    double theta = Math.PI * t / thetaSteps;
                    double sinTheta = Math.Sin(theta);
                    double cosTheta = Math.Cos(theta);
                    
                    for (int ph = 0; ph < phiSteps; ph++)
                    {
                        double phi = 2.0 * Math.PI * ph / phiSteps;
                        double sinPhi = Math.Sin(phi);
                        double cosPhi = Math.Cos(phi);
                        
                        // Calculate 4D coordinates
                        float x = (float)(Radius * sinPsi * sinTheta * cosPhi);
                        float y = (float)(Radius * sinPsi * sinTheta * sinPhi);
                        float z = (float)(Radius * sinPsi * cosTheta);
                        float w = (float)(Radius * cosPsi);
                        
                        Vertices.Add(new Vector4D(x, y, z, w));
                    }
                }
            }
        }
        
        private void AddPoles()
        {
            // 8 "poles" of the 4D hypersphere
            // These are points along the main axes
            Vertices.Add(new Vector4D(0, 0, 0, Radius));    // +W pole
            Vertices.Add(new Vector4D(0, 0, 0, -Radius));   // -W pole
            Vertices.Add(new Vector4D(0, 0, Radius, 0));    // +Z pole
            Vertices.Add(new Vector4D(0, 0, -Radius, 0));   // -Z pole
            Vertices.Add(new Vector4D(0, Radius, 0, 0));    // +Y pole
            Vertices.Add(new Vector4D(0, -Radius, 0, 0));   // -Y pole
            Vertices.Add(new Vector4D(Radius, 0, 0, 0));    // +X pole
            Vertices.Add(new Vector4D(-Radius, 0, 0, 0));   // -X pole
            
            // Add additional points to create "great circles" connecting the poles
            // will serve as reference frames for the 4D structure
            AddGreatCircles();
        }
        
        private void AddGreatCircles()
        {
            // points along the 6 great circles connecting opposite poles
            // create the equator and prime meridians
            
            int circlePoints = Resolution / 2;
            
            // XY plane circle
            for (int i = 0; i < circlePoints; i++)
            {
                double angle = 2.0 * Math.PI * i / circlePoints;
                float x = (float)(Radius * Math.Cos(angle));
                float y = (float)(Radius * Math.Sin(angle));
                Vertices.Add(new Vector4D(x, y, 0, 0));
            }
            
            // XZ plane circle
            for (int i = 0; i < circlePoints; i++)
            {
                double angle = 2.0 * Math.PI * i / circlePoints;
                float x = (float)(Radius * Math.Cos(angle));
                float z = (float)(Radius * Math.Sin(angle));
                Vertices.Add(new Vector4D(x, 0, z, 0));
            }
            
            // XW plane circle
            for (int i = 0; i < circlePoints; i++)
            {
                double angle = 2.0 * Math.PI * i / circlePoints;
                float x = (float)(Radius * Math.Cos(angle));
                float w = (float)(Radius * Math.Sin(angle));
                Vertices.Add(new Vector4D(x, 0, 0, w));
            }
            
            // YZ plane circle
            for (int i = 0; i < circlePoints; i++)
            {
                double angle = 2.0 * Math.PI * i / circlePoints;
                float y = (float)(Radius * Math.Cos(angle));
                float z = (float)(Radius * Math.Sin(angle));
                Vertices.Add(new Vector4D(0, y, z, 0));
            }
            
            // YW plane circle
            for (int i = 0; i < circlePoints; i++)
            {
                double angle = 2.0 * Math.PI * i / circlePoints;
                float y = (float)(Radius * Math.Cos(angle));
                float w = (float)(Radius * Math.Sin(angle));
                Vertices.Add(new Vector4D(0, y, 0, w));
            }
            
            // ZW plane circle
            for (int i = 0; i < circlePoints; i++)
            {
                double angle = 2.0 * Math.PI * i / circlePoints;
                float z = (float)(Radius * Math.Cos(angle));
                float w = (float)(Radius * Math.Sin(angle));
                Vertices.Add(new Vector4D(0, 0, z, w));
            }
        }

        private void CreateEdges()
        {   
            // create great circles by connecting consecutive vertices along them
            CreateGreatCircleEdges();
            
            // connect other vertices based on proximity
            CreateProximityEdges();
        }
        
        private void CreateGreatCircleEdges()
        {
            // The first 8 vertices are the poles
            int poleCount = 8;
            
            // Connect poles with great circle segments
            for (int i = 0; i < 4; i++)
            {
                Edges.Add(new Edge4D(i*2, i*2+1, GetAxisColor(i)));
            }
            
            // Connect great circle points (which start after the poles)
            int circlePoints = Resolution / 2;
            int circleCount = 6; // 6 great circles
            
            for (int c = 0; c < circleCount; c++)
            {
                int circleStart = poleCount + c * circlePoints;
                
                // Connect points along each great circle
                for (int i = 0; i < circlePoints; i++)
                {
                    int current = circleStart + i;
                    int next = circleStart + (i + 1) % circlePoints;
                    
                    // Get appropriate color based on which plane the circle lies in
                    Color circleColor = GetCirclePlaneColor(c);
                    Edges.Add(new Edge4D(current, next, circleColor));
                }
                
                // Connect each circle to its corresponding poles
                if (c < 3) // First 3 circles involve X axis
                {
                    // Connect to +X and -X poles
                    for (int i = 0; i < circlePoints; i += circlePoints/4)
                    {
                        Edges.Add(new Edge4D(6, circleStart + i, Color.Red));
                        Edges.Add(new Edge4D(7, circleStart + (i + circlePoints/2) % circlePoints, Color.Red));
                    }
                }
                
                if (c == 0 || c == 3 || c == 4) // Circles involving Y axis
                {
                    // Connect to +Y and -Y poles
                    for (int i = 0; i < circlePoints; i += circlePoints/4)
                    {
                        Edges.Add(new Edge4D(4, circleStart + i, Color.Green));
                        Edges.Add(new Edge4D(5, circleStart + (i + circlePoints/2) % circlePoints, Color.Green));
                    }
                }
                
                if (c == 1 || c == 3 || c == 5) // Circles involving Z axis
                {
                    // Connect to +Z and -Z poles
                    for (int i = 0; i < circlePoints; i += circlePoints/4)
                    {
                        Edges.Add(new Edge4D(2, circleStart + i, Color.Blue));
                        Edges.Add(new Edge4D(3, circleStart + (i + circlePoints/2) % circlePoints, Color.Blue));
                    }
                }
                
                if (c == 2 || c == 4 || c == 5) // Circles involving W axis
                {
                    // Connect to +W and -W poles
                    for (int i = 0; i < circlePoints; i += circlePoints/4)
                    {
                        Edges.Add(new Edge4D(0, circleStart + i, Color.Yellow));
                        Edges.Add(new Edge4D(1, circleStart + (i + circlePoints/2) % circlePoints, Color.Yellow));
                    }
                }
            }
        }
        
        private void CreateProximityEdges()
        {
            // Create additional edges between vertices that are close to each other
            // This helps to visualize the 4D structure
            int poleCount = 8;
            int circlePoints = Resolution / 2;
            int circleCount = 6;
            int specialVertices = poleCount + circleCount * circlePoints;
            
            // Maximum distance for creating an edge
            float maxDistance = Radius * 0.5f;
            
            // First pass: connect regular vertices to poles and circle points
            for (int i = specialVertices; i < Vertices.Count; i++)
            {
                // Find closest special vertices (poles and great circle points)
                List<int> closestSpecial = new List<int>();
                
                for (int j = 0; j < specialVertices; j++)
                {
                    float distance = Vertices[i].Subtract(Vertices[j]).Magnitude();
                    if (distance < maxDistance)
                    {
                        closestSpecial.Add(j);
                    }
                }
                
                // Connect to up to 4 closest special vertices
                closestSpecial.Sort((a, b) => 
                {
                    float distA = Vertices[i].Subtract(Vertices[a]).Magnitude();
                    float distB = Vertices[i].Subtract(Vertices[b]).Magnitude();
                    return distA.CompareTo(distB);
                });
                
                int specialConnections = Math.Min(4, closestSpecial.Count);
                for (int j = 0; j < specialConnections; j++)
                {
                    // Color based on the w-coordinate difference
                    float wDiff = Math.Abs(Vertices[i].W - Vertices[closestSpecial[j]].W) / (2 * Radius);
                    Color edgeColor = GetWGradientColor(wDiff);
                    
                    Edges.Add(new Edge4D(i, closestSpecial[j], edgeColor));
                }
            }
            
            // Second pass: connect regular vertices to each other
            for (int i = specialVertices; i < Vertices.Count; i++)
            {
                for (int j = i + 1; j < Vertices.Count; j++)
                {
                    float distance = Vertices[i].Subtract(Vertices[j]).Magnitude();
                    if (distance < maxDistance * 0.7f) // Shorter distance for regular-to-regular
                    {
                        // Color based on the w-coordinate average
                        float wAvg = (Vertices[i].W + Vertices[j].W) / (2 * Radius);
                        Color edgeColor = GetWGradientColor((wAvg + 1) / 2); // Normalize from [-1,1] to [0,1]
                        
                        Edges.Add(new Edge4D(i, j, edgeColor));
                    }
                }
            }
        }

        private Color GetAxisColor(int axisIndex)
        {
            switch (axisIndex)
            {
                case 0: return Color.Yellow;   // W axis
                case 1: return Color.Blue;     // Z axis
                case 2: return Color.Green;    // Y axis
                case 3: return Color.Red;      // X axis
                default: return Color.White;
            }
        }
        
        private Color GetCirclePlaneColor(int planeIndex)
        {
            switch (planeIndex)
            {
                case 0: return Color.FromArgb(255, 128, 0);     // XY plane (orange)
                case 1: return Color.FromArgb(128, 255, 0);     // XZ plane (lime)
                case 2: return Color.FromArgb(255, 0, 255);     // XW plane (magenta)
                case 3: return Color.FromArgb(0, 255, 255);     // YZ plane (cyan)
                case 4: return Color.FromArgb(255, 255, 0);     // YW plane (yellow)
                case 5: return Color.FromArgb(128, 0, 255);     // ZW plane (purple)
                default: return Color.White;
            }
        }

        private Color GetWGradientColor(float normalizedW)
        {
            // Create a color gradient based on w-coordinate
            // This creates a visually appealing way to see the 4D structure
            
            // Normalize input to [0,1] range if needed
            normalizedW = Math.Max(0, Math.Min(1, normalizedW));
            
            if (normalizedW < 0.33)
            {
                return Color.FromArgb(
                    255, 
                    (int)(255 * normalizedW * 3), 
                    0);
            }
            else if (normalizedW < 0.66)
            {
                return Color.FromArgb(
                    (int)(255 * (2 - normalizedW * 3)),
                    255,
                    (int)(255 * (normalizedW * 3 - 1)));
            }
            else
            {
                return Color.FromArgb(
                    0,
                    (int)(255 * (3 - normalizedW * 3)),
                    255);
            }
        }

        public override void Render(Renderer renderer)
        {
            base.Render(renderer);
            
            // Draw "Hypersphere" label near the center
            Vector3D center3D = Center.ProjectTo3D(renderer.Camera.ViewerDistance);
            Vector2D labelPos = renderer.Camera.ProjectTo2D(center3D);
            labelPos.Y -= 30; // Move the label up a bit
            renderer.DrawText("Hypersphere", labelPos, Color.Cyan);
            
            // Draw axis labels
            Vector3D wAxis3D = new Vector4D(0, 0, 0, Radius * 0.8f).Add(Center).ProjectTo3D(renderer.Camera.ViewerDistance);
            Vector2D wAxisPos = renderer.Camera.ProjectTo2D(wAxis3D);
            renderer.DrawText("W", wAxisPos, Color.Yellow);
            
            Vector3D zAxis3D = new Vector4D(0, 0, Radius * 0.8f, 0).Add(Center).ProjectTo3D(renderer.Camera.ViewerDistance);
            Vector2D zAxisPos = renderer.Camera.ProjectTo2D(zAxis3D);
            renderer.DrawText("Z", zAxisPos, Color.Blue);
            
            Vector3D yAxis3D = new Vector4D(0, Radius * 0.8f, 0, 0).Add(Center).ProjectTo3D(renderer.Camera.ViewerDistance);
            Vector2D yAxisPos = renderer.Camera.ProjectTo2D(yAxis3D);
            renderer.DrawText("Y", yAxisPos, Color.Green);
            
            Vector3D xAxis3D = new Vector4D(Radius * 0.8f, 0, 0, 0).Add(Center).ProjectTo3D(renderer.Camera.ViewerDistance);
            Vector2D xAxisPos = renderer.Camera.ProjectTo2D(xAxis3D);
            renderer.DrawText("X", xAxisPos, Color.Red);
        }
    }
}