using System;
using Windows.Foundation;

namespace BulletTime.Models
{
    public class MappedFrame
    {
        public MappedFrame()
        {
            MappedTime = DateTime.UtcNow;
        }

        public DateTimeOffset MappedTime { get; }
        public Point Point { get; set; }
        public int Camera { get; set; }
        public double Speed { get; set; }
        public int Frame { get; set; }
        public double ViewTime { get; set; }

        public override string ToString()
        {
            return $"Camera {Camera} Frame {Frame} Speed {Speed}";
        }
    }
}