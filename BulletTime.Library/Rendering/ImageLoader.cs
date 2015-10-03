using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using BulletTime.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BulletTime.Rendering
{
    public class ImageLoader
    {
        private readonly IList<RemoteCameraModel> _camers;
        private readonly StorageFolder _folder;
        private readonly IDictionary<string, WriteableBitmap> _images;

        private ImageLoader(IEnumerable<RemoteCameraModel> cameras, StorageFolder folder)
        {
            _images = new Dictionary<string, WriteableBitmap>(StringComparer.OrdinalIgnoreCase);
            _camers = cameras.ToList().AsReadOnly();
            this._folder = folder;
        }

        public static async Task<ImageLoader> Create(IEnumerable<RemoteCameraModel> cameras)
        {
            var folder = await KnownFolders.VideosLibrary.CreateFolderAsync("BulletTime", CreationCollisionOption.OpenIfExists);
            return new ImageLoader(cameras, folder);
        }

        public async Task<WriteableBitmap> GetImage(MappedFrame frame)
        {
            var key = $"{frame.Camera}:{frame.Frame}";

            if (!_images.ContainsKey(key))
            {
                var ip = _camers[frame.Camera].IPAddress;
                var cameraFolder = await _folder.CreateFolderAsync(ip.ToString(), CreationCollisionOption.OpenIfExists);
                var file = await cameraFolder.OpenStreamForReadAsync($"{frame.Frame:D2}.jpg");
                var decoder = await BitmapDecoder.CreateAsync(file.AsRandomAccessStream());
                var f = await decoder.GetFrameAsync(0);
                var bmp = new WriteableBitmap((int)f.PixelWidth, (int)f.PixelHeight);
                await bmp.SetSourceAsync(file.AsRandomAccessStream());
                _images[key] = bmp;
            }

            return _images[key];
        }
    }
}
