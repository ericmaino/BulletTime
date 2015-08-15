using System.Collections.Generic;
using System.Linq;
using System.Net;
using Windows.UI.Xaml.Media.Imaging;
using BulletTime.UI;
using Newtonsoft.Json;

namespace BulletTime.Models
{
    public class RemoteCameraModel : PropertyHost
    {
        public RemoteCameraModel(IPAddress endPoint, int port, IEnumerable<VideoCameraResolutionModel> resolutions)
            : this()
        {
            Resolutions = resolutions.Select(x => new RemoteResolutionModel(x));
            AddressAsBytes = endPoint.GetAddressBytes().ToList();
            Port = port;
        }

        public RemoteCameraModel()
        {
            View = this.NewProperty(x => x.View);
        }

        [JsonIgnore]
        public IPAddress IPAddress
        {
            get { return new IPAddress(AddressAsBytes.ToArray()); }
        }

        [JsonIgnore]
        public Property<WriteableBitmap> View { get; }

        public IEnumerable<byte> AddressAsBytes { get; set; }
        public IEnumerable<RemoteResolutionModel> Resolutions { get; set; }
        public RemoteCameraState State { get; set; }
        public int Port { get; set; }
    }
}