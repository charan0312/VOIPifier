﻿using System;
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

        /// <summary>
        /// Indicates wether or not to send the voice data
        /// </summary>
        public Boolean Sending { get; set; }

        /// <summary>
        /// Will register a peer to recive the voice data
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void RegisterListener(String ip, int port)
        {
            listeners.Add(ip, port);
        }

        private void StartRecoding(int device = 0)
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
            Sending = true;
            StartRecoding();
        }

        public void StopBroadcasting()
        {
            Sending = false;
        }
    }
}
