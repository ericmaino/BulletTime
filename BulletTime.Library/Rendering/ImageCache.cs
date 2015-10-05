using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using BulletTime.Models;

namespace BulletTime.Rendering
{
    public class ImageCache
    {
        private readonly IList<RemoteCameraModel> _camers;
        private readonly IDictionary<string, WriteableBitmap> _images;

        public ImageCache(IEnumerable<RemoteCameraModel> cameras)
        {
            _images = new Dictionary<string, WriteableBitmap>(StringComparer.OrdinalIgnoreCase);
            _camers = cameras.ToList().AsReadOnly();
        }

        public async Task<WriteableBitmap> GetImage(MappedFrame frame)
        {
            var key = $"{frame.Camera}:{frame.Frame}";

            if (!_images.ContainsKey(key))
            {
                var camera = _camers[frame.Camera];
                _images[key] = await (await ImageLoader.Create(camera)).GetImage(frame);
            }

            return _images[key];
        }
    }
}