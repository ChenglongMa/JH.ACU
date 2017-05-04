using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Documents.Excel;
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
            _bllMain.ChamberReset.RunWorkerCompleted += ChamberReset_RunWorkerCompleted;
            _bllMain.ChamberReset.ProgressChanged += TestWorker_ProgressChanged;
            SetControlEnabled(!IsBusy);
            BindingControls(_report);

            #region ugTestItems

            if (ugTestItems.Rows.Count > 0)
            {
                ugTestItems.Rows[0].Selected = true;
            }

            #endregion

        }

        private void ChamberReset_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetControlEnabled(true);
            MessageBoxHelper.ShowInformationOk("测试已完成");
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
            ugTestItems.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
            foreach (var row in ugTestItems.Rows)
            {
                var cell = row.Cells["ResultInfo"];
                var text = cell.Text.ToLower();
                if (text.Contains("failed"))
                {
                    cell.Appearance.ForeColor = Color.Red;
                }
                else if (text.Contains("passed"))
                {
                    cell.Appearance.ForeColor = Color.Green;
                }
                else if (text.Contains("cancelled"))
                {
                    cell.Appearance.ForeColor = Color.Orange;
                }
                else
                {
                    cell.Appearance.ForeColor = DefaultForeColor;
                }
                cell.Appearance.BackColor = text.Contains("progressing") ? Color.Aquamarine : DefaultBackColor;

            }
        }

        /// <summary>
        /// 为Grid绑定测试结果
        /// </summary>
        /// <param name="grid"></param>
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

        /// <summary>
        /// 设置Tab Grid的Tag
        /// </summary>
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
            if (_report.ChamberEnable)
            {
                BindingChart(_report.ActualTemp, _report.SettingTemp);
            }
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
            string msg = null;
            try
            {
                if (e.Error != null)
                {
                    LogHelper.WriteErrorLog("MainForm", e.Error);
                    statusBar.Panels["messBar"].Text = @"测试过程有错误，详情请查看日志";
                    progressBar.Value = 0;
                    msg = @"测试过程有错误，详情请查看日志";
                }
                if (e.Cancelled)
                {
                    //操作取消
                    statusBar.Panels["messBar"].Text = @"Test has been cancelled.";
                    progressBar.Value = 0;
                    msg = "测试已取消";
                }
                else
                {
                    //正常完成
                    statusBar.Panels["messBar"].Text = @"Test has been done!";
                    progressBar.Value = 100;
                    msg = "测试已完成";
                }
                if (_bllMain.ChamberReset != null && _bllMain.ChamberReset.IsBusy)
                {
                    msg += "\n请等待温箱恢复至常温状态";
                }
            }
            finally
            {
                if (_bllMain.ChamberReset == null || !_bllMain.ChamberReset.IsBusy)
                {
                    SetControlEnabled(true);
                }
                if (!msg.IsNullOrEmpty())
                {
                    MessageBoxHelper.ShowInformationOk(msg);
                }
                else
                {
                    MessageBoxHelper.ShowError("测试发生未知错误");
                }
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
                    ExportToExcel();
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
        /// 导出至Excel中
        /// </summary>
        private void ExportToExcel()
        {
            try
            {
                if (saveFileDialog1.ShowDialog(this) != DialogResult.OK) return;
                var fileName = saveFileDialog1.FileName;
                var book = new Workbook(WorkbookFormat.Excel97To2003);
                var sheetTotal = book.Worksheets.Add(pageTotal.Text);
                var sheetLtLv = book.Worksheets.Add(pageLL.Text);
                var sheetLtNv = book.Worksheets.Add(pageLN.Text);
                var sheetLtHv = book.Worksheets.Add(pageLH.Text);
                var sheetNtLv = book.Worksheets.Add(pageNL.Text);
                var sheetNtNv = book.Worksheets.Add(pageNN.Text);
                var sheetNtHv = book.Worksheets.Add(pageNH.Text);
                var sheetHtLv = book.Worksheets.Add(pageHL.Text);
                var sheetHtNv = book.Worksheets.Add(pageHN.Text);
                var sheetHtHv = book.Worksheets.Add(pageHH.Text);
                ultraGridExcelExporter1.Export(ugTotal, sheetTotal);
                ultraGridExcelExporter1.Export(ugLTLV, sheetLtLv);
                ultraGridExcelExporter1.Export(ugLTNV, sheetLtNv);
                ultraGridExcelExporter1.Export(ugLTHV, sheetLtHv);
                ultraGridExcelExporter1.Export(ugNTLV, sheetNtLv);
                ultraGridExcelExporter1.Export(ugNTNV, sheetNtNv);
                ultraGridExcelExporter1.Export(ugNTHV, sheetNtHv);
                ultraGridExcelExporter1.Export(ugHTLV, sheetHtLv);
                ultraGridExcelExporter1.Export(ugHTNV, sheetHtNv);
                ultraGridExcelExporter1.Export(ugHTHV, sheetHtHv);
                book.Save(fileName);
                MessageBoxHelper.ShowInformationOk("File saved successfully.");
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog("MainForm", ex, "保存测试结果失败");
                MessageBoxHelper.ShowError("File saved failed!\n" + ex.Message);
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
                    Duration = BLL.Properties.Settings.Default.Duration,
                    NorTemp = new Temp {Value = numTempTarget.Value, Delay = 0}
                },
                Voltage = new Voltage
                {
                    NorVolt = numVoltTarget.Value,
                },
                TvItems = new Dictionary<TvType, double[]>
                {
                    {TvType.NorTempNorVolt, new[] {numTempTarget.Value, numVoltTarget.Value, 0}}
                },
                AcuItems = new List<AcuItems> {new AcuItems {Index = (int) numAcuIndex.Value - 1, Items = items}}
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
            toolBarsManager.Tools["btnInitialize"].SharedProps.Enabled = _isAuto;
        }

        private void ledManualRun_Click(object sender, EventArgs e)
        {
            ledAutoRun.Value = !ledManualRun.Value;
            _isAuto = !ledManualRun.Value;
            toolBarsManager.Tools["btnInitialize"].SharedProps.Enabled = _isAuto;
        }

        #endregion

    }
}