using System;
using Models.Commands;
using Models.Commands.Socket;


namespace Engine.Socket
{
	public class SocketClient: AbstractClient
	{

		public SocketClient (String serverIpString, int serverPort)
		{
			//TODO code a default constructor
			throw new NotImplementedException();
		}

		public override void open()
		{
			//TODO code the opening routine
			throw new NotImplementedException();
		}

		public override void close()
		{
			//TODO code the closing routine
			throw new NotImplementedException();
		}

		public override void declareName(DeclareName declareName)
		{
			ISocketCommand commandToSend = SocketCommandFactory.buildSocketCommand (declareName);
			sendBytes (commandToSend);
		}

		public override void executeAction(IAction action)
		{
			ISocketCommand commandToSend = SocketCommandFactory.buildSocketCommand (action);
			sendBytes (commandToSend);
		}

		private void sendBytes(ISocketCommand socketCommandToSend)
		{
			byte[] bytesToSend = socketCommandToSend.toBytes ();
			//TODO send the bytes
			throw new NotImplementedException ();
		}
	}
}

