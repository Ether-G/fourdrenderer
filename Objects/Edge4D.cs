using System;
using System.Collections.Generic;
using System.Drawing;
using FourDRenderer.Mathematics;
using FourDRenderer.Rendering;

namespace FourDRenderer.Objects
{
    public class Edge4D
    {
        public int StartVertexIndex { get; set; }
        public int EndVertexIndex { get; set; }
        public Color Color { get; set; }

        public Edge4D(int startIndex, int endIndex)
        {
            StartVertexIndex = startIndex;
            EndVertexIndex = endIndex;
            Color = Color.White; // Default color
        }

        public Edge4D(int startIndex, int endIndex, Color color)
        {
            StartVertexIndex = startIndex;
            EndVertexIndex = endIndex;
            Color = color;
        }

        // Render the edge using the provided renderer and projected vertices
        public void Render(Renderer renderer, List<Vector2D> projectedVertices)
        {
            if (StartVertexIndex < 0 || StartVertexIndex >= projectedVertices.Count ||
                EndVertexIndex < 0 || EndVertexIndex >= projectedVertices.Count)
            {
                // Index out of range, can't render this edge
                return;
            }

            Vector2D start = projectedVertices[StartVertexIndex];
            Vector2D end = projectedVertices[EndVertexIndex];

            renderer.DrawLine(start, end, Color);
        }
    }
}