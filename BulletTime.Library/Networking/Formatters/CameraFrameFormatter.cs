using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using BulletTime.Models;

namespace BulletTime.Networking.Formatters
{
    public class CameraFrameFormatter : ISocketStreamFormatter<CameraFrame>
    {
        public async Task Write(IDataWriter writer, CameraFrame frame)
        {
            writer.WriteInt32(frame.FrameIndex);
            var bytes = Encoding.UTF8.GetBytes(frame.CameraId);
            writer.WriteInt32(bytes.Length);
            writer.WriteBytes(bytes);
            writer.WriteUInt32(frame.Stream.Length);
            writer.WriteBuffer(frame.Stream);
            await Task.Yield();
        }

        public async Task<CameraFrame> Read(IDataReader reader)
        {
            await reader.LoadAsync(sizeof (int)*2);
            var frameIndex = reader.ReadInt32();
            var strSize = reader.ReadInt32();
            await reader.LoadAsync((uint) (sizeof (byte)*strSize));
            var bytes = new byte[strSize];
            reader.ReadBytes(bytes);
            var cameraId = Encoding.UTF8.GetString(bytes);
            await reader.LoadAsync(sizeof (int));
            var size = reader.ReadUInt32();
            await reader.LoadAsync(size);
            var buffer = reader.ReadBuffer(size);
            return new CameraFrame(cameraId, frameIndex, buffer);
        }
    }
}