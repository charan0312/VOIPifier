using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using NAudio.Wave;

namespace VOIPIfier.UI
{
    /// <summary>
    /// Interaction logic for IncommingCall.xaml
    /// </summary>
    public partial class IncommingCall : Window, IDisposable
    {
        private WaveOut waveOut;

        public IncommingCall()
        {
            InitializeComponent();
            waveOut = new WaveOut();

            this.Closing += IncommingCall_Closing;
            WaveFileReader reader = new WaveFileReader("Resources/ring_in.wav");

            Sound.LoopStream loop = new Sound.LoopStream(reader);
            loop.EnableLooping = true;

            waveOut.Init(loop);
            waveOut.Play();
        }

        void IncommingCall_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            waveOut.Stop();
            if (Answer == null) Answer = false;
        }

        public string Username {
            get
            {
                return UNameLbl.Content.ToString();
            }
            set
            {
                UNameLbl.Content = value;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Answer = false;
            Close();
        }

        public Boolean Answer { get; set; }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Answer = true;
            Close();
        }

        public void Dispose()
        {
            waveOut.Dispose();
        }
    }
}
