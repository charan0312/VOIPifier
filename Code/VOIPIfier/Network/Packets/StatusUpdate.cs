using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOIPIfier.Network.Packets
{
    class StatusUpdate : BasePacket
    {
        public String Status
        {
            get
            {
                return values["Status"].ToString();
            }
            set
            {
                if (values.ContainsKey("Status"))
                {
                    values["Status"] = value;
                }
                else
                {
                    values.Add("Status", value);
                }
            }
        }

        public StatusUpdate()
        {
            PacketName = "StatusUpdate";
            values.Add("User", User.User.RealName);
        }

        public override void Process()
        {
            Logger.Info("User " + values["User"].ToString() + " went " + values["Status"].ToString());
        }
    }
}
