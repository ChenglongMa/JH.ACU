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
using JH.ACU.Model.Config.InstrumentConfig;

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

        }
    }
}
