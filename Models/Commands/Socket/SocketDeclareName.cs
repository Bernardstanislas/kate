using System;

namespace Models.Commands.Socket
{
	public class SocketDeclareName: ISocketCommand
	{
		private readonly DeclareName DeclareName;

		public SocketDeclareName (DeclareName declareName)
		{
			if (declareName == null)
			{
				throw new ArgumentNullException ();
			} else
			{
				this.DeclareName = declareName;
			}
		}

		public byte[] toBytes ()
		{
			throw new NotImplementedException ();
		}
	}
}

