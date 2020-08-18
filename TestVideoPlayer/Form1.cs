using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using VideoPlayerEngine;
using AxWMPLib;

namespace TestVideoPlayer
{
    public partial class Form1 : Form
    {
        // Главный класс программы, методы которого мы вызываем из обработчиков формы
        Engine vpe = null;
        private SynchronizationContext _synchronizationContext;

        public Form1()
        {
            InitializeComponent();
            _synchronizationContext = SynchronizationContext.Current;
            vpe = new Engine(new WinFormsInterfaceWrapper(this));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            openFileDialog1.Filter = "txt files (*.txt)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                vpe.LoadShedule(openFileDialog1.FileName);
            }
            
        }

        public bool WMPAccessibility
        {
            set
            {
                _synchronizationContext.Post(
                (o) => axWindowsMediaPlayer1.Ctlenabled = value, null);
            }
        }

        public string WMPVideo
        {
            set
            {
                _synchronizationContext.Post(
                (o) => axWindowsMediaPlayer1.URL = value, null);
            }
        }

        public double PositionWMP
        {
            set
            {
                _synchronizationContext.Post(
                (o) => axWindowsMediaPlayer1.Ctlcontrols.currentPosition = value, null);
            }
            get
            {
                return axWindowsMediaPlayer1.Ctlcontrols.currentPosition;
            }
        }

        public void subscribeVideoEndEvent(_WMPOCXEvents_PlayStateChangeEventHandler _handler)
        {
            _synchronizationContext.Post(
                (o) => axWindowsMediaPlayer1.PlayStateChange+=_handler, null);
        }

    }
}
