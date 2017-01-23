using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Ivi.Visa;
using JH.ACU.BLL;
using JH.ACU.Lib;
using JH.ACU.Model;


namespace JH.ACU.UI
{
    public partial class InstrConfigForm : Form
    {
        private Instr _instr;
        public InstrConfigForm()
        {
            InitializeComponent();

            #region 绑定数据
            cmbInstrName.DisplayMember = "Key";
            cmbInstrName.ValueMember = "Value";
            cmbInstrName.DataSource = new BindingSource { DataSource = ConstParameter.InstrNameString };

            cmbInstrType.DataSource = Enum.GetValues(typeof (InstrType));

            cmbBaudRate.DataSource = ConstParameter.BaudRate;
            cmbDataBits.DataSource = ConstParameter.DataBits;
            cmbParity.DataSource = Enum.GetValues(typeof (SerialParity));
            cmbStopBits.DataSource = Enum.GetValues(typeof(SerialStopBitsMode));
            UpdateControls();

            #endregion

        }

        private void UpdateControls()
        {
            cmbInstrType.SelectedIndex = cmbInstrType.Items.IndexOf(_instr.Type);
            //cmbPortNum
            cmbBaudRate.SelectedIndex = cmbBaudRate.Items.IndexOf(_instr.BaudRate);
            cmbDataBits.SelectedIndex = cmbDataBits.Items.IndexOf(_instr.DataBits);
            cmbStopBits.SelectedIndex = cmbStopBits.Items.IndexOf(_instr.StopBits);
            cmbParity.SelectedIndex = cmbParity.Items.IndexOf(_instr.Parity);
        }

        private void cmbInstrName_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            var name = (InstrName) cmbInstrName.SelectedValue;
            _instr = BllConfig.GetInstr(name) ?? new Instr {Name = name};
            UpdateControls();
        }

        private void cmbInstrType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = (InstrType) cmbInstrType.SelectedValue;
            var list=VisaHelper.FindResources(type);
            cmbPortNum.DataSource = list;
            cmbPortNum.SelectedItem = _instr.PortNumber;
            SetControlVisible(type);
        }

        private void SetControlVisible(InstrType instrType)
        {
            switch (instrType)
            {
                case InstrType.Gpib:
                    lblPortNum.Text = @"GPIB地址";
                    lblBaudRate.Visible = false;
                    lblDataBits.Visible = false;
                    lblParity.Visible = false;
                    lblStopBits.Visible = false;

                    cmbBaudRate.Visible = false;
                    cmbDataBits.Visible = false;
                    cmbParity.Visible = false;
                    cmbStopBits.Visible = false;
                    break;
                case InstrType.Serial:
                    lblPortNum.Text = @"COM端口号";
                    lblBaudRate.Visible = true;
                    lblDataBits.Visible = true;
                    lblParity.Visible = true;
                    lblStopBits.Visible = true;

                    cmbBaudRate.Visible = true;
                    cmbDataBits.Visible = true;
                    cmbParity.Visible = true;
                    cmbStopBits.Visible = true;

                    break;
                case InstrType.Tcp:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("instrType", instrType, null);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //try
            //{
                _instr.Type = (InstrType) cmbInstrType.SelectedItem;
                _instr.PortNumber = Convert.ToInt32(cmbPortNum.SelectedItem);
                _instr.BaudRate = (int) cmbBaudRate.SelectedItem;
                _instr.Parity = (SerialParity) cmbParity.SelectedItem;
                _instr.DataBits = (short) cmbDataBits.SelectedItem;
                _instr.StopBits = (SerialStopBitsMode) cmbStopBits.SelectedItem;

                BllConfig.Save(_instr);
                MessageBoxHelper.ShowInformationOK("保存成功!");
            //}
            //catch (Exception ex)
            //{
            //    MessageBoxHelper.ShowError("保存失败，错误信息：\n" + ex.Message);
            //}
        }

        private void cmbPortNum_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmbBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmbParity_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmbDataBits_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void cmbStopBits_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
