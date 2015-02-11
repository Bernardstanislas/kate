using System;
using System.Text;

namespace Kate.Commands.Socket
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
            byte[] name = Encoding.ASCII.GetBytes(declareName.Name);
            byte[] output = new byte[3 + 1 + name.Length];
            output[0] = (byte)'N';
            output[1] = (byte)'M';
            output[2] = (byte)'E';
            output[3] = (byte)name.Length;
            
            for(int i = 0; i < name.Length; i++)
                output[i + 4] = name[i];

            return output;
        }
    }
}

