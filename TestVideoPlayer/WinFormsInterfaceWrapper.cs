using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoPlayerEngine;
using System.Windows.Forms;

namespace TestVideoPlayer
{
    class WinFormsInterfaceWrapper:BaseInterfaceWrapper
    {
        private Form1 _mainForm = null;

        public WinFormsInterfaceWrapper(Form1 mainForm)
        {
            _mainForm = mainForm;
        }

        public override void PrintMessage(string message, string Caption="")
        {
            MessageBox.Show(message, Caption, MessageBoxButtons.OK);
        }
    }
}
