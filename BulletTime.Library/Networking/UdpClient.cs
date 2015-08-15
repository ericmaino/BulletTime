using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace BulletTime.Networking
{
    public class UdpClient : INetworkClient<string>
    {
        private readonly DatagramSocket _datagram;
        private readonly int _port;
        private readonly Socket _socket;

        public UdpClient()
            : this(0)
        {
        }

        public UdpClient(int port)
        {
            _port = port;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _datagram = new DatagramSocket();
            _datagram.MessageReceived += UdpMessageReceived;
        }

        public HostName LocalHost
        {
            get { return _datagram.Information.LocalAddress; }
        }

        public int LocalPort
        {
            get { return int.Parse(_datagram.Information.LocalPort); }
        }

        public void Dispose()
        {
            _datagram.Dispose();
            _socket.Dispose();
        }

        public async Task SendMessage(IPAddress address, string message)
        {
            await SendMessage(address, Encoding.UTF8.GetBytes(message));
        }

        public async Task Bind()
        {
            if (string.IsNullOrEmpty(_datagram.Information.LocalPort))
            {
                await _datagram.BindServiceNameAsync(_port.ToString());
            }
        }

        public async Task SendMessage(IPAddress address, byte[] payload)
        {
            await SendMessage(address, payload, false);
        }

        public async Task SendMessageAndWait(IPAddress address, byte[] payload)
        {
            await SendMessage(address, payload, true);
        }

        private async Task SendMessage(IPAddress address, byte[] payload, bool waitForResponse)
        {
            await Task.Run(() =>
            {
                var waitEvent = new ManualResetEvent(true);

                var socketEventArg = new SocketAsyncEventArgs
                {
                    RemoteEndPoint = new IPEndPoint(address, _port)
                };

                if (waitForResponse)
                {
                    waitEvent.Reset();

                    socketEventArg.Completed += (sender, args) => { waitEvent.Set(); };
                }

                socketEventArg.SetBuffer(payload, 0, payload.Length);
                _socket.SendToAsync(socketEventArg);
                waitEvent.WaitOne();
            });
        }

        public event Action<string> MessageRecieved;

        private void UdpMessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
        {
            var action = MessageRecieved;

            if (action != null)
            {
                using (var reader = new StreamReader(args.GetDataStream().AsStreamForRead()))
                {
                    action(reader.ReadToEnd());
                }
            }
        }
    }
}