using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NAudio.Wave;
using System.IO;

namespace VOIPIfier.Sound
{
    class Backend
    {
        private static WaveOut waveOut;
        private static BufferedWaveProvider provider;

        public static void Init()
        {
            provider = new BufferedWaveProvider(new WaveFormat(44100, 16, 1)); // DO NOT TOUCH!
            provider.DiscardOnBufferOverflow = true;

            waveOut = new WaveOut();
            //waveOut.DesiredLatency = 20;
            waveOut.NumberOfBuffers = 2;
            waveOut.Init(provider);

            waveOut.Play();
        }

        public static void AddBytes(byte[] bytes, int length)
        {
            if (provider == null) Init(); // Has the provider not been initilized yet? If not, do so!
            provider.AddSamples(bytes, 0, bytes.Length);
        }

        public String[] getRecordDevices()
        {
            int waveInDevices = WaveIn.DeviceCount;
            String[] re = new String[waveInDevices];
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                re[waveInDevice] = deviceInfo.ProductName;
                Console.WriteLine("Device {0}: {1}, {2} channels", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }
            return re;
        }

        public static void PlayLoop(String file)
        {

        }

        internal static void Play()
        {
            waveOut.Play();
        }
    }
}
