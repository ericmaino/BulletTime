using Windows.Media.MediaProperties;
using Newtonsoft.Json;

namespace BulletTime.Models
{
    public class VideoCameraResolutionModel
    {
        public VideoCameraResolutionModel()
        {
            IsRemote = true;
            Properties = new VideoEncodingProperties();
        }

        public VideoCameraResolutionModel(VideoEncodingProperties video)
        {
            IsRemote = false;
            Properties = video;
        }

        public VideoEncodingProperties Properties { get; set; }

        public uint Height
        {
            get { return Properties.Height; }
        }

        public uint Width
        {
            get { return Properties.Width; }
        }

        [JsonIgnore]
        public bool IsRemote { get; }

        public override string ToString()
        {
            return string.Format("{0} X {1} ({2} hz) {3} {4}:{5} {6}",
                Properties.Width, Properties.Height,
                Properties.FrameRate.Numerator/Properties.FrameRate.Denominator,
                Properties.Bitrate,
                Properties.Type,
                Properties.Subtype,
                Properties.PixelAspectRatio.Numerator/Properties.PixelAspectRatio.Denominator
                );
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}