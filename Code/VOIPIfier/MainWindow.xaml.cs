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

namespace VOIPIfier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow INSTANCE;
        private static Dictionary<String, UI.Messages> dialogs = new Dictionary<string, UI.Messages>();

        public MainWindow()
        {
            InitializeComponent();
            User.User.RealName = "Tim Panajott";
            INSTANCE = this;

            Network.Backend.Listen();

            Username.Content = User.User.RealName;

            Network.Packets.Message update = new Network.Packets.Message();
            update.MessageString = "Hey, sup?";
            update.IP = "192.168.0.199";
            update.Port = Network.Backend.PORT;
            update.Send();

            /*Network.Packets.CallRequest req = new Network.Packets.CallRequest();
            req.IP = "192.168.0.198";
            req.Port = Network.Backend.PORT;
            req.Send();*/

            Sound.Backend.Init();

            //Sound.Backend.GetRecordByte();
            Network.VoiceChannel channel = new Network.VoiceChannel();
            channel.RegisterListener("192.168.0.199");

            channel.StartBroadcasting();

        }

        internal static void AddMessage(string user, string message)
        {
            UI.MessageDialog dialog = new UI.MessageDialog();
            dialog.User = user;
            dialog.Message = message;
            var par = GetMessageDialog(user);
            par.AddMessage(dialog);

            GetUserView(user).RecentMessage = message;
        }

        public static UI.Messages GetMessageDialog(String user)
        {
            if (dialogs.ContainsKey(user))
            {
                return dialogs[user];
            }

            dialogs.Add(user, new UI.Messages());
            INSTANCE.MessaagesGrid.Children.Clear();
            INSTANCE.MessaagesGrid.Children.Add(dialogs[user]);
            return dialogs[user];
        }

        public static User.UserView GetUserView(String username)
        {
            for (int i = 0; i < INSTANCE.ContactsPanel.Children.Count; i++)
            {
                if (INSTANCE.ContactsPanel.Children[i] is User.UserView)
                {
                    var view = (User.UserView)INSTANCE.ContactsPanel.Children[i];

                    if (view.Username == username)
                    {
                        return view;
                    }
                }
            }


            User.UserView nview = null;
            INSTANCE.Dispatcher.Invoke(new Action(() =>
            {
                nview = new User.UserView();
                nview.Username = username;
                INSTANCE.ContactsPanel.Children.Add(nview);
            }));
            return nview;

        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            //TODO: Implement contact search and adding
        }
    }
}
