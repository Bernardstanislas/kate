using System;

using Kate.Bots;
using Kate.IO;

namespace kate
{
    class Program
    {
        public static void Main(string[] args)
        {
            string ipAddress = "";
            int port = 0;

            foreach (var arg in args)
            {
                var content = arg.Split('=');
                if (content[0] == "-ip")
                    ipAddress = content[1];
                else if (content[0] == "-port")
                    port = Convert.ToInt32(content[1]);
            }

            if (ipAddress == "" || port == 0)
                throw new ArgumentException("Missing IP or port number");

            Console.WriteLine("Connecting to " + ipAddress + " on port " + port.ToString());
            var socket = new SocketClient(ipAddress, port);
            new DumbBot(socket, "KATE");
            Console.WriteLine("KATE has started");
            Console.ReadKey(true);
        }
    }
}
