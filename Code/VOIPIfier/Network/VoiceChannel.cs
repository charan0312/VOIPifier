using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using System.IO;
using System.Threading;

namespace VOIPIfier.Network
{
    class VoiceChannel
    {
        // The peers to send the data to
        private Dictionary<String, int> listeners = new Dictionary<string, int>();
        private List<byte> VoiceBuffer = new List<byte>();
        private Thread broadcastThread;
        /// <summary>
        /// Indicates wether or not to send the voice data
        /// </summary>
        public Boolean Sending { get; set; }

        public void RegisterListener(String ip, int port)
        {
            listeners.Add(ip, port);
        }

        private void GetRecordByte(int device = 0)
        {
            VoiceBuffer.Clear();

            var sourceStream = new NAudio.Wave.WaveIn();
            sourceStream.BufferMilliseconds = 20;
            sourceStream.DeviceNumber = device;
            sourceStream.WaveFormat = new NAudio.Wave.WaveFormat(44100, 16, 1);

            sourceStream.DataAvailable += sourceStream_DataAvailable;


            sourceStream.StartRecording();

        }

        void sourceStream_DataAvailable(object sender, WaveInEventArgs e)
        {
            /*for (int i = 0; i < e.BytesRecorded; i++)
            {
                VoiceBuffer.Add((byte)e.Buffer.GetValue(i));
            }*/

            if(Sending)
            {
                for (int i = 0; i < listeners.Count; i++)
                {
                    var element = listeners.ElementAt(i);
                    Network.Backend.SendUDP(e.Buffer, e.BytesRecorded, element.Key, element.Value);
                }
            }
        }

        /// <summary>
        /// Will start broadcasting the voice data to the registered listeners
        /// </summary>
        public void StartBroadcasting()
        {
            if (broadcastThread == null || !broadcastThread.IsAlive)
            {
                GetRecordByte();
                Sending = true;
                /*broadcastThread = new Thread(new ThreadStart(_sendBroadcast));
                broadcastThread.IsBackground = true;
                broadcastThread.Start();*/
            }
        }

        public void StopBroadcasting()
        {
            Sending = false;
            //broadcastThread.Abort();
        }

        private Byte[] getVoiceBytes()
        {
            int len = VoiceBuffer.Count();
            Byte[] buffer = new Byte[len];

            for (int i = 0; i < len; i++)
            {
                buffer[i] = VoiceBuffer[i];
            }
            return buffer;
        }

        private void _sendBroadcast() {
            /*while (Sending)
            {
                for (int i = 0; i < listeners.Count; i++)
                {
                    var element = listeners.ElementAt(i);
                    String ip = element.Key;
                    int port = element.Value;
                    byte[] buffer = getVoiceBytes();
                    Network.Backend.SendUDP(buffer, 0, ip, port);
                }
                VoiceBuffer.Clear();
            }*/
        }

    }
}
