using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using BulletTime.Networking.Formatters;

namespace BulletTime.Networking
{
    public class SocketListener<TFrame>
    {
        private readonly ISocketStreamFormatter<TFrame> _formatter;
        private readonly StreamSocketListener _listener;
        private readonly string _requestedPort;

        public SocketListener(ISocketStreamFormatter<TFrame> formatter, SocketQualityOfService qualityOfService = SocketQualityOfService.LowLatency)
            : this(formatter, 0, qualityOfService)
        {
        }

        public SocketListener(ISocketStreamFormatter<TFrame> formatter, int port, SocketQualityOfService qualityOfService = SocketQualityOfService.LowLatency)
        {
            _requestedPort = port.ToString();
            _formatter = formatter;
            _listener = new StreamSocketListener();
            _listener.Control.QualityOfService = qualityOfService;
            _listener.ConnectionReceived += ConnectionReceived;
            DataRecieved += async x => { await Task.Yield(); };
        }

        public int Port
        {
            get { return int.Parse(_listener.Information.LocalPort); }
        }

        public event Func<TFrame, Task> DataRecieved;

        private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                Debug.WriteLine($"Connection received {sender.Information.LocalPort} from {args.Socket.Information.RemoteAddress}");
                var reader = new DataReader(args.Socket.InputStream);
                await DataRecieved(await _formatter.Read(reader));
            }
            catch
            {
                Debug.WriteLine("Failed network message");
            }
        }

        public virtual async Task Bind()
        {
            if (string.IsNullOrEmpty(_listener.Information.LocalPort))
            {
                await _listener.BindServiceNameAsync(_requestedPort);
                Debug.WriteLine($"Listening on port {_listener.Information.LocalPort}");
            }
        }
    }
}