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

namespace VOIPIfier.User
{
    /// <summary>
    /// Interaction logic for UserView.xaml
    /// </summary>
    public partial class UserView : UserControl
    {
        public String Username
        {
            get
            {
                return UName.Content.ToString();
            }
            set
            {
                UName.Content = value;
            }
        }

        public String RecentMessage
        {
            get
            {
                return RecentMsg.Content.ToString();
            }
            set
            {
                String msg = value;
                SolidColorBrush b = (SolidColorBrush)this.Background;
                if (!(b.Color.R == 255 && b.Color.G == 255 && b.Color.B == 255))
                {
                    SetBackground(237, 250, 255);
                    if (msg.Length > 10)
                    {
                        msg = msg.Remove(7);
                        if (msg.Contains(System.Environment.NewLine))
                        {
                            String[] split = msg.Split(new String[] {System.Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
                            msg = split[0];
                        }
                        msg += "...";
                    }

                    RecentMsg.Content = msg;
                }
            }
        }

        public UserView()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.RecentMessage = "";
            SetBackground(255, 255, 255);
        }

        public void SetBackground(int r, int g, int b)
        {
            this.Background = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
        }
    }
}
