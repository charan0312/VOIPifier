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

namespace VOIPIfier.UI
{
    /// <summary>
    /// Interaction logic for Messages.xaml
    /// </summary>
    public partial class Messages : UserControl
    {
        public Messages()
        {
            InitializeComponent();
        }

        public void AddMessage(MessageDialog dialog)
        {
            MsgsHolder.Children.Add(dialog);
        }

        public void ClearMessages()
        {
            MsgsHolder.Children.Clear();
        }

        private void SendMessage()
        {
            if (Message.Text.Trim() == String.Empty) return; // Do not send empty text
            Network.Packets.Message msg = new Network.Packets.Message();
            msg.MessageString = Message.Text.ToString();
            Message.Clear();

            msg.IP = "192.168.0.199";
            msg.Port = Network.Backend.PORT;
            msg.Send();
        }

        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SendMessage();
        }

        private void Message_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Return))
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    //Message.Text += System.Environment.NewLine;
                    return;
                }
                else
                {
                    SendMessage();
                }
            }

            if (Message.Text.Trim() == String.Empty)
            {
                SendSvg.Fill = new SolidColorBrush(Color.FromRgb(201, 201, 201));
            }
            else if (SendSvg != null)
            {
                SendSvg.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }
    }
}
