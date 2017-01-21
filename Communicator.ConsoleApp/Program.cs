using System;

namespace Communicator.ConsoleApp
{
    public class Program
    {
        private static ServerService _server = new ServerService();

        public static void Main(string[] args)
        {
            Console.WriteLine($"{DateTime.Now} - Server app started");
            _server.Start();
        }
    }
}