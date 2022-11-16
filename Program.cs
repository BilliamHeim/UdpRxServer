using System;

namespace PunchServerUDPReceiver
{
    class Program
    {
        public static void TestCallback(string data)
        {
            Console.WriteLine(data);
        }
        static void Main(string[] args)
        {
            UDPListener s = new UDPListener(TestCallback);
            s.Server("rx.server.ip.address", 27000); //Set port as desired
            Console.WriteLine("Awaiting Messages...");

            Console.ReadKey();
        }
    }
}