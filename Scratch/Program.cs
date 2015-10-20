using System;
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
}