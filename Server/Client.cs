using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Client
    {
        public string Username { get; set; }
        public Guid UID { get; set; }
        public string Status { get; set; } = "Online";
        public TcpClient ClientSocket { get; set; }

        PacketReader packetReader;
        public Client(TcpClient client)
        {
            ClientSocket = client;
            UID = Guid.NewGuid();

            packetReader = new PacketReader(ClientSocket.GetStream());
            var opcode = packetReader.ReadByte();
            Username = packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: {Username} has connected.");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 2:
                            var msg = packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}] {Username}: {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now.ToString("HH:mm")}] {Username}: {msg}");

                            break;

                        case 4:
                            Program.BroadcastChangeStatus(UID.ToString());
                            break;

                        default:

                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{UID.ToString()}]: Client disconnected.");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();

                    break;
                }
            }
        }
    }
}
