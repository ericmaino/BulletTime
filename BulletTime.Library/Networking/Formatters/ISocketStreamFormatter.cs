using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BulletTime.Networking.Formatters
{
    public interface ISocketStreamFormatter<T>
    {
        Task<T> Read(IDataReader reader);
        Task Write(IDataWriter reader, T dataFrame);
    }
}