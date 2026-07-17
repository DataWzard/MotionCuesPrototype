using System;
using System.Diagnostics;
namespace MotionCuesPrototype
{
    internal struct MotionVector { public double X; public double Y; public MotionVector(double x, double y) { X = x; Y = y; } }
    internal sealed class MotionEngine
    {
        private readonly Stopwatch clock = Stopwatch.StartNew(); private double smoothX; private double smoothY;
        public MotionVector Update(int cycleSeconds, bool invert)
        {
            double phase = clock.Elapsed.TotalSeconds / Math.Max(2, cycleSeconds) * Math.PI * 2.0;
            double x = Math.Sin(phase) * .68 + Math.Sin(phase * 2.0 + .8) * .16;
            double y = Math.Sin(phase + 1.4) * .48 + Math.Sin(phase * 2.0) * .13;
            if (invert) { x = -x; y = -y; }
            smoothX += (x - smoothX) * .075; smoothY += (y - smoothY) * .075; return new MotionVector(smoothX, smoothY);
        }
    }
}
