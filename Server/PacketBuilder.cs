using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PacketBuilder
    {
        MemoryStream Ms;
        public PacketBuilder()
        {
            Ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            Ms.WriteByte(opcode);
        }

        public void WriteMessage(string msg)
        {
            var msgLen = msg.Length;
            Ms.Write(BitConverter.GetBytes(msgLen));
            Ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        public byte[] GetPacketBytes()
        {
            return Ms.ToArray();
        }
    }
}
