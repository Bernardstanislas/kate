using System;
using System.Text;

namespace Models.Commands.Socket
{
	public class SocketDeclareName: ISocketCommand
	{
		private readonly DeclareName declareName;

		public SocketDeclareName (DeclareName declareName)
		{
			if (declareName == null)
				throw new ArgumentNullException();
            else
				this.declareName = declareName;
		}

		public byte[] toBytes()
		{
            return Encoding.ASCII.GetBytes(declareName.Name);
		}
	}
}

