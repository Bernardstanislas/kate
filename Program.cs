using System;

using Kate.Bots;
using Kate.IO;

namespace kate
{
    class Program
    {
        public static void Main()
        {
            Console.WriteLine("KATE is starting...");
            var socket = new SocketClient("127.0.0.1", 5555);
            new DumbBot(socket, "KATE");
            Console.ReadKey(true);
        }
    }
}
