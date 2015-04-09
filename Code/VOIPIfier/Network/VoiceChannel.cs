using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace VOIPIfier.Network
{
    class VoiceChannel
    {
        // The peers to send the data to
        private Dictionary<String, int> listeners = new Dictionary<string, int>();
        private List<byte> VoiceBuffer = new List<byte>();
        private Sound.WideBandSpeexCodec encoder;
        private int UdpPORT = 0;

        /// <summary>
        /// Indicates wether or not to send the voice data
        /// </summary>
        public Boolean Sending { get; set; }

        /// <summary>
        /// Will register a peer to recive the voice data
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void RegisterListener(String ip)
        {
            listeners.Add(ip, UdpPORT);
        }

        private void StartRecoding(int device = 0)
        {
            UdpPORT = new Random().Next(1000, 25564);
            Logger.Info("Opened port " + UdpPORT + " for listening and sending");

            VoiceBuffer.Clear();

            encoder = new Sound.WideBandSpeexCodec();

            var sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.BufferMilliseconds = 20;
            sourceStream.NumberOfBuffers = 2;
            sourceStream.DeviceNumber = device;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, 16, 1);

            sourceStream.DataAvailable += sourceStream_DataAvailable;


            sourceStream.StartRecording();

            Listen();

        }

        void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            Byte[] send = encoder.Encode(e.Buffer, 0, e.BytesRecorded);
            if(Sending)
            {
                for (int i = 0; i < listeners.Count; i++)
                {
                    var element = listeners.ElementAt(i);
                    Network.Backend.SendUDP(send, send.Length, element.Key, UdpPORT);
                }
            }
        }

        /// <summary>
        /// Start to listen to the data in this channel
        /// </summary>
        public void Listen()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                UdpClient client = new UdpClient(UdpPORT);
                //client.Client.Bind(new IPEndPoint(IPAddress.Any, UdpPORT));
                while (Sending)
                {
                    IPEndPoint ip = new IPEndPoint(IPAddress.Any, UdpPORT);
                    Logger.Info("Waiting to recive info on port " + UdpPORT);
                    byte[] data = client.Receive(ref ip);
                    Logger.Info("Recived " + data.Length + " bytes");
                    Sound.Backend.AddBytes(data, data.Length);
                }
            }));

            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// Will start broadcasting the voice data to the registered listeners
        /// </summary>
        public void StartBroadcasting()
        {
            Sending = true;
            StartRecoding();
        }

        public void StopBroadcasting()
        {
            Sending = false;
        }
    }
}
