using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using BulletTime.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BulletTime.Rendering
{
    public class ImageLoader
    {
        private readonly StorageFolder _cameraFolder;

        private ImageLoader(StorageFolder folder)
        {
            _cameraFolder = folder;
        }

        public static async Task<ImageLoader> Create(RemoteCameraModel camera)
        {
            var folder = await KnownFolders.VideosLibrary.CreateFolderAsync("BulletTime", CreationCollisionOption.OpenIfExists);
            folder = await folder.CreateFolderAsync(camera.IPAddress.ToString(), CreationCollisionOption.OpenIfExists);
            return new ImageLoader(folder);
        }

        public async Task<WriteableBitmap> GetImage(MappedFrame frame)
        {
            var file = await _cameraFolder.OpenStreamForReadAsync($"{frame.Frame:D2}.jpg");
            return await GetImage(file.AsRandomAccessStream());
        }

        public async Task<IEnumerable<BitmapImage>> GetFrameImages()
        {
            var frames = new List<BitmapImage>();
            foreach (var file in await _cameraFolder.GetFilesAsync(CommonFileQuery.OrderByName))
            {
                var stream = await file.OpenStreamForReadAsync();

                var img = new BitmapImage();
                img.DecodePixelWidth = 640;
                img.SetSource(stream.AsRandomAccessStream());
                frames.Add(img);
            }

            return frames;
        }

        private async Task<WriteableBitmap> GetImage(IRandomAccessStream stream)
        {
            var decoder = await BitmapDecoder.CreateAsync(stream);
            var f = await decoder.GetFrameAsync(0);
            var bmp = new WriteableBitmap((int) f.PixelWidth, (int) f.PixelHeight);
            await bmp.SetSourceAsync(stream);
            return bmp;
        }
    }
}