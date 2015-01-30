using System;

namespace Models.Commands.Socket
{
	public class SocketMove: ISocketCommand
	{
		private readonly Move Move;

		public SocketMove(Move move)
		{
			if (move == null)
				throw new ArgumentNullException();
			else
				this.Move = move;
		}

		public byte[] toBytes()
		{
			throw new NotImplementedException();
		}
	}
}

