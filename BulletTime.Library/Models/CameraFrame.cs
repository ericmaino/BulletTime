using System.Threading.Tasks;
using Windows.Storage.Streams;
using BulletTime.Storage;

namespace BulletTime.Models
{
    public class CameraFrame
    {
        public CameraFrame(string id, int index, IBuffer buffer)
        {
            FrameIndex = index;
            CameraId = id;
            Stream = buffer;
        }

        public int FrameIndex { get; }
        public string CameraId { get; }
        public IBuffer Stream { get; }

        public static async Task<CameraFrame> Create(string id, int index, IRandomAccessStream stream)
        {
            return new CameraFrame(id, index, await StorageIO.ReadIntoBuffer(stream));
        }
    }
}