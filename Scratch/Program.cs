using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Scratch
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var calc = new NetworkCalculator(IPAddress.Parse("10.121.208.214"), 23);
            Console.WriteLine(calc.Network);
            Console.WriteLine(calc.BroadcastAddress);
            Console.ReadLine();
            foreach (var ip in calc.NetworkAddresses)
            {
                Console.WriteLine(ip);
            }
        }
    }

    public class NetworkCalculator
    {
        private readonly byte _prefix;

        public NetworkCalculator(IPAddress address, byte prefix)
        {
            LocalAddress = address;
            _prefix = prefix;
            Network = CalculateNetwork(address, prefix);
            BroadcastAddress = GetIpAddress(GetUIntAddress(Network) | (uint.MaxValue >> _prefix));
        }

        public IPAddress Network { get; }
        public IPAddress LocalAddress { get; private set; }
        public IPAddress BroadcastAddress { get; }

        public IEnumerable<IPAddress> NetworkAddresses
        {
            get
            {
                var count = uint.MaxValue >> _prefix;
                var network32 = GetUIntAddress(Network);

                while (count-- > 0)
                {
                    yield return GetIpAddress(network32 | (count + 1));
                }
            }
        }

        private static uint GetUIntAddress(IPAddress ip)
        {
            var ipBytes = ip.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                ipBytes = ipBytes.Reverse().ToArray();
            }

            return BitConverter.ToUInt32(ipBytes, 0);
        }

        private static IPAddress GetIpAddress(uint ipInt)
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
            var ip32 = GetUIntAddress(ip);
            var prefix32 = uint.MaxValue << (32 - prefix);
            return GetIpAddress(ip32 & prefix32);
        }
    }
}