using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Kate.Commands;
using Kate.Commands.Socket;
using Kate.Maps;
using Kate.Types;

namespace Kate.IO
{
    public class SocketClient: AbstractClient
    {
        private string serverIp;
        private int serverPort = 0;
        private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
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
            init();
        }

        public override void executeMoves(ICollection<Move> moves)
        {
            Console.WriteLine("Client: Sending moves to the game server");

            var header = new byte[4];
            header[0] = (byte)'M';
            header[1] = (byte)'O';
            header[2] = (byte)'V';
            header[3] = (byte)moves.Count;
            socket.Send(header);

            foreach (var move in moves)
                sendBytes(move);

            Console.WriteLine("Client: Moves sent");

            listenToServer();
        }

        private void init()
        {
            Console.WriteLine("Client: Listening to server init messages");

            byte[] buffer = new byte[2048];
            var homeTile = new int[2] { 0, 0 }; // Keeping the home tile here to determine our side in the MAP command

            // Get first instruction
            while (socket.Available < 3) { Thread.Sleep(10); }
            socket.Receive(buffer, 3, SocketFlags.Partial);

            var command = toString(buffer.Take(3));
            if (command == "BYE")
                close(); // If the previous game wasn't a draw, the server will close instead of starting a new one
            else if (command != "SET")
                throw new ArgumentException("SET expected");
            else
            {
                // Get Map Size
                while (socket.Available < 2) { Thread.Sleep(10); }
                socket.Receive(buffer, 2, SocketFlags.Partial);

                OnMapSet(new MapSetEventArgs(buffer[1], buffer[0]));

                // Get Houses number
                while (socket.Available < 4) { Thread.Sleep(10); }
                socket.Receive(buffer, 4, SocketFlags.Partial);

                if (toString(buffer.Take(3)) != "HUM")
                    throw new ArgumentException("HUM expected");

                int humanNumber = (int)buffer[3];

                // Get Houses positions
                while (socket.Available < humanNumber * 2) { Thread.Sleep(50); }
                socket.Receive(buffer, humanNumber * 2, SocketFlags.Partial);

                var housesList = new List<Tile>();
                for (int i = 0; i < humanNumber * 2; i += 2)
                    housesList.Add(new Tile(buffer[i], buffer[i + 1], Owner.Humans, 0));

                OnMapInit(new MapUpdateEventArgs(housesList));

                // Get Home position
                while (socket.Available < 5) { Thread.Sleep(10); }
                socket.Receive(buffer, 5, SocketFlags.Partial);
                if (toString(buffer.Take(3)) != "HME")
                    throw new ArgumentException("HME expected");

                var home = new List<Tile>();
                homeTile[0] = buffer[3];
                homeTile[1] = buffer[4];
                home.Add(new Tile(buffer[3], buffer[4], Owner.Me, 0));

                OnMapInit(new MapUpdateEventArgs(home));

                // Get Map
                while (socket.Available < 4) { Thread.Sleep(10); }
                socket.Receive(buffer, 4, SocketFlags.Partial);
                if (toString(buffer.Take(3)) != "MAP")
                    throw new ArgumentException("MAP expected");
                int instructionNumber = buffer[3];

                while (socket.Available < instructionNumber * 5) { }
                socket.Receive(buffer, instructionNumber * 5, SocketFlags.Partial);

                var updateList = new List<Tile>();
                for (int i = 0; i < instructionNumber * 5; i += 5)
                {
                    int number;
                    Owner owner;
                    if (buffer[i + 2] != 0) // Humans on the tile
                    {
                        owner = Owner.Humans;
                        number = buffer[i + 2];
                    }
                    else
                    {
                        if (buffer[i] == homeTile[0] && buffer[i + 1] == homeTile[1]) // Determining our side
                        {
                            owner = Owner.Me;
                            this.side = buffer[i + 3] != 0 ? Side.Vampire : Side.Werewolf;
                        }
                        else
                            owner = Owner.Opponent;

                        if (buffer[i + 3] != 0) // Vampires on the tile
                            number = buffer[i + 3];
                        else if (buffer[i + 4] != 0) // Werewolves on the tile
                            number = buffer[i + 4];
                        else
                            throw new ArgumentException("Error, the tile cannot be empty");
                    }
                    updateList.Add(new Tile(buffer[i], buffer[i + 1], owner, number));
                }
                OnMapInit(new MapUpdateEventArgs(updateList));

                listenToServer();
            }
        }

        private void listenToServer()
        {
            Console.WriteLine("Client: Listening for updates");

            var buffer = new byte[2048];

            // Waiting for a server message
            while (socket.Available < 3) Thread.Sleep(10);
            socket.Receive(buffer, 3, SocketFlags.Partial);

            var command = toString(buffer.Take(3));
            if (command == "END")
            {
                OnGameEnd(new EventArgs());
                init(); // Getting ready for another game
            }
            else if (command != "UPD")
                throw new ArgumentException("Error, didn't get either END or UPD");
            else
            {
                Console.WriteLine("Client: Preparing for update");

                // UPD recieved
                while (socket.Available < 1) Thread.Sleep(10);
                socket.Receive(buffer, 1, SocketFlags.Partial);

                var updateList = new List<Tile>();
                int instructionNumber = buffer[0];

                if (instructionNumber > 0)
                {
                    while (socket.Available < instructionNumber * 5) Thread.Sleep(10);
                    socket.Receive(buffer, instructionNumber * 5, SocketFlags.Partial);

                    for (int i = 0; i < instructionNumber * 5; i += 5)
                    {
                        int number;
                        Owner owner;
                        if (buffer[i + 2] != 0) // Humans on the tile
                        {
                            owner = Owner.Humans;
                            number = buffer[i + 2];
                        }
                        else if (buffer[i + 3] != 0) // Vampires on the tile
                        {
                            owner = this.side == Side.Vampire ? Owner.Me : Owner.Opponent;
                            number = buffer[i + 3];
                        }
                        else if (buffer[i + 4] != 0) // Werewolves on the tile
                        {
                            owner = this.side == Side.Werewolf ? Owner.Me : Owner.Opponent;
                            number = buffer[i + 4];
                        }
                        else
                        {
                            owner = Owner.Neutral;
                            number = 0;
                        }
                        updateList.Add(new Tile(buffer[i], buffer[i + 1], owner, number));
                    }
                }
                OnMapUpdate(new MapUpdateEventArgs(updateList));
            }
        }

        public override void open()
        {
            Console.WriteLine("Client: Starting connection to the game server");
            socket.Connect(new IPEndPoint(IPAddress.Parse(serverIp), serverPort));
            Console.WriteLine("Client: Connected to game server");
        }

        public override void close()
        {
            socket.Close();
            socket.Dispose();
            OnGameDisconnection(new EventArgs());
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

