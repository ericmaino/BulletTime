using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;

namespace BulletTime.Models
{
    public class VideoCameraModel
    {
        public VideoCameraModel(DeviceInformation camera)
        {
            Camera = camera;
            Media = new MediaCapture();
            SinglePhotoSettings = ImageEncodingProperties.CreateJpeg();
        }

        public bool IsCameraOn { get; set; }
        public MediaCapture Media { get; }
        public DeviceInformation Camera { get; }
        public VideoEncodingProperties Resolution { get; set; }
        public ImageEncodingProperties SinglePhotoSettings { get; }
    }
}