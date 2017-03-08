using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ivi.Visa;
using JH.ACU.BLL;
using JH.ACU.BLL.Config;
using JH.ACU.BLL.Instruments;
using JH.ACU.Lib;
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
        private BllPrs _prs;
        private decimal BoardIndex { get { return numBoard.Value; } set { numBoard.Value = value; } }
        private decimal MainRelayIndex { get { return numMainRelay.Value; } set { numMainRelay.Value = value; } }
        private decimal SubRelayIndex { get { return numSubRelay.Value; } set { numSubRelay.Value = value; } }
        private void btnInitialize_Click(object sender, EventArgs e)
        {
            _daq.Initialize();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            _daq.Open((byte)BoardIndex);
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            _daq.SetSubRelayStatus((byte) BoardIndex, (int) SubRelayIndex, true);

        }

        private void btn12_Click(object sender, EventArgs e)
        {
            for (int i = -1; i < 11; i++)
            {
                try
                {
                    _daq.Open((byte) i);
                    Thread.Sleep(1000);
                    _daq.Close((byte) i);
                }
                catch (Exception ex)
                {
                    MessageBoxHelper.ShowInformationOk(ex.Message+"\nIndex = "+i);
                }
            }
            MessageBoxHelper.ShowInformationOk("测试完毕");

        }

        private void btn13_Click(object sender, EventArgs e)
        {
            MessageBoxHelper.ShowInformationOk("请使用上方按钮进行单个测试\n注意断开后再打开下一个");
        }

        private void btn14_Click(object sender, EventArgs e)
        {
            _daq.SetSubRelayStatus((byte) BoardIndex, 200, true);
            //Thread.Sleep(500);
            _daq.SetSubRelayStatus((byte) BoardIndex, 215, true);
            //Thread.Sleep(500);
            _daq.SetSubRelayStatus((byte) BoardIndex, 256, true);
            //Thread.Sleep(500);
            MessageBoxHelper.ShowInformationOk("测试完毕");
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _daq.ResetAll();
        }

        private void btn15_Click(object sender, EventArgs e)
        {
            _daq.Open(0);
            _daq.Open(6);
            MessageBoxHelper.ShowInformationOk("执行完毕，请测试0号板和6号板是否正常使能");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _daq.Close((byte) BoardIndex);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _daq.SetMainRelayStatus((int) MainRelayIndex, true);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            _daq.SetMainRelayStatus((int) MainRelayIndex, false);

        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            _daq.SetSubRelayStatus((byte)BoardIndex, (int)SubRelayIndex, false);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            _prs = new BllPrs(InstrName.PRS0);
            _prs.Initialize();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var value = float.Parse(textBox1.Text);
            _prs.SetResistance(value);
        }

    }
}
