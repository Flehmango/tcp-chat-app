using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace chat_app.FrontEnd.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate

            {
                action.Invoke();
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }

        private void btnClearTextboxes(object sender, RoutedEventArgs e)
        {
            DelayAction(1, new Action(() => {
                textboxUsername.Clear();
                textboxIP.Clear();
                textboxMessage.Clear();
            }));
        }

        private void toPrivateChatClick(object sender, RoutedEventArgs e)
        {

        }

        private void toMainChatClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
