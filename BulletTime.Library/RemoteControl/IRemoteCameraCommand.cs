using System.IO;
using System.Threading.Tasks;

namespace BulletTime.RemoteControl
{
    public interface IRemoteCameraCommand<T> : IRemoteCameraCommand
    {
        T Parse(StringReader reader);
    }

    public interface IRemoteCameraCommand
    {
        string Name { get; }
        Task Execute();
        string Serialize();
    }
}