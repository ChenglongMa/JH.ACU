using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JH.ACU.BLL.Abstract;
using JH.ACU.BLL.Instruments;
using JH.ACU.BLL.Properties;
using JH.ACU.Lib;
using JH.ACU.Model;
using NationalInstruments.Restricted;
using NationalInstruments.UI;
using NationalInstruments.Visa.Internal;

namespace JH.ACU.UI
{
    public partial class InstrumentControlForm : Form
    {
        #region 构造函数

        public InstrumentControlForm()
        {
            InitializeComponent();
            cmbInstrName.DisplayMember = "Key";
            cmbInstrName.ValueMember = "Value";
            cmbInstrName.DataSource = BuildInstrNameSource();
            SetControlStatus(InstrName.Daq, false);
            SetControlStatus(InstrName.Prs0, false);
            SetControlStatus(InstrName.Prs1, false);
            SetControlStatus(InstrName.Dmm, false);
            SetControlStatus(InstrName.Pwr, false);
        }

        #endregion

        #region 属性、字段

        private BllPwr _pwr;
        private BllPrs _prs0;
        private BllPrs _prs1;
        private BllChamber _chamber;
        private BllDmm _dmm;
        private BllDaq _daq;

        private byte BoardIndex
        {
            get { return (byte) (numBoardIndex.Value - 1); }
        }

        #endregion

        private BindingSource BuildInstrNameSource()
        {
            var dic = new Dictionary<string, BllVisa> {{"PWR", _pwr}, {"PRS#1", _prs0}, {"PRS#2", _prs1}, {"DMM", _dmm}};
            return new BindingSource {DataSource = dic};
        }
        private void SetControlStatus(InstrName name, bool enable)
        {
            var isOpen = enable ? "Close" : "Open";
            switch (name)
            {
                case InstrName.Acu:
                    break;
                case InstrName.Pwr:
                    swPwrOpen.Caption = isOpen;
                    btnOvp.Enabled = enable;
                    swPwrOcp.Enabled = enable;
                    swPwrOutput.Enabled = enable;
                    btnSetCurr.Enabled = enable;
                    btnSetVolt.Enabled = enable;
                    break;
                case InstrName.Prs0:
                    swRes0Open.Caption = isOpen;
                    btnSetRes0.Enabled = enable;
                    break;
                case InstrName.Prs1:
                    swRes1Open.Caption = isOpen;
                    btnSetRes1.Enabled = enable;
                    break;
                case InstrName.Dmm:
                    swDmmOpen.Caption = isOpen;
                    btnGetCurr.Enabled = enable;
                    btnGetVolt.Enabled = enable;
                    btnGetFRes.Enabled = enable;
                    btnGetFreq.Enabled = enable;
                    btnGetRes.Enabled = enable;
                    break;
                case InstrName.Chamber:
                    break;
                case InstrName.Daq:
                    swDaqOpen.Caption = isOpen;
                    btnBoardOpen.Enabled = enable;
                    btnBoardClose.Enabled = enable;
                    btnRelayEnable.Enabled = enable;
                    btnRelayDisable.Enabled = enable;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("name", name, null);
            }
        }

        private void InstrumentControlForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _pwr.DisposeIfNotNull();
            _prs0.DisposeIfNotNull();
            _prs1.DisposeIfNotNull();
            _chamber.DisposeIfNotNull();
            _dmm.DisposeIfNotNull();
            _daq.DisposeIfNotNull();
        }

        #region 程控电源操作

        private void swPwrOpen_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                if (!swPwrOpen.Value)
                {
                    _pwr = new BllPwr();
                    _pwr.Initialize();
                    SetControlStatus(InstrName.Pwr, true);
                }
                else
                {
                    _pwr.Dispose();
                    SetControlStatus(InstrName.Pwr, false);
                }
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                e.Cancel = true;
            }
        }

        private void swPwrOcp_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                _pwr.Ocp = !swPwrOcp.Value;
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                e.Cancel = true;
            }
        }

        private void swPwrOutput_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                _pwr.OutPutState = !swPwrOutput.Value;
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                e.Cancel = true;
            }
        }

        private void btnOvp_Click(object sender, EventArgs e)
        {
            try
            {
                _pwr.Ovp = (double) numOvp.Value;
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnSetVolt_Click(object sender, EventArgs e)
        {
            try
            {
                _pwr.OutputVoltage = (double) numSetVolt.Value;
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnSetCurr_Click(object sender, EventArgs e)
        {
            try
            {
                _pwr.OutputCurrent = (double) numSetCurr.Value;
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        #endregion

        #region 电阻箱操作

        #region PRS #1

        private void swResOpen_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                if (!swRes0Open.Value)
                {
                    _prs0 = new BllPrs(InstrName.Prs0);
                    _prs0.Initialize();
                    SetControlStatus(InstrName.Prs0, true);
                }
                else
                {
                    _prs0.Dispose();
                    SetControlStatus(InstrName.Prs0, false);
                }
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                e.Cancel = true;
            }
        }

        private void btnSetRes_Click(object sender, EventArgs e)
        {
            try
            {
                _prs0.SetResistance((double) numSetRes0.Value);
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        #endregion


        #region PRS #2
        private void swRes1Open_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                if (!swRes1Open.Value)
                {
                    _prs1 = new BllPrs(InstrName.Prs1);
                    _prs1.Initialize();
                    SetControlStatus(InstrName.Prs1, true);
                }
                else
                {
                    _prs1.Dispose();
                    SetControlStatus(InstrName.Prs1, false);
                }
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                e.Cancel = true;
            }

        }

        private void btnSetRes1_Click(object sender, EventArgs e)
        {
            try
            {
                _prs1.SetResistance((double)numSetRes1.Value);
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }

        }


        #endregion

        #endregion

        #region 温箱操作

        private void btnGetTemp_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chamber == null)
                {
                    _chamber = new BllChamber();
                }
                numGetTemp.Value = (decimal) _chamber.GetTemp();
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;

            }

        }

        private void btnSetTemp_Click(object sender, EventArgs e)
        {
            try
            {
                if (_chamber == null)
                {
                    _chamber = new BllChamber();
                }
                _chamber.SetTemp((double) numSetTemp.Value);
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;

            }


        }

        private void swChamRun_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                if (_chamber == null)
                {
                    _chamber = new BllChamber();
                }
                if (!swChamRun.Value)
                {
                    _chamber.Run();
                }
                else
                {
                    _chamber.Stop();
                }
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                e.Cancel = true;
            }

        }

        #endregion

        #region 万用表操作

        private void swDmmOpen_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                if (!swDmmOpen.Value)
                {
                    _dmm = new BllDmm();
                    _dmm.Initialize();
                    SetControlStatus(InstrName.Dmm, true);
                }
                else
                {
                    _dmm.Dispose();
                    SetControlStatus(InstrName.Dmm, false);
                }
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                e.Cancel = true;
            }
        }

        private void btnGetVolt_Click(object sender, EventArgs e)
        {
            try
            {
                numGetVolt.Value = (decimal) _dmm.GetVoltage();
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnGetCurr_Click(object sender, EventArgs e)
        {
            try
            {
                numGetCurr.Value = (decimal) _dmm.GetCurrent();
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnGetRes_Click(object sender, EventArgs e)
        {
            try
            {
                numGetRes.Value = (decimal) _dmm.GetRes();
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnGetFRes_Click(object sender, EventArgs e)
        {
            try
            {
                numGetFRes.Value = (decimal) _dmm.GetFourWireRes();
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnGetFreq_Click(object sender, EventArgs e)
        {
            try
            {
                numGetFreq.Value = (decimal) _dmm.GetFrequency();
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        #endregion

        #region DAQ操作

        private void swDaqOpen_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                if (!swDaqOpen.Value)
                {
                    _daq = new BllDaq();
                    _daq.Initialize();
                    SetControlStatus(InstrName.Daq, true);
                }
                else
                {
                    _daq.Dispose();
                    SetControlStatus(InstrName.Daq, false);
                }
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                e.Cancel = true;
            }
        }

        private void btnBoardOpen_Click(object sender, EventArgs e)
        {
            try
            {
                _daq.OpenBoard(BoardIndex);
                RefreshLedStatus();
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnBoardClose_Click(object sender, EventArgs e)
        {
            try
            {
                _daq.CloseBoard(BoardIndex);
                RefreshLedStatus();
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnRelayEnable_Click(object sender, EventArgs e)
        {
            try
            {
                var index = (int) numRelayIndex.Value;
                if (index == 300 || index == 301 || index == 302)
                {
                    _daq.SetMainRelayStatus(index, true);
                    switch (index)
                    {
                        case 300:
                            led300.Value = true;
                            break;
                        case 301:
                            led301.Value = true;
                            break;
                        case 302:
                            led302.Value = true;
                            break;
                    }
                }
                else
                {
                    _daq.SetSubRelayStatus(BoardIndex, index, true);
                    RefreshLedStatus();
                }

                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        private void btnRelayDisable_Click(object sender, EventArgs e)
        {
            try
            {
                var index = (int) numRelayIndex.Value;
                if (index == 300 || index == 301 || index == 302)
                {
                    _daq.SetMainRelayStatus(index, false);
                    switch (index)
                    {
                        case 300:
                            led300.Value = false;
                            break;
                        case 301:
                            led301.Value = false;
                            break;
                        case 302:
                            led302.Value = false;
                            break;
                    }
                }
                else
                {
                    _daq.SetSubRelayStatus(BoardIndex, index, false);
                    RefreshLedStatus();
                }
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

        /// <summary>
        /// 刷新子板指示灯状态，包括子板灯和子板继电器灯
        /// </summary>
        private void RefreshLedStatus()
        {
            LightenBoard(BoardIndex);
            var lowBound1 = _daq.RelaysGroupMask.GetLowerBound(1);
            var upperBound1 = _daq.RelaysGroupMask.GetUpperBound(1);
            for (var groupIndex = lowBound1; groupIndex <= upperBound1; groupIndex++)
            {
                LightenRelay(groupIndex);
            }
            Application.DoEvents();
        }

        /// <summary>
        /// 点亮或熄灭子板指示灯
        /// </summary>
        /// <param name="boardIndex"></param>
        private void LightenBoard(int boardIndex)
        {
            var pwr = (_daq.RelaysGroupMask[boardIndex, 6] & 0x30) == 0x30; //k264 k265同时使能
            var kLineAcout1 = (_daq.RelaysGroupMask[boardIndex, 7] & 0x88) == 0x88; //k273 k277同时使能
            var cout2 = (_daq.RelaysGroupMask[boardIndex, 12] & 0x01) == 0x01; //k340使能
            ledBoard[boardIndex].Value = pwr && kLineAcout1 && cout2;
        }

        /// <summary>
        /// 点亮或熄灭某组继电器指示灯
        /// </summary>
        /// <param name="groupIndex"></param>
        private void LightenRelay(int groupIndex)
        {
            for (var i = 0; i < 8; i++)
            {
                var isTrue = _daq.RelaysGroupMask[BoardIndex, groupIndex].GetBit(i) == 1;
                switch (groupIndex)
                {
                    case 0:
                        leds20[i].Value = isTrue;
                        break;
                    case 1:
                        leds21[i].Value = isTrue;
                        break;
                    case 2:
                        leds22[i].Value = isTrue;
                        break;
                    case 3:
                        leds23[i].Value = isTrue;
                        break;
                    case 4:
                        leds24[i].Value = isTrue;
                        break;
                    case 5:
                        leds25[i].Value = isTrue;
                        break;
                    case 6:
                        leds26[i].Value = isTrue;
                        break;
                    case 7:
                        leds27[i].Value = isTrue;
                        break;
                    case 8:
                        leds28[i].Value = isTrue;
                        break;
                    case 9:
                        leds31[i].Value = isTrue;
                        break;
                    case 10:
                        leds32[i].Value = isTrue;
                        break;
                    case 11:
                        leds33[i].Value = isTrue;
                        break;
                    case 12:
                        if (i > 0) break;
                        leds34[i].Value = isTrue;
                        break;

                }

            }

        }

        #endregion

        #region GPIB命令行操作

        private void btnQuery_Click(object sender, EventArgs e)
        {
            try
            {
                var command = txtCommand.Text.Trim();
                if (command.IsNullOrEmpty())
                {
                    MessageBoxHelper.ShowInformationOk("命令行为空");
                    return;
                }
                BllVisa bll = null;
                switch (cmbInstrName.Text)
                {
                    case "PWR":
                        bll = _pwr;
                        break;
                    case "PRS#1":
                        bll = _prs0;
                        break;
                    case "PRS#2":
                        bll = _prs1;
                        break;
                    case "DMM":
                        bll = _dmm;
                        break;
                }
                if (bll == null)
                {
                    MessageBoxHelper.ShowError("相关仪器未启动");
                    return;
                }
                txtResult.Text = bll.Read(command);
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                txtResult.Text = ex.Message;
            }

        }

        #endregion

        private void btnRelayTest_Click(object sender, EventArgs e)
        {
            try
            {
                if(!MessageBoxHelper.ShowQuestion("即将开始线阻测试，耗时较长\n是否继续？"))
                {
                    return;
                }

                toolStatus.Text = @"Relay Testing..." + Settings.Default.AmendResistance;
                if (_daq == null)
                {
                    _daq = new BllDaq();
                    _daq.Initialize();
                }
                if (_prs0 == null)
                {
                    _prs0 = new BllPrs(InstrName.Prs0);
                    _prs0.Initialize();
                }
                if (_prs1 == null)
                {
                    _prs1 = new BllPrs(InstrName.Prs1);
                    _prs1.Initialize();
                }
                if (_dmm == null)
                {
                    _dmm = new BllDmm();
                    _dmm.Initialize();
                }
                var list = new List<double>();
                for (int boardIndex = 0; boardIndex < 8; boardIndex++)
                {
                    if (boardIndex > 0)
                    {
                        _daq.CloseBoard((byte) (boardIndex-1));
                    }
                    _daq.OpenBoard((byte) boardIndex);
                    RefreshLedStatus();
                    for (int squibIndex = 1; squibIndex <= 16; squibIndex++)
                    {
                        _prs0.SetResistance(1.0);
                        _daq.SetFcInReadMode(boardIndex, squibIndex, SquibMode.TooHigh);
                        RefreshLedStatus();
                        var res = _dmm.GetFourWireRes();
                        var value = res - 1.0;
                        if (value >= 0 && value < 1)
                        {
                            list.Add(value);
                        }
                        _daq.SetFcReset(boardIndex, squibIndex);
                    }
                    for (int belt = 1; belt <= 3; belt++)
                    {
                        _prs1.SetResistance(1.0);
                        _daq.SetBeltInReadMode(boardIndex, belt);
                        RefreshLedStatus();
                        var res = _dmm.GetFourWireRes();
                        var value = res - 1.0;
                        if (value >= 0 && value < 1)
                        {
                            list.Add(value);
                        }
                        _daq.SetBeltReset(boardIndex, belt);
                    }
                    for (int sis = 1; sis <= 6; sis++)
                    {
                        _prs1.SetResistance(1.0);
                        _daq.SetSisInReadMode(boardIndex, sis);
                        RefreshLedStatus();
                        var res = _dmm.GetFourWireRes();
                        var value = res - 1.0;
                        if (value >= 0 && value < 1)
                        {
                            list.Add(value);
                        }
                        _daq.SetSisReset(boardIndex, sis);
                    }
                }
                var resValue = list.Average();
                Settings.Default.AmendResistance = resValue;
                Settings.Default.Save();
                MessageBoxHelper.ShowInformationOk("线阻测试已完成，重启后生效");
                toolStatus.Text = @"Ready";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
                LogHelper.WriteErrorLog("RelayTest", ex);
                MessageBoxHelper.ShowError("测试失败\n错误信息：" + ex.Message);
            }
        }
    }
}