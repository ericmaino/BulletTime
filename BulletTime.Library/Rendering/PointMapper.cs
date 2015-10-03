using System;
using System.Collections.Generic;
using Windows.Foundation;
using BulletTime.Models;

namespace BulletTime.Rendering
{
    public class PointMapper
    {
        private readonly int height;
        private readonly int width;
        private readonly int cameraCount;
        private readonly int frameCount;
        private readonly IList<MappedFrame> frames;

        public PointMapper(double canvasHeight, double canvasWidth, int cameraCount, int expectedFrames)
        {
            frames = new List<MappedFrame>();
            this.cameraCount = cameraCount;
            height = (int)canvasHeight;
            width = (int)canvasWidth;
            frameCount = expectedFrames;
        }

        private MappedFrame PreviousFrame { get; set; }

        public IEnumerable<MappedFrame> MappedFrames
        {
            get { return frames; }
        }

        public bool AddPoint(Point currentPoint)
        {
            bool result = false;

            if (PreviousFrame == null)
            {
                PreviousFrame = new MappedFrame()
                {
                    Point = currentPoint
                };

                result = true;
            }
            else
            {
                var x1 = PreviousFrame.Point.X;
                var y1 = PreviousFrame.Point.Y;
                var x2 = currentPoint.X;
                var y2 = currentPoint.Y;

                var frame = (int)((x2 * frameCount) / width) + 2;
                var camera = (int)((y2 * cameraCount) / height);
                var distance = PreviousFrame.Point.GetDistance(currentPoint);

                if (frame != PreviousFrame.Frame || camera != PreviousFrame.Camera)
                {
                    var mappedFrame = new MappedFrame()
                    {
                        Speed = distance / (DateTime.UtcNow - PreviousFrame.MappedTime).TotalSeconds,
                        ViewTime = (DateTime.UtcNow - PreviousFrame.MappedTime).TotalMilliseconds,
                        Point = currentPoint,
                        Camera = camera,
                        Frame = frame
                    };

                    PreviousFrame = mappedFrame;
                    frames.Add(mappedFrame);
                    result = true;
                }
            }

            return result;
        }


    }
}