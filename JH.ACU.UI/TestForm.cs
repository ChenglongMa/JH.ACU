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
using JH.ACU.Model.Config.TestConfig;

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
            _daq.OpenBoard((byte)BoardIndex);
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
                    _daq.OpenBoard((byte) i);
                    Thread.Sleep(1000);
                    _daq.CloseBoard((byte) i);
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
            _daq.OpenBoard(0);
            _daq.OpenBoard(6);
            MessageBoxHelper.ShowInformationOk("执行完毕，请测试0号板和6号板是否正常使能");
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _daq.CloseBoard((byte) BoardIndex);
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
            _prs = new BllPrs(InstrName.Prs0);
            _prs.Initialize();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var value = double.Parse(textBox1.Text);
            _prs.SetResistance(value);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var list=new List<int>();
            for (int i = 0; i < 74; i++)
            {
                list.Add(i);
            }
            var testCondition = new TestCondition
            {
                Voltage = {HighVolt = 13.5, NorVolt = 12.0, LowVolt = 6.5},
                Temperature = {Duration = 1000, Enable = true, HighTemp = 85.0, LowTemp = -40.0, NorTemp = 25.0},
                AcuItems = new List<AcuItems>(new[] { new AcuItems { Name = "A", Items = new List<int> { 1, 5, 6, 73 } }, new AcuItems { Name = "B", Items = new List<int> { 1, 8,10 } }, new AcuItems { Name = "C", Items = list} })
            };
            var settingFileName = Environment.CurrentDirectory + "\\Config\\TestCondition.xml";

            XmlHelper.XmlSerializeToFile(testCondition, settingFileName);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var settingFileName = Environment.CurrentDirectory + "\\Config\\TestCondition.xml";
            var t = XmlHelper.XmlDeserializeFromFile<TestCondition>(settingFileName, Encoding.UTF8);
            foreach (var acuItemse in t.AcuItems)
            {
                textBox1.Text += acuItemse.Items.Count+";";
            }
        }

    }
}
