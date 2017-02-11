using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JH.ACU.BLL.Instruments;
using JH.ACU.Lib;
using JH.ACU.Model;


namespace JH.ACU.Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private BllAcu _acu;
        private BllChamber _chamber;
        private BllDaq _daq;
        private BllDmm _dmm;
        private BllPrs _prs0;
        private BllPrs _prs1;
        private BllPwr _pwr;
        //打开设备，
        private bool Open()
        {
            try
            {
                //_acu = new BllAcu();
                //_chamber = new BllChamber();
                _daq = new BllDaq();
                _dmm = new BllDmm();
                _prs0 = new BllPrs(InstrName.PRS0);
                _prs1 = new BllPrs(InstrName.PRS1);
                _pwr = new BllPwr();
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("设备端口打开失败\n错误信息:" + ex.Message);
                return false;
            }
        }
        //设备初始化，包括自检
        private bool Initialize()
        {
            try
            {
                _daq.Initialize();
                _pwr.Initialize();
                _dmm.Initialize();
                _prs0.Initialize();
                _prs1.Initialize();
                //ACU 不需要初始化
                //温箱不需要初始化
                //
                return true;
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("设备初始化失败\n错误信息:" + ex.Message);
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Open())
            {
                MessageBoxHelper.ShowInformationOK("打开成功");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Initialize();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            VisaHelper.FindResources(InstrType.Gpib);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            _daq.EnableRelays(2,2);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            var o = _pwr.Ocp;
            _pwr.Ocp = !_pwr.Ocp;
            _pwr.OutputCurrent = 1.2;
            _pwr.OutputVoltage = 5.8;
            var a = _pwr.ActualCurrent;
            a = _pwr.ActualVoltage;
            a = _pwr.OutputCurrent;
            a = _pwr.OutputVoltage;
            _pwr.Ovp = 15.3;
            a = _pwr.Ovp;
            _pwr.OutPutState = true;
            _pwr.OutPutState = false;
            var s = _pwr.Idn;
            s = _pwr.Error;
            _pwr.ClearProduction();
            _pwr.Wait();
            _pwr.WaitComplete();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            var s = _dmm.GetCurrent();
            s = _dmm.GetCurrent();
            var q = _dmm.GetFourWireRes();
            q=_dmm.GetFrequency();
            q=_dmm.GetRes();
            q=_dmm.GetVoltage();
            var r=_dmm.Read(true);
            q=_dmm.Read(250);
            _dmm.SetAutoZero(BllDmm.AutoZero.Off);

        }

    }
}
