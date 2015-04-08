using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOIPIfier.Network.Packets
{
    class Message : BasePacket
    {

        public String MessageString
        {
            get
            {
                return values["Message"].ToString();
            }
            set
            {
                if (values.ContainsKey("Message"))
                {
                    values["Message"] = value;
                }
                else
                {
                    values.Add("Message", value);
                }
            }
        }

        public Message()
        {
            PacketName = "Message";
            values.Add("User", User.User.RealName);
        }

        public override void Process()
        {
            MainWindow.INSTANCE.Dispatcher.BeginInvoke(new Action(() =>
            {
                MainWindow.AddMessage(values["User"].ToString(), values["Message"].ToString());
            }));
        }

    }
}
