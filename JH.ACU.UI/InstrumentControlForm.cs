using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using JH.ACU.BLL.Instruments;
using JH.ACU.Lib;
using JH.ACU.Model;
using NationalInstruments.UI;

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
            cmbInstrName.DataSource = new BindingSource {DataSource = ConstParameter.InstrNameString};
            cmbResIndex.DisplayMember = "Key";
            cmbResIndex.ValueMember = "Value";
            var dic = new Dictionary<string, InstrName> {{"PRS#1", InstrName.Prs0}, {"PRS#2", InstrName.Prs1}};
            cmbResIndex.DataSource = new BindingSource {DataSource = dic};
            SetControlStatus(InstrName.Daq, false);
            SetControlStatus(InstrName.Prs0, false);
            SetControlStatus(InstrName.Dmm, false);
            SetControlStatus(InstrName.Pwr, false);
        }

        #endregion

        #region 属性、字段

        private BllPwr _pwr;
        private BllPrs _prs;
        private BllChamber _chamber;
        private BllDmm _dmm;
        private BllDaq _daq;

        private byte BoardIndex
        {
            get { return (byte) (numBoardIndex.Value - 1); }
        }

        #endregion

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
                case InstrName.Prs1:
                    swResOpen.Caption = isOpen;
                    btnSetRes.Enabled = enable;
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

        private void swResOpen_StateChanging(object sender, ActionCancelEventArgs e)
        {
            try
            {
                if (!swResOpen.Value)
                {
                    _prs = new BllPrs((InstrName) cmbResIndex.SelectedValue);
                    _prs.Initialize();
                    SetControlStatus(InstrName.Prs0, true);
                }
                else
                {
                    _prs.Dispose();
                    SetControlStatus(InstrName.Prs0, true);
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
                _prs.SetResistance((double) numSetRes.Value);
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }

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
    }
}