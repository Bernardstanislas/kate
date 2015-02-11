using System;

namespace Kate.Commands.Socket
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
            output[0] = (byte)Move.Origin.X;
            output[1] = (byte)Move.Origin.Y;
            output[2] = (byte)Move.PopToMove;
            output[3] = (byte)Move.Dest.X;
            output[4] = (byte)Move.Dest.Y;
            return output;
        }
    }
}

