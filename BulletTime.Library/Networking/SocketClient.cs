using System;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using BulletTime.Networking.Formatters;

namespace BulletTime.Networking
{
    public class SocketClient<T> : INetworkClient<T>
    {
        private readonly ISocketStreamFormatter<T> _formatter;

        public SocketClient(int port, ISocketStreamFormatter<T> formatter)
        {
            LocalPort = port;
            _formatter = formatter;
        }

        public int LocalPort { get; }

        public Task Bind()
        {
            return Task.FromResult(0);
        }

        public void Dispose()
        {
        }

        public async Task SendMessage(IPAddress endpoint, T content)
        {
            using (var socket = new StreamSocket())
            {
                await socket.ConnectAsync(new HostName(endpoint.ToString()), LocalPort.ToString());
                var writer = new DataWriter(socket.OutputStream);
                await _formatter.Write(writer, content);
                await writer.StoreAsync();
            }
        }
    }
}