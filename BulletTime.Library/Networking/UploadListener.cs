using System;
using System.Threading.Tasks;
using Windows.Storage;
using BulletTime.Models;
using BulletTime.Networking.Formatters;

namespace BulletTime.Networking
{
    internal class UploadListener : SocketListener<CameraFrame>
    {
        private StorageFolder _folder;

        public UploadListener()
            : base(new CameraFrameFormatter())
        {
            DataRecieved += _listener_DataRecieved;
        }

        private async Task _listener_DataRecieved(CameraFrame frame)
        {
            var folder = await _folder.CreateFolderAsync(frame.CameraId, CreationCollisionOption.OpenIfExists);
            var outFile = await folder.CreateFileAsync($"{frame.FrameIndex:D2}.jpg", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBufferAsync(outFile, frame.Stream);
        }

        public override async Task Bind()
        {
            await base.Bind();
            _folder = await KnownFolders.VideosLibrary.CreateFolderAsync("BulletTime", CreationCollisionOption.OpenIfExists);
        }
    }
}