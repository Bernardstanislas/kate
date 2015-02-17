using System;
using System.Collections.Generic;

namespace Kate.Commands.Socket
{
    public static class SocketCommandFactory
    {
        private static readonly IDictionary<String, Func<ICommand, ISocketCommand>> socketCommandsReference = new Dictionary<String, Func<ICommand, ISocketCommand>>()
        {
            {typeof(DeclareName).ToString(), (ICommand command) => new SocketDeclareName(command as DeclareName)},
            {typeof(Move).ToString(), (ICommand command) => new SocketMove(command as Move)}
        };

        public static ISocketCommand buildSocketCommand(ICommand command)
        {
            return socketCommandsReference [command.GetType().ToString()] (command);
        }
    }
}

