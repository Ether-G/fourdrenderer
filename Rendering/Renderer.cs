using System;
using System.Drawing;
using FourDRenderer.Mathematics;
using FourDRenderer.Objects;

namespace FourDRenderer.Rendering
{
    public class Renderer
    {
        public Bitmap Canvas { get; private set; }
        public Graphics Graphics { get; private set; }
        public Camera4D Camera { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Color BackgroundColor { get; set; }

        // Pens for drawing
        private Pen _defaultPen;
        private Pen _customPen;

        public Renderer(int width, int height)
        {
            Width = width;
            Height = height;
            Canvas = new Bitmap(width, height);
            Graphics = Graphics.FromImage(Canvas);
            Camera = new Camera4D();
            Camera.SetScreenParameters(width, height);
            BackgroundColor = Color.Black;
            
            _defaultPen = new Pen(Color.White, 1);
            _customPen = new Pen(Color.White, 1);

            // Set up basic rendering settings
            Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        // Resize the rendering surface
        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            
            // Dispose of old resources
            Graphics.Dispose();
            Canvas.Dispose();
            
            // Create new rendering surface
            Canvas = new Bitmap(width, height);
            Graphics = Graphics.FromImage(Canvas);
            Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            // Update camera parameters
            Camera.SetScreenParameters(width, height);
        }

        // Clear the canvas with background color
        public void Clear(Color? color = null)
        {
            Graphics.Clear(color ?? BackgroundColor);
        }

        // Draw a line between two 2D points
        public void DrawLine(Vector2D start, Vector2D end, Color color)
        {
            // Check if the points are within a reasonable range
            if (IsPointVisible(start) || IsPointVisible(end))
            {
                _customPen.Color = color;
                Graphics.DrawLine(_customPen, start.ToPoint(), end.ToPoint());
            }
        }

        // Draw a point at the specified 2D location
        public void DrawPoint(Vector2D point, Color color, int size = 3)
        {
            if (IsPointVisible(point))
            {
                _customPen.Color = color;
                Graphics.DrawEllipse(_customPen, point.X - size/2, point.Y - size/2, size, size);
            }
        }

        // Check if point is within a visible range
        private bool IsPointVisible(Vector2D point)
        {
            // Add a margin around the screen
            int margin = 1000;
            return point.X > -margin && point.X < Width + margin && 
                   point.Y > -margin && point.Y < Height + margin;
        }

        // Render a 4D object
        public void RenderObject(Object4D obj)
        {
            obj.Render(this);
        }

        // Draw text at the specified location
        public void DrawText(string text, Vector2D position, Color color, Font? font = null)
        {
            using (Brush brush = new SolidBrush(color))
            {
                Graphics.DrawString(text, font ?? SystemFonts.DefaultFont, brush, position.ToPoint());
            }
        }

        // Draw debug information
        public void DrawDebugInfo(Point position, Object4D? object4D = null)
        {
            using (Brush brush = new SolidBrush(Color.Yellow))
            {
                string info = $"Viewer Distance: {Camera.ViewerDistance:F2}\n" +
                              $"Screen3D Distance: {Camera.Screen3DDistance:F2}\n" +
                              $"Camera Position: ({Camera.Position.X:F2}, {Camera.Position.Y:F2}, " +
                              $"{Camera.Position.Z:F2}, {Camera.Position.W:F2})";

                if (object4D != null)
                {
                    info += $"\nObject: {object4D.Name}\n" +
                            $"Vertices: {object4D.Vertices.Count}, Edges: {object4D.Edges.Count}";
                }

                Graphics.DrawString(info, SystemFonts.DefaultFont, brush, position);
            }
        }

        // Clean up resources
        public void Dispose()
        {
            _defaultPen.Dispose();
            _customPen.Dispose();
            Graphics.Dispose();
            Canvas.Dispose();
        }
    }
}