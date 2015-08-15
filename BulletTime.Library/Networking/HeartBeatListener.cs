using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using BulletTime.Networking.Formatters;

namespace BulletTime.Networking
{
    public class HeartBeatListener : SocketListener<CameraHeartBeat>
    {
        public HeartBeatListener()
            : base(new Formatter())
        {
        }

        public class Formatter : ISocketStreamFormatter<CameraHeartBeat>
        {
            public async Task Write(IDataWriter writer, CameraHeartBeat dataFrame)
            {
                var bytes = Encoding.UTF8.GetBytes(dataFrame.CameraId);
                writer.WriteInt32(bytes.Length);
                writer.WriteUInt32(dataFrame.ViewBuffer.Length);
                writer.WriteBytes(bytes);
                writer.WriteBuffer(dataFrame.ViewBuffer);

                await Task.Yield();
            }

            public async Task<CameraHeartBeat> Read(IDataReader reader)
            {
                await reader.LoadAsync(sizeof (int)*2);
                var strSize = reader.ReadInt32();
                var bufferSize = reader.ReadUInt32();
                await reader.LoadAsync((uint) (sizeof (byte)*strSize));
                var bytes = new byte[strSize];
                reader.ReadBytes(bytes);
                var cameraId = Encoding.UTF8.GetString(bytes);
                await reader.LoadAsync(bufferSize);
                var buffer = reader.ReadBuffer(bufferSize);

                return new CameraHeartBeat(cameraId, buffer);
            }
        }
    }
}