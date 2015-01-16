using System;
using Models.Commands;
using Models.Commands.Socket;

namespace Engine.Socket
{
	public class SocketClient: AbstractClient
	{
		public SocketClient ()
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
			throw new NotImplementedException();
		}

		public override void executeAction(IAction action)
		{
			throw new NotImplementedException();
			ISocketAction socketAction = action as ISocketAction;
			if (socketAction != null)
			{
				//TODO send the socket action
			}
		}
	}
}

