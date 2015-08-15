using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Buffer = Windows.Storage.Streams.Buffer;

namespace BulletTime.Storage
{
    public class StorageIO
    {
        public static async Task<IBuffer> ReadIntoBuffer(IRandomAccessStream stream)
        {
            stream.Seek(0);
            var buffer = new Buffer((uint) stream.Size);
            await stream.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.None);
            return buffer;
        }

        public static async Task CopyFromStream(IRandomAccessStream stream, IOutputStream output)
        {
            var buffer = await ReadIntoBuffer(stream);
            await output.WriteAsync(buffer);
        }

        public static async Task CopyFromStream(IRandomAccessStream stream, StorageFile file)
        {
            var buffer = await ReadIntoBuffer(stream);
            await FileIO.WriteBufferAsync(file, buffer);
        }
    }
}