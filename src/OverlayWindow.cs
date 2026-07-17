using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;
using Drawing = System.Drawing;
namespace MotionCuesPrototype
{
    internal sealed class OverlayWindow : Window
    {
        private sealed class Particle
        {
            public Ellipse Dot;
            public double X, Y, VX, VY, Phase, Frequency, Scale;
        }

        const int GWL_EXSTYLE = -20, WS_EX_TRANSPARENT = 0x20, WS_EX_TOOLWINDOW = 0x80, WS_EX_NOACTIVATE = 0x08000000;
        [DllImport("user32.dll")] static extern int GetWindowLong(IntPtr h, int i);
        [DllImport("user32.dll")] static extern int SetWindowLong(IntPtr h, int i, int s);

        readonly CueSettings settings;
        readonly Canvas canvas;
        readonly List<Particle> particles = new List<Particle>();
        readonly Random random;
        readonly Stopwatch clock = Stopwatch.StartNew();
        double lastUpdate;

        public OverlayWindow(Forms.Screen screen, CueSettings s)
        {
            settings = s;
            random = new Random(Environment.TickCount ^ screen.Bounds.GetHashCode());
            WindowStyle = WindowStyle.None; ResizeMode = ResizeMode.NoResize; AllowsTransparency = true; Background = Brushes.Transparent; Topmost = true; ShowInTaskbar = false; ShowActivated = false; Focusable = false; IsHitTestVisible = false;
            Left = screen.Bounds.Left; Top = screen.Bounds.Top; Width = screen.Bounds.Width; Height = screen.Bounds.Height;
            canvas = new Canvas(); canvas.Background = Brushes.Transparent; Content = canvas;
            SourceInitialized += HandleSourceInitialized;
            Rebuild();
        }

        void HandleSourceInitialized(object sender, EventArgs e)
        {
            IntPtr h = new WindowInteropHelper(this).Handle;
            SetWindowLong(h, GWL_EXSTYLE, GetWindowLong(h, GWL_EXSTYLE) | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE);
        }

        public void RefreshAppearance()
        {
            if (particles.Count != settings.DotCount) Rebuild();
            SolidColorBrush fill = Brush();
            foreach (Particle p in particles)
            {
                double size = settings.DotSize * p.Scale;
                p.Dot.Width = size; p.Dot.Height = size; p.Dot.Fill = fill;
            }
            if (settings.Enabled && !IsVisible) Show(); else if (!settings.Enabled && IsVisible) Hide();
        }

        public void UpdateMotion(MotionVector v)
        {
            if (!settings.Enabled || !IsVisible) return;
            double now = clock.Elapsed.TotalSeconds;
            double dt = lastUpdate == 0 ? .033 : Math.Min(.10, now - lastUpdate);
            lastUpdate = now;
            double width = ActualWidth > 1 ? ActualWidth : Width;
            double height = ActualHeight > 1 ? ActualHeight : Height;
            double baseSpeed = 12.0 + settings.Sensitivity * .62;
            double globalX = baseSpeed * (.72 + v.X * .42);
            double globalY = baseSpeed * v.Y * .36;
            double response = Math.Min(1.0, dt * 1.8);

            foreach (Particle p in particles)
            {
                double wander = Math.Sin(now * p.Frequency + p.Phase) * 1.6 + Math.Cos(now * p.Frequency * .43 + p.Phase * .7) * .8;
                double targetX = globalX + Math.Cos(wander) * baseSpeed * .52;
                double targetY = globalY + Math.Sin(wander) * baseSpeed * .58;
                p.VX += (targetX - p.VX) * response;
                p.VY += (targetY - p.VY) * response;
                p.X += p.VX * dt;
                p.Y += p.VY * dt;

                double size = settings.DotSize * p.Scale;
                if (p.X > width + size) p.X = -size;
                else if (p.X < -size) p.X = width + size;
                if (p.Y > height + size) p.Y = -size;
                else if (p.Y < -size) p.Y = height + size;
                Canvas.SetLeft(p.Dot, p.X - size / 2.0);
                Canvas.SetTop(p.Dot, p.Y - size / 2.0);
            }
        }

        void Rebuild()
        {
            canvas.Children.Clear(); particles.Clear();
            SolidColorBrush fill = Brush();
            double width = Math.Max(100, Width), height = Math.Max(100, Height);
            for (int i = 0; i < settings.DotCount; i++)
            {
                Particle p = new Particle();
                p.Scale = .72 + random.NextDouble() * .52;
                p.Phase = random.NextDouble() * Math.PI * 2.0;
                p.Frequency = .28 + random.NextDouble() * .92;
                p.X = random.NextDouble() * width;
                p.Y = random.NextDouble() * height;
                p.Dot = new Ellipse { Fill = fill, IsHitTestVisible = false };
                double size = settings.DotSize * p.Scale;
                p.Dot.Width = size; p.Dot.Height = size;
                particles.Add(p); canvas.Children.Add(p.Dot);
                Canvas.SetLeft(p.Dot, p.X); Canvas.SetTop(p.Dot, p.Y);
            }
        }

        SolidColorBrush Brush()
        {
            Drawing.Color c = settings.DotColor;
            SolidColorBrush b = new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)settings.Opacity, c.R, c.G, c.B));
            b.Freeze(); return b;
        }
    }
}
