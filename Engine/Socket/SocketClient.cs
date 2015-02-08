using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Models;
using Models.Commands;
using Models.Commands.Socket;
using Models.Map;
using System.Collections.Generic;

namespace Engine.Socket
{
	public class SocketClient: AbstractClient
	{
        private string serverIp;
        private int serverPort = 0;
        private System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private bool isPlaying = false;

        private Side side = Side.Unknown; // We need to store if we're playing werewolves or vampires, because the server won't explicity tell us

        public SocketClient(string serverIp, int serverPort)
        {
            this.serverIp = serverIp;
            this.serverPort = serverPort;

            open();
        }

        public override void declareName(DeclareName declareName)
        {
            sendBytes(declareName);

            initialize();
            listenToServer();
        }

        public override void executeMoves(ICollection<Move> moves)
        {
            foreach (var move in moves)
                sendBytes(move);

            listenToServer();
        }

        private void initialize()
        {
            byte[] buffer = new byte[2048];
            var homeTile = new int[2] { 0, 0 }; // Keeping the home tile here to determine our side in the MAP command

            // Get Map size
            while (socket.Available < 5) { Thread.Sleep(10); }
            socket.Receive(buffer, 5, SocketFlags.Partial);

            if (toString(buffer.Take(3)) != "SET")
                throw new ArgumentException("SET expected");

            OnMapSet(new MapSetEventArgs(buffer[4], buffer[3])); // Fire MapSet event

            // Get Houses number
            while (socket.Available < 4) { Thread.Sleep(10); }
            socket.Receive(buffer, 4, SocketFlags.Partial);

            if (toString(buffer.Take(3)) != "HUM")
                throw new ArgumentException("HUM expected");

            int humanNumber = (int)buffer[3];

            // Get Houses positions
            while (socket.Available < humanNumber * 2) { Thread.Sleep(10); }
            socket.Receive(buffer, humanNumber * 2, SocketFlags.Partial);

            var housesList = new List<IMapUpdater>();
            for (int i = 0; i < humanNumber; i++)
                housesList.Add(new MapUpdater(new Tile(buffer[i], buffer[i + 1], Player.Humans, 0))); // Should have factories here
            OnHousesSet(new MapUpdateEventArgs(housesList));

            // Get Home position
            while (socket.Available < 5) { Thread.Sleep(10); }
            socket.Receive(buffer, 5, SocketFlags.Partial);
            if (toString(buffer.Take(3)) != "HME")
                throw new ArgumentException("HME expected");
            var home = new List<IMapUpdater>();
            homeTile[0] = buffer[3];
            homeTile[1] = buffer[4];
            home.Add(new MapUpdater(new Tile(buffer[3], buffer[4], Player.Me, 0))); // Should have factories here
            OnHomeSet(new MapUpdateEventArgs(home));

            // Get Map
            while (socket.Available < 4) { Thread.Sleep(10); }
            socket.Receive(buffer, 4, SocketFlags.Partial);
            if (toString(buffer.Take(3)) != "MAP")
                throw new ArgumentException("MAP expected");
            int instructionNumber = buffer[3];

            while (socket.Available < instructionNumber * 5) { }
            socket.Receive(buffer, buffer[3] * 5, SocketFlags.Partial);

            var updateList = new List<IMapUpdater>();
            for (int i = 0; i < instructionNumber; i++)
            {
                int number;
                Player side;
                if (buffer[i + 2] != 0) // Humans on the tile
                {
                    side = Player.Humans;
                    number = buffer[i + 2];
                }
                else
                {
                    if (buffer[i] == homeTile[0] && buffer[i + 1] == homeTile[1]) // Determining our side
                    {
                        side = Player.Me;
                        this.side = buffer[i + 3] != 0 ? Side.Vampire : Side.Werewolf;
                    }
                    else
                        side = Player.Opponent;

                    if (buffer[i + 3] != 0) // Vampires on the tile
                        number = buffer[i + 3];
                    else if (buffer[i + 4] != 0) // Werewolves on the tile
                        number = buffer[i + 4];
                    else
                        throw new ArgumentException("Error, the tile cannot be empty");
                }
                updateList.Add(new MapUpdater(new Tile(buffer[i], buffer[i + 1], side, number))); // Should have factories here
            }
            OnMapInitialization(new MapUpdateEventArgs(updateList));
        }

        private void listenToServer()
        {
            var buffer = new byte[2048];

            // Waiting for a server message
            while (socket.Available < 3) Thread.Sleep(10);
            socket.Receive(buffer, 3, SocketFlags.Partial);

            var command = toString(buffer.Take(3));
            if (command == "END")
                close(); // Game is ending, we close the connection
            if (command != "UPD")
                throw new ArgumentException("Error, didn't get either END or UPD");

            // UPD recieved
            while (socket.Available < 1) Thread.Sleep(10);
            socket.Receive(buffer, 1, SocketFlags.Partial);
            if (buffer[0] < 1)
                throw new ArgumentException("Error, no updates sent");

            int instructionNumber = buffer[0];
            while (socket.Available < instructionNumber * 5) Thread.Sleep(10);
            socket.Receive(buffer, instructionNumber * 5, SocketFlags.Partial);

            var updateList = new List<IMapUpdater>();
            for (int i = 0; i < instructionNumber; i++)
            {
                int number;
                Player side;
                if (buffer[i + 2] != 0) // Humans on the tile
                {
                    side = Player.Humans;
                    number = buffer[i + 2];
                }
                else if (buffer[i + 3] != 0) // Vampires on the tile
                {
                    side = this.side == Side.Vampire ? Player.Me : Player.Opponent;
                    number = buffer[i + 3];
                }
                else if (buffer[i + 4] != 0) // Werewolves on the tile
                {
                    side = this.side == Side.Werewolf ? Player.Me : Player.Opponent;
                    number = buffer[i + 4];
                }
                else
                    throw new ArgumentException("Error, the tile cannot be empty");
                updateList.Add(new MapUpdater(new Tile(buffer[i], buffer[i + 1], side, number))); // Should have factories here
            }
            OnMapUpdate(new MapUpdateEventArgs(updateList));
        }

		public override void open()
		{
            socket.Connect(new IPEndPoint(IPAddress.Parse(serverIp), serverPort));
            isPlaying = true;
		}

		public override void close()
		{
            isPlaying = false;
            socket.Close();
            socket.Dispose();
		}

		private void sendBytes(ICommand commandToSend)
		{
            socket.Send(SocketCommandFactory.buildSocketCommand(commandToSend).toBytes());
		}

        private string toString(IEnumerable<byte> bytes)
        {
            return Encoding.ASCII.GetString(bytes.ToArray());
        }
	}
}

