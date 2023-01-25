using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace Server
{
    class Program
    {
        static List<Client> users;
        static TcpListener listener;
        static void Main(string[] args)
        {
            users = new List<Client>();
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 7891);
            listener.Start();

            Console.WriteLine("Listening for users on port 7891");
            while (true)
            {
                var client = new Client(listener.AcceptTcpClient());
                users.Add(client);

                BroadcastConnection();
            }
        }

        static void BroadcastConnection()
        {
            foreach (var user in users)
            {
                foreach (var usr in users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(0);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    broadcastPacket.WriteMessage(usr.Status);

                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }

        public static void BroadcastPrivateMessage(string message)
        {
            foreach (var user in users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(1);
                msgPacket.WriteMessage(message);
                //if (username is not null) msgPacket.WriteMessage(username);

                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastMessage(string message)
        {
            foreach (var user in users)
            {
                var msgPacket = new PacketBuilder();
                msgPacket.WriteOpCode(2);
                msgPacket.WriteMessage(message);
                //if (username is not null) msgPacket.WriteMessage(username);

                user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
            users.Remove(disconnectedUser);

            foreach (var user in users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(3);
                broadcastPacket.WriteMessage(uid);

                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage($"{disconnectedUser.Username} disconnected.");
        }

        public static void BroadcastChangeStatus(string uid)
        {
            var switchedUser = users.Where(x => x.UID.ToString() == uid).FirstOrDefault();

            if (switchedUser.Status == "Online")
                switchedUser.Status = "Busy";
            else
                switchedUser.Status = "Online";

            foreach (var user in users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(4);
                broadcastPacket.WriteMessage(uid);

                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }
        }
    }
}
