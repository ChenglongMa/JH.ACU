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
            _bllMain=new BllMain();
            _bllMain.TestWorker.ProgressChanged += TestWorker_ProgressChanged;
            _bllMain.TestWorker.RunWorkerCompleted+=TestWorker_RunWorkerCompleted;
            RefreshControlByIsBusy();
        }


        #region 属性字段

        private bool _isAuto = true;

        private bool _isSetting;//指示是否设置过配置文件

        private bool IsBusy
        {
            get { return _bllMain != null && _bllMain.TestWorker != null && _bllMain.TestWorker.IsBusy; }
        }

        private readonly BllMain _bllMain;
        #endregion

        #region 私有方法

        /// <summary>
        /// 根据测试线程状态更新按钮状态
        /// </summary>
        private void RefreshControlByIsBusy()
        {
            ledAutoRun.Enabled = !IsBusy;
            ledManualRun.Enabled = !IsBusy;
            numTempTarget.InteractionMode = IsBusy
                ? NumericEditInteractionModes.Indicator
                : NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                  NumericEditInteractionModes.Text;
            numVoltTarget.InteractionMode = IsBusy
                ? NumericEditInteractionModes.Indicator
                : NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                  NumericEditInteractionModes.Text;            
            numAcuIndex.InteractionMode = IsBusy
                ? NumericEditInteractionModes.Indicator
                : NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                  NumericEditInteractionModes.Text;
            ckbChamberEnable.Enabled = !IsBusy;
            grbCout.Enabled = !IsBusy;
            toolBarsManager.Tools["btnInitialize"].SharedProps.Enabled = !IsBusy;
            toolBarsManager.Tools["btnStop"].SharedProps.Enabled = IsBusy;
            toolBarsManager.Tools["btnRunPause"].SharedProps.Caption = IsBusy ? "Pause" : "Run";
            toolBarsManager.Tools["btnRunPause"].SharedPropsInternal.AppearancesLarge.Appearance.Image =
                IsBusy ? Properties.Resources.pauseNew : Properties.Resources.StartBlue;
            statusBar.Panels["messBar"].Text = IsBusy ? "Test is running..." : "Ready";

            //TODO：添加控件后更新
            txtDtc.Focus();
        }

        /// <summary>
        /// 将控件状态修改为初始状态
        /// </summary>
        private void ResetControl()
        {
            ledAutoRun.Enabled = true;
            ledManualRun.Enabled = true;
            numTempTarget.InteractionMode = NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                                            NumericEditInteractionModes.Text;
            numVoltTarget.InteractionMode = NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                                            NumericEditInteractionModes.Text;
            numAcuIndex.InteractionMode = NumericEditInteractionModes.ArrowKeys | NumericEditInteractionModes.Buttons |
                                          NumericEditInteractionModes.Text;
            ckbChamberEnable.Enabled = true;
            grbCout.Enabled = true;
            toolBarsManager.Tools["btnInitialize"].SharedProps.Enabled = true;
            toolBarsManager.Tools["btnStop"].SharedProps.Enabled = false;
            toolBarsManager.Tools["btnRunPause"].SharedProps.Caption = @"Run";
            toolBarsManager.Tools["btnRunPause"].SharedPropsInternal.AppearancesLarge.Appearance.Image =
                Properties.Resources.StartBlue;
            //TODO：添加控件后更新
        }

        private void TestWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ResetControl();
            if (e.Cancelled)
            {
                //操作取消
                statusBar.Panels["messBar"].Text = @"Test has been cancelled.";

                return;
            }
            else
            {
                statusBar.Panels["messBar"].Text = @"Test has been done!";
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
                TvItems = new List<double[]> {new[] {numTempTarget.Value, numVoltTarget.Value}},
                AcuItems = new List<AcuItems> {new AcuItems {Index = (int) numAcuIndex.Value, Items = items}}
            };
            _bllMain.Start(con);
            RefreshControlByIsBusy();
        }

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
            RefreshControlByIsBusy();
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
            ResetControl();
        }

        #region 公有方法


        #endregion

    }
}
