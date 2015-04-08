using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VOIPIfier.Network.Packets
{
    public class BasePacket
    {
        protected Dictionary<string, object> values = new Dictionary<string, object>();

        public String PacketName;

        public int Port { get; set; }

        public String IP { get; set; }

        internal byte[] GetBytes()
        {
            if (PacketName == null || PacketName == String.Empty)
            {
                throw new Exception("PacketName cannot be null or empty!");
            }

            // Get the json value and the flag for this packet.
            String json = JsonConvert.SerializeObject(values);

            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(Backend.GetStringBytes(PacketName));
            bytes.AddRange(Backend.GetStringBytes(json));
            return bytes.ToArray();
        }

        public void LoadJson(String json)
        {
            values = JsonConvert.DeserializeObject<Dictionary<String, object>>(json); // Load the json values
        }

        public virtual void Process()
        {

        }

        public void Send()
        {
            Backend.SendPacketTCP(this);
        }
    }
}
