using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace chat_app.Backend
{
    class PacketReader : BinaryReader
    {
        private NetworkStream stream;
        public PacketReader(NetworkStream ns) : base(ns)
        {
            stream = ns;
        }

        public string ReadMessage()
        {
            var len = ReadInt32();
            byte[] msgBuffer = new byte[len];
            stream.Read(msgBuffer, 0, len);

            var msg = Encoding.ASCII.GetString(msgBuffer);
            return msg;
        }
    }
}
