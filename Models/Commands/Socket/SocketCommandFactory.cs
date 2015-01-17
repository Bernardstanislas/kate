using System;
using System.Collections.Generic;

namespace Models.Commands.Socket
{
	public static class SocketCommandFactory
	{
		private static readonly IDictionary<String, Func<ICommand, ISocketCommand>> socketCommandsReference = new Dictionary<String, Func<ICommand, ISocketCommand>>()
		{
			{typeof(Attack).ToString(), (ICommand command) => new SocketAttack(command as Attack)},
			{typeof(DeclareName).ToString(), (ICommand command) => new SocketDeclareName(command as DeclareName)},
			{typeof(Moves).ToString(), (ICommand command) => new SocketMoves(command as Moves)}
		};

		public static ISocketCommand buildSocketCommand(ICommand command)
		{
			return socketCommandsReference [command.GetType ().ToString ()] (command);
		}
	}
}

