using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using BulletTime.Controllers;
using BulletTime.Models;
using BulletTime.Networking;
using BulletTime.RemoteControl.Commands;
using BulletTime.Storage;
using BulletTime.ViewModels;

namespace BulletTime.RemoteControl
{
    public class CameraClientRemoteControl
    {
        private readonly ClientController _camera;
        private readonly HeartbeatTimer _heartbeat;
        private readonly CommandListener _listener;
        private readonly CameraClientViewModel _model;
        private readonly PingUtility _ping;

        public CameraClientRemoteControl(CameraClientViewModel model, ClientController camera)
        {
            _model = model;
            _camera = camera;
            _heartbeat = new HeartbeatTimer(() => Task.FromResult(0));
            _ping = new PingUtility(50123);
            _ping.HandlePing += ReportCamera;
            _listener = new CommandListener();
            _listener.RegisterHandler<RecordCommand, RecordCommand>(RemoteRecord);
            _listener.RegisterHandler<EnableHeartbeatCommand, EnableHeartbeatCommand>(EnableHeartbeat);
            _listener.RegisterHandler<UpdateResolutionCommand, RemoteResolutionModel>(UpdateResolution);
        }

        private async Task EnableHeartbeat(EnableHeartbeatCommand command)
        {
            _heartbeat.UpdateCallback(async () =>
            {
                using (var socket = new StreamSocket())
                {
                    await socket.ConnectAsync(new HostName(command.Address.ToString()), command.Port.ToString());
                    var formatter = new HeartBeatListener.Formatter();
                    var writer = new DataWriter(socket.OutputStream);
                    var buffer = await StorageIO.ReadIntoBuffer(await _camera.Snapshot());
                    await formatter.Write(writer, new CameraHeartBeat(_listener.LocalHost.ToString(), buffer));
                    await writer.StoreAsync();
                }
            });
            _heartbeat.Enable(TimeSpan.FromSeconds(3));
            await Task.Yield();
        }

        private async Task ReportCamera(IPAddress ip, int port)
        {
            using (var sender = new CommandSender())
            {
                var reporting = new ReportingCommand(_listener.LocalHost, _listener.Port, _camera.GetSupportedResolutions());
                await sender.IssueCommand(reporting, ip, port);
            }
        }

        private async Task RemoteRecord(RecordCommand command)
        {
            _heartbeat.Disable();
            var uploader = new FrameUploader(_listener.LocalHost.ToString(), command.Address, command.Port);
            _model.FramesProcessed += uploader.Upload;
            uploader.UploadComplete += () => { _model.FramesProcessed -= uploader.Upload; };
            await Dispatch(() => _model.Record.Execute(null));
            _heartbeat.Enable(TimeSpan.FromSeconds(3));
        }

        private async Task UpdateResolution(RemoteResolutionModel resolution)
        {
            if (resolution == null)
            {
                return;
            }

            await Dispatch(() =>
            {
                var found = ((IEnumerable<VideoCameraResolutionModel>) _model.Resolutions.Source).FirstOrDefault(x => x.ToString() == resolution.Name);

                if (found != null)
                {
                    _model.SelectedResolution.Value = found;
                }
            });
        }

        private async Task Dispatch(DispatchedHandler handler)
        {
            await _model.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);
        }

        public async Task Initialize()
        {
            await _listener.Initialize();
            await _ping.Bind();
        }
    }
}