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
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : UserControl
    {
        public MessageDialog()
        {
            InitializeComponent();
            Time = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
        }

        public String Message
        {
            get
            {
                return MessaageLbl.Content.ToString();
            }
            set
            {
                MessaageLbl.Content = value;
            }
        }

        public String Time
        {
            get
            {
                return TimeLbl.Content.ToString();
            }
            set
            {
                TimeLbl.Content = value;
            }
        }

        public String User
        {
            get
            {
                return UserLbl.Content.ToString();
            }
            set
            {
                UserLbl.Content = value;
            }
        }
    }
}
