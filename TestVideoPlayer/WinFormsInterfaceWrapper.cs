using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlayerEngine;
using System.Windows.Forms;
using AxWMPLib;
using WMPLib;

namespace TestVideoPlayer
{
    class WinFormsInterfaceWrapper:BaseInterfaceWrapper
    {
        private Form1 _mainForm = null;

        public WinFormsInterfaceWrapper(Form1 mainForm)
        {
            _mainForm = mainForm;
            _mainForm.WMPAccessibility = false;
            _mainForm.subscribeVideoEndEvent( VideoChangeState );
        }

        private void VideoChangeState(object sender, _WMPOCXEvents_PlayStateChangeEvent e)
        {
            if (e.newState == (int)WMPPlayState.wmppsMediaEnded)
            {
                RiseVideoEndEvent();
            }
        }

        public override void PrintMessage(string message, string Caption="")
        {
            MessageBox.Show(message, Caption, MessageBoxButtons.OK);
        }

        public override void startVideo(string path, double position)
        {
            _mainForm.WMPVideo = path;
            if (position!=0.0)
                _mainForm.PositionWMP = position;
        }

        public override double getPosition()
        {
            return _mainForm.PositionWMP;
        }

        public override void stopVideo()
        {

        }

    }
}
