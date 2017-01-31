using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ivi.Visa;
using JH.ACU.BLL;
using JH.ACU.BLL.Config;
using JH.ACU.Model;

namespace JH.ACU.UI
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private Instr instr;
        private void button1_Click(object sender, EventArgs e)
        {
            instr = BllConfig.GetInstr(InstrName.ACU);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var gpib = new Gpib
            {
                Name = InstrName.ACU,
                Port = 2,
            };
            var serial = new Serial
            {
                BaudRate = 9600,
                Name = InstrName.ACU,
                DataBits = 8,
                StopBits = SerialStopBitsMode.Two,
                Parity = SerialParity.Mark,
                Port = 4,
            };
            var tcp = new TcpIp
            {
                Name = InstrName.ACU,
                IpAddress = "192.168.1.101",
                Port = 502,
                Timeout = 2050,
            };
            InstrConfig icInstrConfig = new InstrConfig {gpib};
            BllConfig.Fun(new InstrConfig { new Instr{TcpIp = tcp,Gpib = gpib} });
        }
    }
}
