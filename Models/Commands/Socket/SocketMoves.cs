using System;

namespace Models.Commands.Socket
{
	public class SocketMoves: ISocketCommand, ISocketAction
	{
		private readonly Moves Moves;

		public SocketMoves (Moves Moves)
		{
			this.Moves = Moves;
		}

		public byte[] toBytes ()
		{
			throw new NotImplementedException ();
		}
	}
}

