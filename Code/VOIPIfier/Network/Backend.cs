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

        public static async void SendPacket(Packets.BasePacket packet)
        {
            byte[] buffer = packet.GetBytes();
            if (packet.UseTCP)
            {
                // Send a tcp version
                TcpClient tcp = new TcpClient();
                await tcp.ConnectAsync(IPAddress.Parse(packet.IP), packet.Port);
                await tcp.GetStream().WriteAsync(buffer, 0, buffer.Length);
            }
            else
            {
                // Send a UDP version
                //if(client == null) client = new UdpClient(new IPEndPoint(IPAddress.Any, PORT));
                //await client.SendAsync(buffer, buffer.Length, new IPEndPoint(IPAddress.Parse(packet.IP), packet.Port));
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                IPAddress serverAddr = IPAddress.Parse(packet.IP);

                IPEndPoint endPoint = new IPEndPoint(serverAddr, packet.Port);

                sock.SendTo(buffer, endPoint);
            }
        }

        public static void Listen()
        {
            Running = true;

            ListenThreadTCP = new Thread(new ThreadStart(ListenForTcp));
            ListenThreadTCP.IsBackground = true;
            ListenThreadTCP.Start();

            ListenThreadUDP = new Thread(new ThreadStart(ListenForUDP));
            ListenThreadUDP.IsBackground = true;
            ListenThreadUDP.Start();
        }

        public static void ListenForUDP()
        {
            client = new UdpClient(PORT);

            while (Running)
            {
                try
                {
                    IPEndPoint ip = new IPEndPoint(IPAddress.Any, PORT);
                    byte[] buffer = client.Receive(ref ip);
                    //ThreadPool.QueueUserWorkItem((Object o) =>
                    //{
                    HandlePacket(buffer);
                    //});
                }
                catch (Exception ex)
                {
                    Logger.Error("UDP-Recive error: " + ex.Message);
                }
            }
        }

        public static void SendUDP(byte[] bytes, int length, String ip, int port)
        {
            if (ip == "" || ip == null) return;
            if (port <= 0) return;
            if (bytes == null || bytes.Length <= 0) return;

            if (length == 0) length = bytes.Length;

            if (client == null) client = new UdpClient();
            client.SendAsync(bytes, length, new IPEndPoint(IPAddress.Parse(ip), port));
        }

        public static void ListenForTcp()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();

            while (Running)
            {

                TcpClient client = listener.AcceptTcpClient(); // Listen for and accept a TcpClient

                ThreadPool.QueueUserWorkItem((Object o) => // Put into it's own "fake-thread" and handle packet in order to not block the accepting thread
                {
                    NetworkStream stream = client.GetStream();
                    HandlePacket(stream);
                });
            }
        }

        /// <summary>
        /// Will pipe into HandlePacket(Stream str) with stream as a MemoryStream of bytes put in
        /// </summary>
        /// <param name="data">The data to put in MemoryStream/to be handled</param>
        private static void HandlePacket(byte[] data)
        {
            Logger.Info("Adding " + data.Length + " bytes to MS for packet handeling");
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            HandlePacket(ms);
        }

        /// <summary>
        /// Will read and process a packet
        /// </summary>
        /// <param name="str">The input stream</param>
        private static void HandlePacket(Stream str)
        {
            String type = ReadString(str);

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
                packet.LoadJson(ReadString(str));
                packet.Process();
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
