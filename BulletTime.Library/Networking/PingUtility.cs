using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BulletTime.Networking
{
    public class PingUtility
    {
        private readonly UdpClient _client;
        private readonly NetworkCalculator _network;

        public PingUtility(int port)
        {
            _client = new UdpClient(port);
            _network = NetworkCalculator.Create();
            _client.MessageRecieved += OnMessage;
            HandlePing += (ip, p) => Task.FromResult(0);
        }

        private void OnMessage(string payload)
        {
            var data = payload.Split(':');
            HandlePing(IPAddress.Parse(data.First()), int.Parse(data.Last()));
        }

        public event Func<IPAddress, int, Task> HandlePing;

        public async Task Broadcast(IPAddress localAddress, int port)
        {
            foreach (var address in _network.NetworkAddresses)
            {
                await _client.SendMessage(address, string.Join(":", localAddress, port));
            }
        }

        public async Task Bind()
        {
            await _client.Bind();
        }
    }
}