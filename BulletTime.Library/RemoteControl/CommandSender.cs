using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using BulletTime.Models;
using BulletTime.Networking;
using BulletTime.Networking.Formatters;

namespace BulletTime.RemoteControl
{
    public class CommandSender : IDisposable
    {
        private readonly JsonCommandFormatter _formatter;
        private readonly ConcurrentDictionary<IPAddress, int> _knownEndpoints;
        private readonly NetworkCalculator _network;

        public CommandSender()
        {
            _network = NetworkCalculator.Create();
            _knownEndpoints = new ConcurrentDictionary<IPAddress, int>();
            _formatter = new JsonCommandFormatter();
        }

        public void Dispose()
        {
        }

        public async Task IssueCommand(IRemoteCameraCommand command)
        {
            var bad = new Queue<IPAddress>();

            foreach (var ip in _knownEndpoints)
            {
                try
                {
                    await IssueCommand(command, ip.Key, ip.Value);
                }
                catch
                {
                    bad.Enqueue(ip.Key);
                }
            }

            foreach (var badIp in bad)
            {
                int unused;
                _knownEndpoints.TryRemove(badIp, out unused);
            }
        }

        public async Task IssueCommand(IRemoteCameraCommand command, IPAddress endpoint, int port)
        {
            using (var socket = new StreamSocket())
            {
                await socket.ConnectAsync(new HostName(endpoint.ToString()), port.ToString());
                var writer = new DataWriter(socket.OutputStream);
                await _formatter.Write(writer, new CommandJson
                {
                    CommandName = command.Name,
                    Command = command.Serialize()
                });
                await writer.StoreAsync();
            }
        }

        public void RegisterEndPoint(IPAddress endpoint, int port)
        {
            _knownEndpoints[endpoint] = port;
        }
    }
}