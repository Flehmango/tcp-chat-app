using chat_app.Backend;
using chat_app.Net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace chat_app.FrontEnd.ViewModel
{
    public class UserModel
    {
        public string username { get; set; }
        public string UID { get; set; }
        public string status { get; set; } = "Online";
    }

    public class MainViewModel
    {
        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        public string username { get; set; }
        public string ipAddress { get; set; }
        public string message { get; set; }

        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand SwitchStatusCommand { get; set; }

        private Client client;

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            client = new Client();
            client.connectedEvent += UserConnected;
            client.messageReceivedEvent += MessageReceived;
            client.userDisconnectedEvent += RemoveUser;
            client.switchStatusEvent += SwitchStatus;

            //connect to server--------------------------
            ConnectToServerCommand = new RelayCommand(o => {

                System.Net.IPAddress IP = null;
                bool isValidIp = System.Net.IPAddress.TryParse(ipAddress, out IP);

                if (!isValidIp)
                    ipAddress = "127.0.0.1";

                client.ConnectToServer(username, ipAddress);

            }, o => !string.IsNullOrEmpty(username));
            

            //send message--------------------------
            SendMessageCommand = new RelayCommand(o => {

                client.SendMessageToServer(message);
            }
            , o => !string.IsNullOrEmpty(message));


            //switch status--------------------------
            SwitchStatusCommand = new RelayCommand(o => {

                client.SwitchStatus();
            });
        }

        private void MessageReceived()
        {
            var msg = client.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add($"{msg}"));
        }

        private void RemoveUser()
        {
            var uid = client.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                username = client.PacketReader.ReadMessage(),
                UID = client.PacketReader.ReadMessage(),
                status = client.PacketReader.ReadMessage(),
            };

            if (!Users.Any(x => x.UID ==  user.UID))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }

        private void SwitchStatus()
        {
            var uid = client.PacketReader.ReadMessage();
            var user = Users.Where(x => x.UID == uid).FirstOrDefault();

            if (user.status == "Online")
                user.status = "Busy";
            else
                user.status = "Online";

            var replacement = new UserModel
            {
                username = user.username,
                UID = user.UID,
                status = user.status
            };

            Application.Current.Dispatcher.Invoke(() => {
                Users.Remove(user);
                Users.Add(replacement);
            });
        }
    }
}

