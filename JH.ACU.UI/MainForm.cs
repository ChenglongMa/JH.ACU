using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using JH.ACU.BLL;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Model;
using JH.ACU.Model.Config.TestConfig;
using NationalInstruments.UI;

namespace JH.ACU.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            _report=new Report();
            BindingControls();
            _bllMain=new BllMain();
            _bllMain.TestWorker.ProgressChanged += TestWorker_ProgressChanged;
            _bllMain.TestWorker.RunWorkerCompleted+=TestWorker_RunWorkerCompleted;
            SetControlEnabled(!IsBusy);
        }


        #region 属性字段

        private bool _isAuto = true;

        private bool _isSetting;//指示是否设置过配置文件

        private bool IsBusy
        {
            get { return _bllMain != null && _bllMain.TestWorker != null && _bllMain.TestWorker.IsBusy; }
        }

        private readonly BllMain _bllMain;
        private Report _report;//绑定至UI控件
        #endregion

        #region 私有方法

        private void BindingControls()
        {
            if (_report == null) _report=new Report();
            numTempTarget.DataBindings.Add("Value", _report, "SettingTemp");
            numVoltTarget.DataBindings.Add("Value", _report, "SettingVolt");
            numTempReal.DataBindings.Add("Value", _report, "ActualTemp");
            numVoltReal.DataBindings.Add("Vaule", _report, "ActualVolt");
            ckbChamberEnable.DataBindings.Add("Checked", _report, "ChamberEnable");
            numAcuIndex.DataBindings.Add("Vaule", _report, "AcuIndex");
            lblAcuName.DataBindings.Add("Text", _report, "AcuName");
            //TODO:还差两个Grid
        }
        /// <summary>
        /// 设置控件是否可用
        /// </summary>
        private void SetControlEnabled(bool enabled)
        {
            ledAutoRun.Enabled = enabled;
            ledManualRun.Enabled = enabled;
            numTempTarget.InteractionMode = !enabled
                ? NumericEditInteractionModes.Indicator
                : NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                  NumericEditInteractionModes.Text;
            numVoltTarget.InteractionMode = !enabled
                ? NumericEditInteractionModes.Indicator
                : NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                  NumericEditInteractionModes.Text;            
            numAcuIndex.InteractionMode = !enabled
                ? NumericEditInteractionModes.Indicator
                : NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                  NumericEditInteractionModes.Text;
            ckbChamberEnable.Enabled = enabled;
            grbCout.Enabled = enabled;
            toolBarsManager.Tools["btnInitialize"].SharedProps.Enabled = enabled;
            toolBarsManager.Tools["btnStop"].SharedProps.Enabled = !enabled;
            toolBarsManager.Tools["btnRunPause"].SharedProps.Enabled = enabled;
            toolBarsManager.Tools["menuFile"].SharedProps.Enabled = enabled;
            toolBarsManager.Tools["menuConfig"].SharedProps.Enabled = enabled;
            toolBarsManager.Tools["menuTools"].SharedProps.Enabled = enabled;
            //toolBarsManager.Tools["btnRunPause"].SharedProps.Caption = !enabled ? "Pause" : "Run";
            //toolBarsManager.Tools["btnRunPause"].SharedPropsInternal.AppearancesLarge.Appearance.Image =
            //    !enabled ? Properties.Resources.pauseNew : Properties.Resources.StartBlue;
            statusBar.Panels["messBar"].Text = !enabled ? "Test is running..." : "Ready";
            //TODO：添加控件后更新
            txtDtc.Focus();
        }

        private void TestWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var report = e.UserState as Report;
            if (report == null) return;
            _report = report;
            progressBar.Value = e.ProgressPercentage;
            statusBar.Panels["messBar"].Text = report.Message;

        }

        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetControlEnabled(true);
            if (e.Cancelled)
            {
                //操作取消
                statusBar.Panels["messBar"].Text = @"Test has been cancelled.";
                progressBar.Value = 0;
            }
            else
            {
                //正常完成
                statusBar.Panels["messBar"].Text = @"Test has been done!";
                progressBar.Value = 100;
            }
        }

        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            InstrConfigForm instrConfig;
            switch (e.Tool.Key)
            {
                case "btnInitialize": // ButtonTool
                case "btnCondition": // ButtonTool
                    var conditionForm = new InitializationForm();
                    _isSetting = conditionForm.ShowDialog(this)==DialogResult.OK;
                    break;
                case "btnSpecConfig":
                    var specForm = new SpecConfigForm();
                    specForm.Show(this);
                    break;
                    #region 仪器配置按钮

                case "btnAcu": // ButtonTool
                    instrConfig = new InstrConfigForm(InstrName.Acu);
                    instrConfig.ShowDialog(this);
                    break;

                case "btnPwr": // ButtonTool
                    instrConfig = new InstrConfigForm(InstrName.Pwr);
                    instrConfig.ShowDialog(this);
                    break;

                case "btnDmm": // ButtonTool
                    instrConfig = new InstrConfigForm(InstrName.Dmm);
                    instrConfig.ShowDialog(this);
                    break;

                case "btnPrs0": // ButtonTool
                    instrConfig = new InstrConfigForm(InstrName.Prs0);
                    instrConfig.ShowDialog(this);
                    break;

                case "btnPrs1": // ButtonTool
                    instrConfig = new InstrConfigForm(InstrName.Prs1);
                    instrConfig.ShowDialog(this);
                    break;

                case "btnChamber": // ButtonTool
                    instrConfig = new InstrConfigForm(InstrName.Chamber);
                    instrConfig.ShowDialog(this);
                    break;

                    #endregion
                case "btnInstrControl": // ButtonTool
                    var instrConForm = new InstrumentControlForm();
                    instrConForm.ShowDialog(this);
                    break;
                case "btnExport": // ButtonTool
                    // Place code here
                    break;
                case "btnRunPause": // ButtonTool
                    if (e.Tool.SharedProps.Caption == @"Run")
                    {
                        if (_isAuto)
                        {
                            AutoRun();
                        }
                        else
                        {
                            ManualRun();
                        }
                    }
                    else
                    {
                        //TODO:设计暂停方法
                    }
                    break;

                case "btnStop": // ButtonTool
                    if (IsBusy && !_bllMain.TestWorker.CancellationPending)
                    {
                        _bllMain.TestWorker.CancelAsync();
                    }
                    break;



            }

        }
    /// <summary>
    /// 手动执行
    /// </summary>
        private void ManualRun()
        {
            if (ugTestItems.Selected == null)
            {
                MessageBoxHelper.ShowWarning("请选择测试项");
                return;
            }
            var items =
                (from UltraGridRow row in ugTestItems.Selected.Rows select row.ListObject).OfType<SpecItem>()
                    .Select(spec => spec.Index)
                    .ToList();
            var con = new TestCondition
            {
                Temperature = new Temperature
                {
                    Enable = ckbChamberEnable.Checked,
                    Duration = Properties.Settings.Default.Duration,
                    NorTemp = numTempTarget.Value,
                },
                Voltage = new Voltage
                {
                    NorVolt = numVoltTarget.Value,
                },
                TvItems = new Dictionary<TvType, double[]>
                {
                    {TvType.NorTempNorVolt, new[] {numTempTarget.Value, numVoltTarget.Value}}
                },
                AcuItems = new List<AcuItems> {new AcuItems {Index = (int) numAcuIndex.Value, Items = items}}
            };
            _bllMain.Start(con);
            SetControlEnabled(!IsBusy);
        }

        /// <summary>
        /// 自动运行
        /// </summary>
        private void AutoRun()
        {
            if (!_isSetting)
            {
                if(!MessageBoxHelper.ShowQuestion("检测到未初始化设置，是否继续？"))
                {
                    return;
                }
            }
            var con = BllConfig.GetTestCondition();
            if (con==null)
            {
                MessageBoxHelper.ShowError("配置文件缺失");
                return;
            }
            _bllMain.Start(con);
            SetControlEnabled(!IsBusy);
        }

        private void ledAutoRun_Click(object sender, EventArgs e)
        {
            ledManualRun.Value = !ledAutoRun.Value;
            _isAuto = ledAutoRun.Value;

        }

        private void ledManualRun_Click(object sender, EventArgs e)
        {
            ledAutoRun.Value = !ledManualRun.Value;
            _isAuto = !ledManualRun.Value;

        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            //IsBusy = !IsBusy;
            SetControlEnabled(true);
        }

    }
}
