using System;
using System.Collections.Generic;
using System.Drawing;
using FourDRenderer.Mathematics;
using FourDRenderer.Rendering;

namespace FourDRenderer.Objects
{
    public class Tesseract : Object4D
    {
        public float Size { get; private set; }

        public Tesseract(float size = 1.0f)
        {
            Size = size;
            Name = "Tesseract";
            GenerateGeometry();
            StoreOriginalVertices(); // Store original vertices after generation
        }

        public override void GenerateGeometry()
        {
            Vertices.Clear();
            Edges.Clear();

            // Generate the 16 vertices of a tesseract
            float halfSize = Size / 2;
            
            // Generate all combinations of (±halfSize, ±halfSize, ±halfSize, ±halfSize)
            for (int i = 0; i < 16; i++)
            {
                float x = ((i & 1) == 0) ? -halfSize : halfSize;
                float y = ((i & 2) == 0) ? -halfSize : halfSize;
                float z = ((i & 4) == 0) ? -halfSize : halfSize;
                float w = ((i & 8) == 0) ? -halfSize : halfSize;
                
                Vertices.Add(new Vector4D(x, y, z, w));
            }

            // Generate edges - each vertex connects to 4 others
            for (int i = 0; i < 16; i++)
            {
                for (int j = i + 1; j < 16; j++)
                {
                    if (ShouldConnect(i, j))
                    {
                        // Create an edge with color based on which dimensions differ
                        Color edgeColor = GetEdgeColor(i, j);
                        Edges.Add(new Edge4D(i, j, edgeColor));
                    }
                }
            }
        }

        // Helper method to determine if two vertices should be connected
        private bool ShouldConnect(int index1, int index2)
        {
            // Two vertices are connected if they differ in exactly one bit
            // This ensures each vertex connects to exactly 4 others
            int xor = index1 ^ index2;
            
            // Check if xor has exactly one bit set (power of 2)
            return xor != 0 && (xor & (xor - 1)) == 0;
        }

        // Helper method to get color based on which dimension the edge represents
        private Color GetEdgeColor(int index1, int index2)
        {
            int xor = index1 ^ index2;
            
            if (xor == 1) return Color.Red;      // X-axis edge
            if (xor == 2) return Color.Green;    // Y-axis edge
            if (xor == 4) return Color.Blue;     // Z-axis edge
            if (xor == 8) return Color.Yellow;   // W-axis edge
            
            return Color.White; // Shouldnt happen but just in case
        }

        public override void Render(Renderer renderer)
        {
            base.Render(renderer);
            
            // Draw label near the center
            Vector3D center3D = Center.ProjectTo3D(renderer.Camera.ViewerDistance);
            Vector2D labelPos = renderer.Camera.ProjectTo2D(center3D);
            labelPos.Y -= 30;
            renderer.DrawText("Tesseract", labelPos, Color.Cyan);
        }
    }
}