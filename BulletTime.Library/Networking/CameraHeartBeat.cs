using Windows.Storage.Streams;
using BulletTime.RemoteControl;

namespace BulletTime.Networking
{
    public class CameraHeartBeat
    {
        public CameraHeartBeat(string cameraId, IBuffer buffer, CameraClientState state)
        {
            CameraId = cameraId;
            ViewBuffer = buffer;
            State = state;
        }

        public string CameraId { get; }
        public IBuffer ViewBuffer { get; }
        public CameraClientState State { get; }
    }
}