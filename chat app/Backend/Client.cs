using chat_app.Backend;
using System;
using System.Globalization;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Data;

namespace chat_app.Net
{

    class Client
    {
        TcpClient client;
        public PacketReader PacketReader;
        public event Action connectedEvent;
        public event Action privateMessageReceivedEvent;
        public event Action messageReceivedEvent;
        public event Action userDisconnectedEvent;
        public event Action switchStatusEvent;

        public Client()
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username, string ip)
        {
            if (!client.Connected)
            {
                client.Connect(ip, 7891);
                PacketReader = new PacketReader(client.GetStream());

                if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(ip))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    client.Client.Send(connectPacket.GetPacketBytes());
                }

                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 0:
                            connectedEvent?.Invoke();
                            break;

                        case 1:
                            privateMessageReceivedEvent?.Invoke();
                            break;

                        case 2:
                            messageReceivedEvent?.Invoke();
                            break;

                        case 3:
                            userDisconnectedEvent?.Invoke();
                            break;

                        case 4:
                            switchStatusEvent?.Invoke();
                            break;

                        default:
                            Console.WriteLine("unknown opcode");

                            break;
                    }
                }
            });
        }

        public void SendPrivateMessageToServer(string message)
        {
            /*var values = (object[])parameter;
            var message = (string)values[0];
            var recipent = (string)values[1];*/

            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(1);
            messagePacket.WriteMessage(message);
            client.Client.Send(messagePacket.GetPacketBytes());
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(2);
            messagePacket.WriteMessage(message);
            client.Client.Send(messagePacket.GetPacketBytes());
        }

        public void SwitchStatus()
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(4);
            client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
