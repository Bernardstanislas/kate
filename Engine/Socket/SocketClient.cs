using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Models.Commands;
using Models.Commands.Socket;
using System.Collections.Generic;

namespace Engine.Socket
{
	public class SocketClient: AbstractClient
	{
        private string serverIp;
        private int serverPort;
        private System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		public SocketClient (string serverIp, int serverPort)
		{
            this.serverIp = serverIp;
            this.serverPort = serverPort;

            byte[] buffer = new byte[2048];
            
            // SET
            while (socket.Available < 5) { }            
            socket.Receive(buffer, 5, SocketFlags.Partial);

            if (toString(buffer.Take(3)) != "SET")
                throw new Exception("Erreur, attendu: SET");
            else
                MapSet += new MapSetEventHandler(buffer[3], buffer[4]);
            //Utilisez buffer[3] (lignes) et buffer[4] (colonnes) pour créer une grille
		}

		public override void open()
		{
            socket.Connect(new IPEndPoint(IPAddress.Parse(serverIp), serverPort));
		}

		public override void close()
		{
            socket.Close();
            socket.Dispose();
		}

		public override void declareName(DeclareName declareName)
		{
			sendBytes(SocketCommandFactory.buildSocketCommand(declareName));
		}

		public override void executeMoves(ICollection<Move> moves)
		{
            foreach (var move in moves)
			    sendBytes(SocketCommandFactory.buildSocketCommand(move));
		}

		private void sendBytes(ISocketCommand socketCommandToSend)
		{
            socket.Send(socketCommandToSend.toBytes());
		}

        private string toString(IEnumerable<byte> bytes)
        {
            return Encoding.ASCII.GetString(bytes.ToArray());
        }
	}
}

