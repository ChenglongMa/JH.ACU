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
        private byte BoardIndex { get { return (byte) (numBoardIndex.Value - 1); } }
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
                var index = (byte) numRelayIndex.Value;
                if (index == 300 || index == 301 || index == 302)
                {
                    _daq.SetMainRelayStatus(index, true);
                }
                else
                {
                    _daq.SetSubRelayStatus(BoardIndex, index, true);
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
                var index = (byte) numRelayIndex.Value;
                if (index == 300 || index == 301 || index == 302)
                {
                    _daq.SetMainRelayStatus(index, false);
                }
                else
                {
                    _daq.SetSubRelayStatus(BoardIndex, index, false);
                }
                toolStatus.Text = @"Successful";
            }
            catch (Exception ex)
            {
                toolStatus.Text = ex.Message;
            }
        }
        private void SetLedStatus()
        {
            var lowBound0 = _daq.RelaysGroupMask.GetLowerBound(0);
            var upperBound0 = _daq.RelaysGroupMask.GetUpperBound(0);
            var lowBound1 = _daq.RelaysGroupMask.GetLowerBound(1);
            var upperBound1 = _daq.RelaysGroupMask.GetLowerBound(1);
            for (var i = lowBound0; i <= upperBound0; i++)
            {
                for (var j = lowBound1; j < upperBound1; j++)
                {
                    if (_daq.RelaysGroupMask[i,j]!=0)
                    {
                        //i为板号,j为继电器组号
                    }
                }
            }

        }

        #endregion



    }
}
