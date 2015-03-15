using System;
using System.Collections.Generic;

using Kate.Bots;
using Kate.Bots.Workers;
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

                Console.WriteLine("Missing IP or port number: Starting test client");
                var tiles = new List<Tile>()
                {
                    new Tile(5, 4, Owner.Me, 3),
                    new Tile(2, 3, Owner.Opponent, 3),
                    new Tile(2, 2, Owner.Humans, 1),
                    new Tile(4, 4, Owner.Humans, 1)
                };
                client = new TestClient(6, 5, tiles);
            Console.WriteLine("KATE has started");
            var bot = new AlphaBetaBot(client, "KATE", Worker.DefaultWorker, 1950);
            bot.Start();
            Console.ReadKey();
        }
    }
}
