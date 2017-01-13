using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JH.ACU.BLL;
using JH.ACU.Tool;

namespace JH.ACU.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //var log = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器

            //log.Info(DateTime.Now + ": login success");//写入一条新log
            ultraGrid1.Show();
        }

        private int tick;
        BllAcu acu = new BllAcu("ASRL4::INSTR");

        private void button1_Click(object sender, EventArgs e)
        {
            tick = Environment.TickCount;
            var b = acu.Start();
            var t = Environment.TickCount - tick;
            MessageBoxHelper.ShowInformationOK(b+"---"+t.ToString());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //var dataT = new byte[] {0x72, 0x08, 0x10, 199};
            //var data = acu.WriteAndRead(dataT);
            MessageBoxHelper.ShowInformationOK(acu.Stop().ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            acu.ReadRtFault();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            acu.StopRtFault();
        }

    }
}
