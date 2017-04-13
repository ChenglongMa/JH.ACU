using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using JH.ACU.BLL;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Lib.GridConfig;
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
            SetGridTag();
            _report = new Report();

            #region Chart Binding

            _dataTable = new DataTable();
            _dataTable.Columns.Add(new DataColumn("legend"));
            _actualValueRow = _dataTable.NewRow();
            _settingValueRow = _dataTable.NewRow();
            _actualValueRow["legend"] = "Actual Value";
            _settingValueRow["legend"] = "Setting Value";
            _dataTable.Rows.Add(_actualValueRow);
            _dataTable.Rows.Add(_settingValueRow);
            chart.DataSource = _dataTable;

            #endregion

            _fieldsProgress = BllFieldConfig.LoadFieldsInfo("SpecProgress.xml");
            _fieldsResult = BllFieldConfig.LoadFieldsInfo("SpecResult.xml");
            _bllMain = new BllMain();
            _bllMain.TestWorker.ProgressChanged += TestWorker_ProgressChanged;
            _bllMain.TestWorker.RunWorkerCompleted += TestWorker_RunWorkerCompleted;
            SetControlEnabled(!IsBusy);
            BindingControls(_report);
        }


        #region 属性字段

        private bool _isAuto = true;

        private bool _isSetting; //指示是否设置过配置文件

        private bool IsBusy
        {
            get { return _bllMain != null && _bllMain.TestWorker != null && _bllMain.TestWorker.IsBusy; }
        }

        private readonly BllMain _bllMain;
        private readonly Report _report; //绑定至UI控件
        private readonly DataTable _dataTable;
        private readonly DataRow _actualValueRow;
        private readonly DataRow _settingValueRow;
        private readonly List<FieldMetaInfo> _fieldsProgress;
        private readonly List<FieldMetaInfo> _fieldsResult;
        private InitializationForm _conditionForm;
        #endregion

        #region 私有方法

        private void BindingChart(double actualValue, double settingValue)
        {
            var name = DateTime.Now.ToString("T");
            var col = new DataColumn(name, typeof (double));
            if (!_dataTable.Columns.Contains(name))
            {
                _dataTable.Columns.Add(col);
                _actualValueRow[name] = actualValue;
                _settingValueRow[name] = settingValue;
            }
            chart.DataBind();
        }
        private void BindingControls(Report report)
        {
            numTempTarget.DataBindings.Add("Value", report, "SettingTemp");
            numVoltTarget.DataBindings.Add("Value", report, "SettingVolt");
            numTempReal.DataBindings.Add("Value", report, "ActualTemp");
            numVoltReal.DataBindings.Add("Value", report, "ActualVolt");
            ckbChamberEnable.DataBindings.Add("Checked", report, "ChamberEnable");
            numAcuIndex.DataBindings.Add("Value", report, "AcuIndex");
            lblAcuName.DataBindings.Add("Text", report, "AcuName");
            BindingGridToTestItems();
            BindingToResultGrid(ugLTLV);
            BindingToResultGrid(ugLTNV);
            BindingToResultGrid(ugLTHV);
            BindingToResultGrid(ugNTLV);
            BindingToResultGrid(ugNTNV);
            BindingToResultGrid(ugNTHV);
            BindingToResultGrid(ugHTLV);
            BindingToResultGrid(ugHTNV);
            BindingToResultGrid(ugHTHV);
            BindingChart(_report.ActualTemp, _report.SettingTemp);
        }

        private void BindingGridToTestItems()
        {
            var list = _report.SpecUnitsDict.ContainsKey(_bllMain.SelectedTvType)
                ? new BindingList<SpecItem>(_report.SpecUnitsDict[_bllMain.SelectedTvType])
                : new BindingList<SpecItem>(BllConfig.GetSpecItems());
            ugTestItems.DataSource = list;
            ugTestItems.SetStyle(_fieldsProgress);
            ugTestItems.SetGridDefaultStyle();
            ugTestItems.DisplayLayout.Override.CellClickAction=CellClickAction.RowSelect;
            foreach (var row in ugTestItems.Rows)
            {
                var cell = row.Cells["ResultInfo"];
                var text = cell.Text.ToLower();
                if (text.Contains("failed"))
                {
                    cell.Appearance.ForeColor=Color.Red;
                }
                else if (text.Contains("passed"))
                {
                    cell.Appearance.ForeColor=Color.Green;
                }
                else if (text.Contains("cancelled"))
                {
                    cell.Appearance.ForeColor = Color.Orange;
                }
                else
                {
                    cell.Appearance.ForeColor = DefaultForeColor;
                }
            }
        }

        private void BindingToResultGrid(UltraGrid grid)
        {
            var tvType = (TvType) grid.Tag;
            var list = _report.SpecUnitsDict.ContainsKey(tvType)
                ? new BindingList<SpecItem>(_report.SpecUnitsDict[tvType])
                : new BindingList<SpecItem>(BllConfig.GetSpecItems());
            grid.DataSource = list;
            grid.SetStyle(_fieldsResult);
            grid.SetGridDefaultStyle();
        }

        private void SetGridTag()
        {
            pageLL.Tag = ugLTLV.Tag = TvType.LowTempLowVolt;
            pageLN.Tag = ugLTNV.Tag = TvType.LowTempNorVolt;
            pageLH.Tag = ugLTHV.Tag = TvType.LowTempHighVolt;
            pageNL.Tag = ugNTLV.Tag = TvType.NorTempLowVolt;
            pageNN.Tag = ugNTNV.Tag = TvType.NorTempNorVolt;
            pageNH.Tag = ugNTHV.Tag = TvType.NorTempHighVolt;
            pageHL.Tag = ugHTLV.Tag = TvType.HighTempLowVolt;
            pageHN.Tag = ugHTNV.Tag = TvType.HighTempNorVolt;
            pageHH.Tag = ugHTHV.Tag = TvType.HighTempHighVolt;
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
            _report.DeepCopy(report);
            var progress = e.ProgressPercentage;
            progressBar.Value = progress <= 100 ? progress : 100;
            statusBar.Panels["messBar"].Text = report.Message;
            BindingGridToTestItems();
            BindingToResultGrid(ugLTLV);
            BindingToResultGrid(ugLTNV);
            BindingToResultGrid(ugLTHV);
            BindingToResultGrid(ugNTLV);
            BindingToResultGrid(ugNTNV);
            BindingToResultGrid(ugNTHV);
            BindingToResultGrid(ugHTLV);
            BindingToResultGrid(ugHTNV);
            BindingToResultGrid(ugHTHV);
            BindingChart(_report.ActualTemp, _report.SettingTemp);
            foreach (TabPage page in tabControl1.TabPages)
            {
                var tvType = (TvType) page.Tag;
                if (tvType == _bllMain.SelectedTvType)
                {
                    tabControl1.SelectedTab = page;
                    break;
                }
            }
        }

        private void TestWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    throw e.Error;
                }
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
            finally
            {
                SetControlEnabled(true);
                _bllMain.CloseAllInstrs();
            }

        }

        private void ultraToolbarsManager1_ToolClick(object sender, ToolClickEventArgs e)
        {
            InstrConfigForm instrConfig;
            switch (e.Tool.Key)
            {
                case "btnInitialize": // ButtonTool
                case "btnCondition": // ButtonTool
                    _conditionForm = new InitializationForm();
                    _isSetting = _conditionForm.ShowDialog(this) == DialogResult.OK;
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
                    //SetControlEnabled(true);
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
                    NorTemp = new Temp{Value = numTempTarget.Value,Delay = 0}
                },
                Voltage = new Voltage
                {
                    NorVolt = numVoltTarget.Value,
                },
                TvItems = new Dictionary<TvType, double[]>
                {
                    {TvType.NorTempNorVolt, new[] {numTempTarget.Value, numVoltTarget.Value,0}}
                },
                AcuItems = new List<AcuItems> {new AcuItems {Index = (int) numAcuIndex.Value-1, Items = items}}
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
                if (!MessageBoxHelper.ShowQuestion("检测到未初始化设置，是否继续？"))
                {
                    return;
                }
            }
            var con = _conditionForm.TestCondition;
            if (con == null)
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
            _report.ActualTemp = 10.3;
            _report.ActualVolt = 6.5;
            _report.SettingTemp = 30;
            _report.SettingVolt += 12;
            _report.AcuIndex = 4;
            _report.AcuName = "测试ACU";

        }

    }
}