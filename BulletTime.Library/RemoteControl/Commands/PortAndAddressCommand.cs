using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;

namespace BulletTime.RemoteControl.Commands
{
    public abstract class PortAndAddressCommand<T> : BaseCommand<T>
    {
        protected PortAndAddressCommand()
        {
        }

        protected PortAndAddressCommand(IPAddress address, int port)
        {
            AddressAsBytes = address.GetAddressBytes().ToList();
            Port = port;
        }

        [JsonIgnore]
        public IPAddress Address
        {
            get { return new IPAddress(AddressAsBytes.ToArray()); }
        }

        public IEnumerable<byte> AddressAsBytes { get; set; }
        public int Port { get; set; }

        public override string Serialize()
        {
            return Serialize(this);
        }
    }
}