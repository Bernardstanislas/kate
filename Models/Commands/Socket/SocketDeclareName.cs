using System;

namespace Models.Commands.Socket
{
	public class SocketDeclareName: ISocketCommand
	{
		private readonly DeclareName DeclareName;

		public SocketDeclareName (DeclareName DeclareName)
		{
			this.DeclareName = DeclareName;
		}

		public byte[] toBytes ()
		{
			throw new NotImplementedException ();
		}
	}
}

