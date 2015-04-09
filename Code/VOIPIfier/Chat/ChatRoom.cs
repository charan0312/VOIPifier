using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOIPIfier.Chat
{
    class ChatRoom
    {
        private Dictionary<String, int> peers = new Dictionary<string, int>(); // Registered peers

        /// <summary>
        /// Will register a peer to participate in all packets sent in this chat.
        /// </summary>
        /// <param name="ip">The ip of the peer to join</param>
        /// <param name="port">What port should we connect to</param>
        public void AddPeer(String ip, int port)
        {
            peers.Add(ip, port);
        }

        /// <summary>
        /// Will send a packet to the whole chatroom
        /// </summary>
        /// <param name="packet"></param>
        public void SendPacket(Network.Packets.BasePacket packet)
        {
            for (int i = 0; i < peers.Count; i++)
            {
                Network.Backend.SendPacket(packet);
            }
        }

    }
}
