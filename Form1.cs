using System;
using System.Drawing;
using System.Windows.Forms;
using FourDRenderer.Scene;
using EngineKeys = FourDRenderer.Scene.Keys; // Alias to avoid ambiguity

namespace FourDRenderer
{
    public partial class Form1 : Form
    {
        private Engine4D _engine = null!;
        private System.Windows.Forms.Timer _renderTimer = null!;
        private PictureBox _renderSurface = null!;

        public Form1()
        {
            InitializeFormComponents();
            InitializeEngine();
        }

        private void InitializeFormComponents()
        {
            this.SuspendLayout();
            
            // Form settings
            this.ClientSize = new Size(800, 600);
            this.Text = "4D Renderer";
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(400, 300);
            
            // Create PictureBox for rendering
            _renderSurface = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Normal
            };
            this.Controls.Add(_renderSurface);
            
            // Set up event handlers
            this.Load += Form1_Load;
            this.Resize += Form1_Resize;
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            
            this.ResumeLayout(false);
        }

        private void InitializeEngine()
        {
            // Create rendering engine
            _engine = new Engine4D(ClientSize.Width, ClientSize.Height);
            _engine.Initialize();
            
            // Create render timer
            _renderTimer = new System.Windows.Forms.Timer
            {
                Interval = 16 // ~60 FPS
            };
            _renderTimer.Tick += RenderTimer_Tick;
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            // Start rendering
            _renderTimer.Start();
        }

        private void Form1_Resize(object? sender, EventArgs e)
        {
            if (_engine != null && WindowState != FormWindowState.Minimized)
            {
                // Update engine and renderer with new size
                _engine.Resize(ClientSize.Width, ClientSize.Height);
            }
        }

        private void RenderTimer_Tick(object? sender, EventArgs e)
        {
            // Render frame
            _engine.Render();
            
            // Update display
            _renderSurface.Image?.Dispose();
            _renderSurface.Image = (Bitmap)_engine.Renderer.Canvas.Clone();
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            // Map Windows.Forms keys to our engine keys
            _engine.ProcessInput(MapKey(e.KeyCode), true);
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            // Map Windows.Forms keys to our engine keys
            _engine.ProcessInput(MapKey(e.KeyCode), false);
        }

        // Map Windows.Forms keys to our engine keys
        private EngineKeys MapKey(System.Windows.Forms.Keys key)
        {
            switch (key)
            {
                case System.Windows.Forms.Keys.Space: return EngineKeys.Space;
                case System.Windows.Forms.Keys.D1: return EngineKeys.D1;
                case System.Windows.Forms.Keys.D2: return EngineKeys.D2;
                case System.Windows.Forms.Keys.D3: return EngineKeys.D3;
                case System.Windows.Forms.Keys.D4: return EngineKeys.D4;
                case System.Windows.Forms.Keys.D5: return EngineKeys.D5;
                case System.Windows.Forms.Keys.D6: return EngineKeys.D6;
                case System.Windows.Forms.Keys.W: return EngineKeys.W;
                case System.Windows.Forms.Keys.A: return EngineKeys.A;
                case System.Windows.Forms.Keys.S: return EngineKeys.S;
                case System.Windows.Forms.Keys.D: return EngineKeys.D;
                case System.Windows.Forms.Keys.Q: return EngineKeys.Q;
                case System.Windows.Forms.Keys.E: return EngineKeys.E;
                case System.Windows.Forms.Keys.R: return EngineKeys.R;
                case System.Windows.Forms.Keys.F: return EngineKeys.F;
                case System.Windows.Forms.Keys.Up: return EngineKeys.Up;
                case System.Windows.Forms.Keys.Down: return EngineKeys.Down;
                case System.Windows.Forms.Keys.Left: return EngineKeys.Left;
                case System.Windows.Forms.Keys.Right: return EngineKeys.Right;
                case System.Windows.Forms.Keys.Add: return EngineKeys.Add;
                case System.Windows.Forms.Keys.Subtract: return EngineKeys.Subtract;
                case System.Windows.Forms.Keys.Oemplus: return EngineKeys.OemPlus;
                case System.Windows.Forms.Keys.OemMinus: return EngineKeys.OemMinus;
                default: return 0;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Stop the render timer
            _renderTimer.Stop();
            
            // Clean up resources
            _engine.Dispose();
            _renderSurface.Image?.Dispose();
            
            base.OnFormClosing(e);
        }
    }
}