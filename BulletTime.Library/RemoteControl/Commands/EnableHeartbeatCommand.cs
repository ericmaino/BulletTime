using System.Net;

namespace BulletTime.RemoteControl.Commands
{
    public class EnableHeartbeatCommand : PortAndAddressCommand<EnableHeartbeatCommand>
    {
        public EnableHeartbeatCommand()
        {
        }

        public EnableHeartbeatCommand(IPAddress address, int port)
            : base(address, port)
        {
        }
    }
}