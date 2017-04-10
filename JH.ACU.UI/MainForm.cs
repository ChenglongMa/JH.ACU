using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using JH.ACU.BLL;
using JH.ACU.BLL.Config;
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
            RefreshControlByIsBusy();
        }

        #region 属性字段

        private bool _isAuto;

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
            //TODO：随时更新
        }


        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            InstrConfigForm instrConfig;
            switch (e.Tool.Key)
            {
                case "menuFile": // PopupMenuTool
                    // Place code here
                    break;

                case "menuTools": // PopupMenuTool
                    // Place code here
                    break;

                case "menuConfig": // PopupMenuTool
                    // Place code here
                    break;

                case "btnCondition": // ButtonTool
                    var _conditionForm = new InitializationForm();
                    _conditionForm.ShowDialog(this);
                    break;
                case "btnSpecConfig":
                    var specForm=new SpecConfigForm();
                    specForm.Show(this);
                    break;
                case "menuInstrConfig": // PopupMenuTool
                    // Place code here
                    break;

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

                case "btnInstrControl": // ButtonTool
                    var instrConForm = new InstrumentControlForm();
                    instrConForm.ShowDialog(this);
                    break;
                case "btnExport": // ButtonTool
                    // Place code here
                    break;
            }

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
            RefreshControlByIsBusy();
        }

        #region 公有方法


        #endregion

    }
}
