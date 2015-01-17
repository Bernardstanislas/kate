using System;
using Models.Commands;

namespace Models.Commands.Socket
{
	public class SocketAttack: ISocketCommand
	{
		private readonly Attack Attack;

		public SocketAttack (Attack attack)
		{
			if (attack == null)
			{
				throw new ArgumentNullException ();
			} else
			{
				this.Attack = attack;
			}
		}

		public byte[] toBytes ()
		{
			throw new NotImplementedException ();
		}
	}
}

