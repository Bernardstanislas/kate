using System;
using System.Collections.Generic;

using Kate.Bots;
using Kate.IO;
using Kate.Maps;
using Kate.Types;

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

            IClient client;

            if (ipAddress == "" || port == 0)
            {
                Console.WriteLine("Missing IP or port number: Starting test client");
                var tiles = new List<Tile>()
                {
                    new Tile(1, 2, Owner.Me, 5),
                    new Tile(3, 2, Owner.Opponent, 5),
                    new Tile(2, 3, Owner.Humans, 2),
                    new Tile(1, 1, Owner.Humans, 3),
                    new Tile(2, 2, Owner.Humans, 1)
                };
                client = new TestClient(5, 5, tiles);
            }
            else
            {
                Console.WriteLine("Connecting to " + ipAddress + " on port " + port.ToString());
                client = new SocketClient(ipAddress, port);
            }

            Console.WriteLine("KATE has started");
            new RandomBot(client, "KATE");
            Console.ReadKey();
        }
    }
}
