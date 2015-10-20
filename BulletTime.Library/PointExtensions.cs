using System;
using Windows.Foundation;

namespace BulletTime
{
    public static class PointExtensions
    {
        public static double GetDistance(this Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(p2.X - p1.X), 2) + Math.Pow(Math.Abs(p2.Y - p1.Y), 2));
        }
    }
}