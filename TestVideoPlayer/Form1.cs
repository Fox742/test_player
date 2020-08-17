using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VideoPlayerEngine;

namespace TestVideoPlayer
{
    public partial class Form1 : Form
    {
        // Главный класс программы, методы которого мы вызываем из обработчиков формы
        Engine vpe = null;

        public Form1()
        {
            InitializeComponent();
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
    }
}
