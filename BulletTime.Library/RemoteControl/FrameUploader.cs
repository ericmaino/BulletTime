using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using BulletTime.Models;
using BulletTime.Networking.Formatters;

namespace BulletTime.RemoteControl
{
    public class FrameUploader
    {
        private readonly string _cameraId;
        private readonly HostName _host;
        private readonly string _port;

        public FrameUploader(string id, IPAddress address, int port)
        {
            _host = new HostName(address.ToString());
            _port = port.ToString();
            _cameraId = id;
            UploadComplete = () => { };
        }

        public event Action UploadComplete;

        public async Task Upload(IEnumerable<IRandomAccessStream> frames)
        {
            var frameId = 1;
            foreach (var stream in frames)
            {
                using (var socket = new StreamSocket())
                {
                    await socket.ConnectAsync(_host, _port);
                    var formatter = new CameraFrameFormatter();
                    var writer = new DataWriter(socket.OutputStream);
                    await formatter.Write(writer, await CameraFrame.Create(_cameraId, frameId++, stream));
                    await writer.StoreAsync();
                }
            }

            UploadComplete();
        }
    }
}