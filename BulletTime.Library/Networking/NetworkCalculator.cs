using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace BulletTime.Networking
{
    public class NetworkCalculator
    {
        private readonly byte _prefix;

        protected NetworkCalculator(IPAddress address, byte prefix)
        {
            LocalAddress = address;
            _prefix = prefix;
            Network = CalculateNetwork(address, prefix);
            BroadcastAddress = ConvertToIpAddress(ConvertToUInt32(Network) | (uint.MaxValue >> _prefix));
        }

        public IPAddress Network { get; }
        public IPAddress LocalAddress { get; private set; }
        public IPAddress BroadcastAddress { get; private set; }

        public IEnumerable<IPAddress> NetworkAddresses
        {
            get
            {
                var count = uint.MaxValue >> _prefix;
                var network32 = ConvertToUInt32(Network);

                // Skip the broadcast address
                count -= 1;

                while (count-- > 0)
                {
                    yield return ConvertToIpAddress(network32 | (count + 1));
                }
            }
        }

        public static NetworkCalculator Create()
        {
            var host = NetworkInformation.GetHostNames().First(h => h.Type == HostNameType.Ipv4 && h.IPInformation.PrefixLength.HasValue);
            return new NetworkCalculator(IPAddress.Parse(host.RawName), host.IPInformation.PrefixLength.Value);
        }

        private static uint ConvertToUInt32(IPAddress ip)
        {
            var ipBytes = ip.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                ipBytes = ipBytes.Reverse().ToArray();
            }

            return BitConverter.ToUInt32(ipBytes, 0);
        }

        private static IPAddress ConvertToIpAddress(uint ipInt)
        {
            var networkBytes = BitConverter.GetBytes(ipInt);

            if (BitConverter.IsLittleEndian)
            {
                networkBytes = networkBytes.Reverse().ToArray();
            }

            return new IPAddress(networkBytes);
        }

        private static IPAddress CalculateNetwork(IPAddress ip, byte prefix)
        {
            var ip32 = ConvertToUInt32(ip);
            var prefix32 = uint.MaxValue << (32 - prefix);
            return ConvertToIpAddress(ip32 & prefix32);
        }
    }
}