using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOIPIfier.Network.Packets
{
    class CallRequest : BasePacket
    {

        public CallRequest()
        {
            PacketName = "CallRequest";
            values.Add("User", User.User.RealName);
        }

        public override void Process()
        {
            MainWindow.INSTANCE.Dispatcher.BeginInvoke(new Action(() =>
            {
                var form = new UI.IncommingCall();
                form.Username = values["User"].ToString();

                form.Show();
            }));
        }

    }
}
