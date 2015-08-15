using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using BulletTime.Models;
using BulletTime.Networking;
using BulletTime.Networking.Formatters;

namespace BulletTime.RemoteControl
{
    public class CommandListener
    {
        private readonly IDictionary<string, Func<StringReader, Task>> _handlers;
        private readonly SocketListener<CommandJson> _listener;

        public CommandListener()
            : this(0)
        {
        }

        public CommandListener(int port)
        {
            LocalHost = NetworkCalculator.Create().LocalAddress;
            _listener = new SocketListener<CommandJson>(new JsonCommandFormatter(), port);
            _listener.DataRecieved += HandleMessage;
            _handlers = new Dictionary<string, Func<StringReader, Task>>(StringComparer.OrdinalIgnoreCase);
        }

        public IPAddress LocalHost { get; }

        public int Port
        {
            get { return _listener.Port; }
        }

        private async Task HandleMessage(CommandJson data)
        {
            var handler = _handlers[data.CommandName];
            await handler(new StringReader(data.Command));
        }

        public void RegisterHandler<T, TPayload>(Func<TPayload, Task> handler) where T : IRemoteCameraCommand<TPayload>, new()
        {
            var command = new T();
            _handlers.Add(command.Name, async reader =>
            {
                var payload = command.Parse(reader);
                await handler(payload);
            });
        }

        internal async Task Initialize()
        {
            await _listener.Bind();
        }
    }
}