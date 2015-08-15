using System.Collections.Generic;
using System.Net;
using BulletTime.Models;

namespace BulletTime.RemoteControl.Commands
{
    public class ReportingCommand : BaseCommand<RemoteCameraModel>
    {
        private readonly RemoteCameraModel _payload;

        public ReportingCommand()
        {
        }

        public ReportingCommand(IPAddress endPoint, int port, IEnumerable<VideoCameraResolutionModel> resolutions)
        {
            _payload = new RemoteCameraModel(endPoint, port, resolutions);
        }

        public override string Serialize()
        {
            return base.Serialize(_payload);
        }
    }
}