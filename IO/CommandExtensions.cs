using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kate.Commands;

namespace Kate.IO
{
    public static class CommandExtensions
    {
        public static byte[] ToBytes(this ICommand command)
        {
            var type = command.GetType().ToString();
            if (type == typeof(DeclareName).ToString())
                return ((DeclareName)command).ToBytes();
            else if (type == typeof(Move).ToString())
                return ((Move)command).ToBytes();
            else
                return null;
        }

        public static byte[] ToBytes(this Move move)
        {
            var output = new byte[5];
            output[0] = (byte)move.Origin.X;
            output[1] = (byte)move.Origin.Y;
            output[2] = (byte)move.PopToMove;
            output[3] = (byte)move.Dest.X;
            output[4] = (byte)move.Dest.Y;
            return output;
        }

        public static byte[] ToBytes(this DeclareName declareName)
        {
            byte[] name = Encoding.ASCII.GetBytes(declareName.Name);
            byte[] output = new byte[3 + 1 + name.Length];
            output[0] = (byte)'N';
            output[1] = (byte)'M';
            output[2] = (byte)'E';
            output[3] = (byte)name.Length;

            for (int i = 0; i < name.Length; i++)
                output[i + 4] = name[i];

            return output;
        }
    }
}
