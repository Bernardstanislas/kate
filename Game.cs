using System;
using Engine;
using Engine.Socket;
using Models.Commands;
using Models.Map;
using Bot;

namespace kate
{
    class Game
    {
        public SocketClient Socket { get; set; }
        public Map Map { get; set; }

        public Game(string ipAddress, int portNumber)
        {
            Socket = new SocketClient(ipAddress, portNumber);
            Socket.MapSet += new MapSetEventHandler(this.createMap);
            var name = new DeclareName("KATE");
            Socket.declareName(name);
        }

        private void createMap(object sender, MapSetEventArgs args)
        {
            Map = new Map(args.width, args.height);
            var bot = new DumbBot(Socket, Map);
        }
    }
}
