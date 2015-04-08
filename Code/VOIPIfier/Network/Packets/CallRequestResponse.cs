using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOIPIfier.Network.Packets
{
    class CallRequestResponse : BasePacket
    {

        public CallRequestResponse()
        {
            PacketName = "CallRequestResponse";
        }

        public override void Process()
        {

        }

    }
}
