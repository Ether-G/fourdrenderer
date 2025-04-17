using System;
using System.Collections.Generic;
using System.Drawing;
using FourDRenderer.Mathematics;
using FourDRenderer.Rendering;

namespace FourDRenderer.Objects
{
    public class Pentachoron : Object4D
    {
        public float Size { get; private set; }

        public Pentachoron(float size = 1.0f)
        {
            Size = size;
            Name = "5-Cell (Pentachoron)";
            GenerateGeometry();
            StoreOriginalVertices();
        }

        public override void Render(Renderer renderer)
        {
            if (renderer?.Camera == null || Vertices == null || Edges == null)
            {
                return;
            }

            List<Vector2D> projected2D = new List<Vector2D>();
            // List<Vector3D> projected3D = new List<Vector3D>();

            foreach (Vector4D vertex in Vertices)
            {
                if (vertex == null) continue; // Skip null

                Vector3D proj3D = renderer.Camera.ProjectTo3D(vertex);
                // projected3D.Add(proj3D);

                if (proj3D == null) continue; // Skip if 3D projection failed

                Vector2D proj2D = renderer.Camera.ProjectTo2D(proj3D);
                projected2D.Add(proj2D); // Store final 2D point
            }

            foreach (Edge4D edge in Edges)
            {
                 if (edge != null)
                 {
                    // Use the consistent projected2D list
                    edge.Render(renderer, projected2D);
                 }
            }

            for (int i = 0; i < Vertices.Count; i++)
            {
                 // Check bounds to ensure the projected vertex exists for this index
                 if (i < projected2D.Count && projected2D[i] != null)
                 {
                    Color vertexColor = GetVertexColor(i);
                    Vector2D currentVertexPos = projected2D[i];

                    renderer.DrawPoint(currentVertexPos, vertexColor, 7);

                    renderer.DrawText($"V{i+1}", new Vector2D(currentVertexPos.X + 5, currentVertexPos.Y - 5), vertexColor);
                 }
            }

            if (Center != null)
            {
                Vector3D center3D = renderer.Camera.ProjectTo3D(Center);
                 if (center3D != null)
                 {
                     Vector2D labelPos = renderer.Camera.ProjectTo2D(center3D);
                      if (labelPos != null)
                      {
                          labelPos.Y -= 20; // Adjust label position
                          renderer.DrawText(Name ?? "Pentachoron", labelPos, Color.Cyan);
                      }
                 }
            }

             if (Center != null)
             {
                 Vector3D center3D = renderer.Camera.ProjectTo3D(Center);
                  if (center3D != null)
                  {
                     Vector2D center2D = renderer.Camera.ProjectTo2D(center3D);
                     if (center2D != null)
                     {
                          renderer.DrawPoint(center2D, Color.Magenta, 5);
                     }
                  }
             }
        }


        public override void GenerateGeometry()
        {
            Vertices.Clear();
            Edges.Clear();
            GenerateVertices();
            CreateEdges();
            CalculateCenter();
        }

         private void GenerateVertices()
        {
            float scale = Size / (float)Math.Sqrt(2.0);
            float a = scale / 2.0f;
            float b = scale / (2.0f * (float)Math.Sqrt(5.0));

            Vertices.Clear();
            Vertices.Add(new Vector4D( a,  a,  a, -b));
            Vertices.Add(new Vector4D(-a, -a,  a, -b));
            Vertices.Add(new Vector4D(-a,  a, -a, -b));
            Vertices.Add(new Vector4D( a, -a, -a, -b));
            Vertices.Add(new Vector4D( 0,  0,  0,  4.0f * b));
        }

        private void CreateEdges()
        {
            int vertexCount = Vertices.Count;
            Color[] edgeColors = new Color[] {
                Color.FromArgb(255, 100, 100), Color.FromArgb(100, 255, 100), Color.FromArgb(100, 100, 255),
                Color.FromArgb(255, 255, 100), Color.FromArgb(255, 100, 255),
            };
            Edges.Clear();
            for (int i = 0; i < vertexCount; i++) {
                for (int j = i + 1; j < vertexCount; j++) {
                    if (i < Vertices.Count && j < Vertices.Count) {
                       Edges.Add(new Edge4D(i, j, edgeColors[i]));
                    }
                }
            }
        }

        private Color GetVertexColor(int index)
        {
            switch (index)
            {
                case 0: return Color.FromArgb(255, 100, 100);    // Red
                case 1: return Color.FromArgb(100, 255, 100);    // Green
                case 2: return Color.FromArgb(100, 100, 255);    // Blue
                case 3: return Color.FromArgb(255, 255, 100);   // Yellow
                case 4: return Color.FromArgb(255, 100, 255);   // Magenta
                default: return Color.White;
            }
        }
    }
}