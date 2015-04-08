using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace VOIPIfier.Network
{
    public class Backend
    {
        private static Thread ListenThreadTCP;
        private static Thread ListenThreadUDP;

        public static String CONNECTED_VOIP_IP;
        public static int PORT = 2437;
        public static bool Running = false;
        public static UdpClient client;

        public static void SendPacketTCP(Packets.BasePacket packet)
        {
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse(packet.IP), packet.Port);
            byte[] buffer = packet.GetBytes();
            client.GetStream().Write(buffer, 0, buffer.Length);
        }

        public static void Listen()
        {
            if (ListenThreadTCP == null || !ListenThreadTCP.IsAlive)
            {
                Running = true;

                ListenThreadTCP = new Thread(new ThreadStart(ListenForTcp));
                ListenThreadTCP.IsBackground = true;
                ListenThreadTCP.Start();

                ListenThreadUDP = new Thread(new ThreadStart(ListenForUDP));
                ListenThreadUDP.IsBackground = true;
                ListenThreadUDP.Start();
            }
        }

        public static void ListenForUDP()
        {
            UdpClient client = new UdpClient(PORT);

            //System.IO.Stream stream = System.IO.File.OpenWrite("Speech Off.wav");
            while (Running)
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Any, PORT);
                byte[] buffer = client.Receive(ref ip);
                //stream.Write(buffer, 0, buffer.Length);
                Sound.Backend.AddBytes(buffer, buffer.Length);
                //if (buffer.Length < 1024) Sound.Backend.Play();
            }
            //stream.Close();
        }

        public static void SendUDP(byte[] bytes, int length, String ip, int port)
        {
            if (ip == "" || ip == null) return;
            if (port <= 0) return;
            if (bytes == null || bytes.Length <= 0) return;

            if (length == 0) length = bytes.Length;

            if(client == null) client = new UdpClient();
            client.SendAsync(bytes, length, new IPEndPoint(IPAddress.Parse(ip), port));
        }

        public static void ListenForTcp()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();

            while (Running)
            {

                TcpClient client = listener.AcceptTcpClient(); // Listen for and accept a TcpClient
                NetworkStream stream = client.GetStream();
                
                String type = ReadString(stream);

                Packets.BasePacket packet = null;
                switch (type)
                {
                    case "StatusUpdate":
                        packet = new Packets.StatusUpdate();
                        break;
                    case "Message":
                        packet = new Packets.Message();
                        break;
                    case "CallRequest":
                        packet = new Packets.CallRequest();
                        break;
                    case "CallRequestResponse":
                        packet = new Packets.CallRequestResponse();
                        break;

                    default:
                        Logger.Error("Packet type '" + type + "' is not rekognized and will not be handled.");
                        break;
                }

                if (packet != null)
                {
                    packet.LoadJson(ReadString(stream));
                    packet.Process();
                }
            }
        }

        public static Byte[] GetStringBytes(String str)
        {
            byte[] strBytes = UTF8Encoding.UTF8.GetBytes(str); // Convert the string into bytes
            byte[] b = BitConverter.GetBytes(strBytes.Length); // Convert the length-integer of the string-array into bytes so the reading end knows how many bytes to read
            
            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(b);
            bytes.AddRange(strBytes);
            return bytes.ToArray();
        }

        public static String ReadString(Stream stream)
        {
            byte[] b = new byte[4];
            stream.Read(b, 0, 4); // Read the int to get length of the string (in bytes)
            int len = BitConverter.ToInt32(b, 0);

            byte[] str = new byte[len];
            stream.Read(str, 0, len); // Read the string bytes
            return UTF8Encoding.UTF8.GetString(str); // Return the string itself
        }

    }
}
