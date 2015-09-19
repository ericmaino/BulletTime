using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BulletTime.Models;
using BulletTime.Networking;
using BulletTime.RemoteControl;
using BulletTime.RemoteControl.Commands;

namespace BulletTime.Controllers
{
    public class ServerController
    {
        private readonly ConcurrentDictionary<string, RemoteCameraModel> _cameras;
        private readonly HeartBeatListener _heartbeat;
        private readonly CommandListener _listener;
        private readonly PingUtility _ping;
        private readonly CommandSender _sender;
        private readonly UploadListener _uploader;
        private bool _isInitilized;

        public ServerController()
        {
            CameraHeartbeat += async f => { await Task.Yield(); };
            _cameras = new ConcurrentDictionary<string, RemoteCameraModel>(StringComparer.OrdinalIgnoreCase);
            _ping = new PingUtility(50123);
            _sender = new CommandSender();
            _listener = new CommandListener();
            _listener.RegisterHandler<ReportingCommand, RemoteCameraModel>(HandleReporting);
            _uploader = new UploadListener();
            _heartbeat = new HeartBeatListener();
            _heartbeat.DataRecieved += async f => { await CameraHeartbeat(f); };
        }

        private IPAddress LocalAddress
        {
            get { return _listener.LocalHost; }
        }

        private int Port
        {
            get { return _listener.Port; }
        }

        public event Func<CameraHeartBeat, Task> CameraHeartbeat;

        private async Task Initialize()
        {
            if (!_isInitilized)
            {
                _isInitilized = true;
                await _listener.Initialize();
                await _uploader.Bind();
                await _heartbeat.Bind();
            }
        }

        public event Func<IEnumerable<RemoteCameraModel>, IEnumerable<RemoteResolutionModel>, Task> CamerasChanged;

        private async Task HandleReporting(RemoteCameraModel model)
        {
            _sender.RegisterEndPoint(model.IPAddress, model.Port);
            _cameras[model.IPAddress.ToString()] = model;
            await OnCamerasUpdated();
            await _sender.IssueCommand(new EnableHeartbeatCommand(LocalAddress, _heartbeat.Port));
        }

        public async Task UpdateResolution(RemoteResolutionModel resolution)
        {
            await _sender.IssueCommand(new UpdateResolutionCommand(resolution));
        }

        public async Task TriggerRecord()
        {
            await Initialize();
            await _sender.IssueCommand(new RecordCommand(LocalAddress, _uploader.Port));
        }

        public async Task RequestReport()
        {
            await Initialize();
            await _ping.Broadcast(LocalAddress, Port);
        }

        private async Task OnCamerasUpdated()
        {
            var action = CamerasChanged;

            if (action != null)
            {
                var comparer = new ResolutionComparer();
                var cameras = _cameras.Keys.OrderBy(x => x).Select(key => _cameras[key]).ToList();

                IEnumerable<RemoteResolutionModel> resolutions = null;

                foreach (var c in cameras)
                {
                    resolutions = resolutions ?? c.Resolutions;
                    resolutions = resolutions.Intersect(c.Resolutions, comparer);
                }

                await action(cameras, resolutions);
            }
        }
    }
}