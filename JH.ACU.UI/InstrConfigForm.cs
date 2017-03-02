using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinEditors;
using Ivi.Visa;
using JH.ACU.BLL;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Model;
using JH.ACU.Model.Config.InstrumentConfig;


namespace JH.ACU.UI
{
    public partial class InstrConfigForm : Form
    {
        private readonly List<Instr> _instrs;
        private Instr _instr;

        public InstrConfigForm(InstrName name)
        {
            InitializeComponent();
            _instrs = BllConfig.GetInstrConfigs();
            #region 绑定数据 注意顺序


            #region Serial

            cmbBaudRate.DataSource = ConstParameter.BaudRate;
            cmbDataBits.DataSource = ConstParameter.DataBits;
            cmbParity.DataSource = Enum.GetValues(typeof (SerialParity));
            cmbStopBits.DataSource = Enum.GetValues(typeof (SerialStopBitsMode));
            cmbSerialPort.DataSource = VisaHelper.FindResources(InstrType.Serial);

            #endregion

            #region Gpib

            cmbGpibAddress.DataSource = VisaHelper.FindResources(InstrType.Gpib);

            #endregion

            #region Tcp

            //None

            #endregion

            cmbInstrType.DataSource = Enum.GetValues(typeof (InstrType));

            cmbInstrName.DisplayMember = "Key";
            cmbInstrName.ValueMember = "Value";
            cmbInstrName.DataSource = new BindingSource {DataSource = ConstParameter.InstrNameString};


            #endregion

            cmbInstrName.SelectedValue = name;
        }

        private void SetTabVisible(InstrType type)
        {
            switch (type)
            {
                case InstrType.Gpib:
                    panGpib.Visible = true;
                    panSerial.Visible = false;
                    panTcp.Visible = false;
                    break;
                case InstrType.Serial:
                    panGpib.Visible = false;
                    panSerial.Visible = true;
                    panTcp.Visible = false;

                    break;
                case InstrType.Tcp:
                    panGpib.Visible = false;
                    panSerial.Visible = false;
                    panTcp.Visible = true;

                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        private void cmbInstrName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbInstrName.SelectedValue == null) return;

            var name = (InstrName) cmbInstrName.SelectedValue;
            _instr = BllConfig.GetInstr(name,_instrs);
            cmbInstrType.SelectedItem = _instr.Type;
            switch (_instr.Type)
            {
                case InstrType.Gpib:
                    if (_instr.Gpib == null)
                    {
                        break;
                    }
                    cmbGpibAddress.Text = _instr.Gpib.Port.ToString();
                    break;
                case InstrType.Serial:
                    if (_instr.Serial == null)
                    {
                        break;
                    }
                    cmbSerialPort.Text = _instr.Serial.Port.ToString();
                    cmbBaudRate.Text = _instr.Serial.BaudRate.ToString();
                    cmbParity.SelectedItem = _instr.Serial.Parity;
                    cmbDataBits.SelectedItem = _instr.Serial.DataBits;
                    cmbStopBits.SelectedItem = _instr.Serial.StopBits;

                    break;
                case InstrType.Tcp:
                    if (_instr.TcpIp == null)
                    {
                        break;
                    }
                    tTcpIp.Text = _instr.TcpIp.IpAddress;
                    nTcpPort.Value = _instr.TcpIp.Port;
                    nTcpTimeout.Value = _instr.TcpIp.Timeout;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void cmbInstrType_SelectedIndexChanged(object sender, EventArgs e)
        {
            var type = (InstrType) cmbInstrType.SelectedItem;
            SetTabVisible(type);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                _instr.Type = (InstrType) cmbInstrType.SelectedItem;
                switch (_instr.Type)
                {
                    case InstrType.Gpib:
                        _instr.Serial = null;
                        _instr.TcpIp = null;
                        int i;
                        if (!int.TryParse(cmbGpibAddress.Text, out i))
                        {
                            cmbGpibAddress.Focus();
                            SetError(cmbGpibAddress, "GPIB地址输入有误");
                            return;
                        }
                        _instr.Gpib.Port = i;
                        break;
                    case InstrType.Serial:
                        _instr.TcpIp = null;
                        _instr.Gpib = null;
                        int port;
                        if (!int.TryParse(cmbSerialPort.Text, out port))
                        {
                            cmbSerialPort.Focus();
                            SetError(cmbSerialPort, "端口值输入有误");
                            return;
                        }
                        _instr.Serial.Port = port;
                        int baud;
                        if (!int.TryParse(cmbBaudRate.Text, out baud))
                        {
                            cmbBaudRate.Focus();
                            SetError(cmbBaudRate, "波特率输入有误");
                            return;
                        }
                        _instr.Serial.BaudRate = baud;
                        _instr.Serial.Parity = (SerialParity) cmbParity.SelectedItem;
                        _instr.Serial.DataBits = (short) cmbDataBits.SelectedItem;
                        _instr.Serial.StopBits = (SerialStopBitsMode) cmbStopBits.SelectedItem;
                        break;
                    case InstrType.Tcp:
                        _instr.Serial = null;
                        _instr.Gpib = null;
                        IPAddress ip;
                        if (!IPAddress.TryParse(tTcpIp.Text, out ip))
                        {
                            tTcpIp.Focus();
                            SetError(tTcpIp, "IP地址输入有误");
                            return;
                        }
                        _instr.TcpIp.IpAddress = tTcpIp.Text;
                        _instr.TcpIp.Port = (int) nTcpPort.Value;
                        _instr.TcpIp.Timeout = (int) nTcpTimeout.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                BllConfig.Save(_instr,_instrs);
                MessageBoxHelper.ShowInformationOk("保存成功!");
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError("保存失败，错误信息：\n" + ex.Message);
            }
        }

        private void cmbSerialPort_SelectedIndexChanged(object sender, EventArgs e)
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

        private void panTcp_VisibleChanged(object sender, EventArgs e)
        {
            if (!panTcp.Visible) return;
        }

        private void panSerial_VisibleChanged(object sender, EventArgs e)
        {
            if (!panSerial.Visible) return;
        }

        private void panGpib_VisibleChanged(object sender, EventArgs e)
        {
            if (!panGpib.Visible) return;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void SetError(Control control, string error)
        {
            errorProvider1.Clear();
            errorProvider1.SetError(control, error);
        }

        private void cmbBaudRate_Leave(object sender, EventArgs e)
        {
            if (splitContainer1.Panel1.ContainsFocus)
            {
                return;
            }
            int i;
            if (!int.TryParse(cmbBaudRate.Text, out i))
            {
                cmbBaudRate.Focus();
                SetError(cmbBaudRate, "波特率输入有误");
            }
        }


        private void cmbSerialPort_Leave(object sender, EventArgs e)
        {
            if (splitContainer1.Panel1.ContainsFocus)
            {
                return;
            }
            int i;
            if (!int.TryParse(cmbSerialPort.Text, out i))
            {
                cmbSerialPort.Focus();
                SetError(cmbSerialPort, "端口值输入有误");
            }
        }

        private void cmbGpibAddress_Leave(object sender, EventArgs e)
        {
            if (splitContainer1.Panel1.ContainsFocus)
            {
                return;
            }
            int i;
            if (!int.TryParse(cmbGpibAddress.Text, out i))
            {
                cmbGpibAddress.Focus();
                SetError(cmbGpibAddress, "GPIB地址输入有误");
            }
        }

        private void tTcpIp_Leave(object sender, EventArgs e)
        {
            if (splitContainer1.Panel1.ContainsFocus)
            {
                return;
            }
            IPAddress ip;
            if (!IPAddress.TryParse(tTcpIp.Text, out ip))
            {
                tTcpIp.Focus();
                SetError(tTcpIp, "IP地址输入有误");
            }
        }
    }
}
