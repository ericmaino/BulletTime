using Windows.Storage.Streams;

namespace BulletTime.Networking
{
    public class CameraHeartBeat
    {
        public CameraHeartBeat(string cameraId, IBuffer buffer)
        {
            CameraId = cameraId;
            ViewBuffer = buffer;
        }

        public string CameraId { get; }
        public IBuffer ViewBuffer { get; }
    }
}