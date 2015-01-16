using System;
using Models.Commands;

namespace Models.Commands.Socket
{
	public class SocketAttack: ISocketCommand, ISocketAction
	{
		private readonly Attack Attack;

		public SocketAttack (Attack Attack)
		{
			this.Attack = Attack;
		}

		public byte[] toBytes ()
		{
			throw new NotImplementedException ();
		}
	}
}

