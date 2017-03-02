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
using JH.ACU.BLL.Instruments;
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

        private readonly BllDaq _daq = new BllDaq();
        private void btnInitialize_Click(object sender, EventArgs e)
        {
            _daq.Initialize();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            _daq.Open((byte) numBoard.Value);
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            
        }

    }
}
