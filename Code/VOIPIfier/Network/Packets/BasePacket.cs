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
        /// <summary>
        /// Use TCP or UDP to send this packet
        /// </summary>
        public Boolean UseTCP = true;

        /// <summary>
        /// Will get all bytes for the current packet to be sent
        /// </summary>
        /// <returns></returns>
        internal byte[] GetBytes()
        {
            // Get the json value and the flag for this packet.
            String json = JsonConvert.SerializeObject(values);

            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(Backend.GetStringBytes(PacketName));
            bytes.AddRange(Backend.GetStringBytes(json));
            return bytes.ToArray();
        }

        /// <summary>
        /// Returns the length of the packet-byte-array
        /// </summary>
        public int Length
        {
            get
            {
                return GetBytes().Count();
            }
        }

        public void LoadJson(String json)
        {
            values = JsonConvert.DeserializeObject<Dictionary<String, object>>(json); // Load the json values
        }

        public virtual void Process()
        {
            // Will be overriden by other packets
        }

        public void Send()
        {
            Backend.SendPacket(this);
        }
    }
}
