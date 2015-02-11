using System;

namespace Kate.Commands.Socket
{
	// A command is any message send to the engine server through the engine client
	public interface ISocketCommand: ICommand
	{
		byte[] toBytes();
	}
}

