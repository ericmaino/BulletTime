using System.Net;

namespace BulletTime.RemoteControl.Commands
{
    public class RecordCommand : PortAndAddressCommand<RecordCommand>
    {
        public RecordCommand()
        {
        }

        public RecordCommand(IPAddress address, int port)
            : base(address, port)
        {
        }
    }
}