using System;

namespace Models.Commands.Socket
{
	public class SocketMoves: ISocketCommand
	{
		private readonly Moves Moves;

		public SocketMoves (Moves moves)
		{
			if (moves == null)
			{
				throw new ArgumentNullException ();
			} else
			{
				this.Moves = moves;
			}
		}

		public byte[] toBytes ()
		{
			throw new NotImplementedException ();
		}
	}
}

