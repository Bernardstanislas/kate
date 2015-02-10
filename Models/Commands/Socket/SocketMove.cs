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
            var output = new byte[5];
            output[0] = (byte)Move.Origin.XCoordinate;
            output[1] = (byte)Move.Origin.YCoordinate;
            output[2] = (byte)Move.PopToMove;
            output[3] = (byte)Move.Dest.XCoordinate;
            output[4] = (byte)Move.Dest.YCoordinate;
            return output;
		}
	}
}

