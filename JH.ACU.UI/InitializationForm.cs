using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Lib.GridConfig;
using JH.ACU.Model.Config.TestConfig;
using NationalInstruments.Restricted;
using ColumnStyle = Infragistics.Win.UltraWinGrid.ColumnStyle;

namespace JH.ACU.UI
{
    public partial class InitializationForm : Form
    {

        public InitializationForm()
        {
            InitializeComponent();
            var t=new AcuItems();
            t.Items=new List<int>();
            t.Items.Add(2);
            t.Items.Add(3);
            t.Items.Add(1);
            //bug:Add无效，暂未解决 2017-04-13
            _fields = BllFieldConfig.LoadFieldsInfo("SpecInitialForm.xml");
            TestCondition = BllConfig.GetTestCondition(); //从默认路径获取
            BindingSourceTable();
            DisplayTestCondition(TestCondition);
        }

        #region 属性字段

        private readonly List<FieldMetaInfo> _fields;
        public TestCondition TestCondition { get; private set; }

        #endregion

        #region 界面变化

        private void ckbChamberEnable_CheckedChanged(object sender, EventArgs e)
        {
            grbTvSetting.Enabled = grbTvValue.Enabled = ckbChamberEnable.Checked;
        }

        //private void ckbAcuAll_Click(object sender, EventArgs e)
        //{
        //    var count = flowAcu.Controls.Count;
        //    for (int i = 0; i < count - 1; i++)
        //    {
        //        ((CheckBox) flowAcu.Controls[i]).Checked = ckbAcuAll.Checked;
        //    }

        //}

        private void ckbTvs_Click(object sender, EventArgs e)
        {
            var isChecked = ((CheckBox) sender).Checked;
            var count = flowTvSetting.Controls.Count;
            for (int i = 0; i < count - 1; i++)
            {
                if (((CheckBox) flowTvSetting.Controls[i]).Checked != isChecked)
                {
                    ckbTvAll.CheckState = CheckState.Indeterminate;
                    return;
                }
            }
            ckbTvAll.CheckState = CheckState.Checked;
            ckbTvAll.Checked = isChecked;
        }

        private void ckbTvAll_Click(object sender, EventArgs e)
        {
            var count = flowTvSetting.Controls.Count;
            for (int i = 0; i < count - 1; i++)
            {
                ((CheckBox) flowTvSetting.Controls[i]).Checked = ckbTvAll.Checked;
            }

        }

        //private void ckbAcus_Click(object sender, EventArgs e)
        //{
        //    var isChecked = ((CheckBox) sender).Checked;
        //    var count = flowAcu.Controls.Count;
        //    for (int i = 0; i < count - 1; i++)
        //    {
        //        if (((CheckBox) flowAcu.Controls[i]).Checked != isChecked)
        //        {
        //            ckbAcuAll.CheckState = CheckState.Indeterminate;
        //            return;
        //        }
        //    }
        //    ckbAcuAll.CheckState = CheckState.Checked;
        //    ckbAcuAll.Checked = isChecked;

        //}

        #endregion

        private void btnDefault_Click(object sender, EventArgs e)
        {
            numDuration.Value = (decimal) Properties.Settings.Default.Duration;
            numHighTemp.Value = (decimal) Properties.Settings.Default.HighTemp;
            numLowTemp.Value = (decimal) Properties.Settings.Default.LowTemp;
            numNorTemp.Value = (decimal) Properties.Settings.Default.NorTemp;
            numHighVolt.Value = (decimal) Properties.Settings.Default.HighVolt;
            numNorVolt.Value = (decimal) Properties.Settings.Default.NorVolt;
            numLowVolt.Value = (decimal) Properties.Settings.Default.LowVolt;
            ckbChamberEnable.Checked = Properties.Settings.Default.TempEnable;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            TestCondition = BuildTestCondition();
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var fileName = saveFileDialog1.FileName;
                TestCondition.SaveToFile(fileName); //保存到指定路径一份
                MessageBoxHelper.ShowInformationOk("File saved successfully.");
            }
            TestCondition.SaveToFile(); //保存到默认路径一份
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var fileName = openFileDialog1.FileName;
                var testCondition = BllConfig.GetTestCondition(fileName);
                DisplayTestCondition(testCondition);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            TestCondition = BuildTestCondition();
            TestCondition.SaveToFile(); //保存到默认路径
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 将配置的值显示到控件上
        /// </summary>
        /// <param name="testCondition"></param>
        private void DisplayTestCondition(TestCondition testCondition)
        {
            if (testCondition == null) return;
            numDuration.Value = (decimal) testCondition.Temperature.Duration;
            numHighTemp.Value = (decimal) testCondition.Temperature.HighTemp.Value;
            numLowTemp.Value = (decimal) testCondition.Temperature.LowTemp.Value;
            numNorTemp.Value = (decimal) testCondition.Temperature.NorTemp.Value;
            numHighVolt.Value = (decimal) testCondition.Voltage.HighVolt;
            numNorVolt.Value = (decimal) testCondition.Voltage.NorVolt;
            numLowVolt.Value = (decimal) testCondition.Voltage.LowVolt;
            ckbChamberEnable.Checked = testCondition.Temperature.Enable;
            SetAcuItems(testCondition.AcuItems);
        }

        /// <summary>
        /// 控件值赋给配置
        /// </summary>
        /// <returns></returns>
        private TestCondition BuildTestCondition()
        {
            var temp = new Temperature
            {
                Duration = (double) numDuration.Value,
                HighTemp = new Temp {Value = (double) numHighTemp.Value, Delay = (double) numHighDelay.Value},
                LowTemp = new Temp {Value = (double) numLowTemp.Value, Delay = (double) numLowDelay.Value},
                NorTemp = new Temp {Value = (double) numNorTemp.Value, Delay = (double) numNorDelay.Value},
                Enable = ckbChamberEnable.Checked
            };
            var volt = new Voltage
            {
                HighVolt = (double) numHighVolt.Value,
                NorVolt = (double) numNorVolt.Value,
                LowVolt = (double) numLowVolt.Value
            };
            var tvItems = new Dictionary<TvType, double[]>();
            if (ckbLL.Checked)
            {
                tvItems.Add(TvType.LowTempLowVolt, new[] {temp.LowTemp.Value, volt.LowVolt, temp.LowTemp.Delay});
            }
            if (ckbLN.Checked)
            {
                tvItems.Add(TvType.LowTempNorVolt, new[] {temp.LowTemp.Value, volt.NorVolt, temp.LowTemp.Delay});
            }
            if (ckbLH.Checked)
            {
                tvItems.Add(TvType.LowTempHighVolt, new[] {temp.LowTemp.Value, volt.HighVolt, temp.LowTemp.Delay});
            }
            if (ckbNL.Checked)
            {
                tvItems.Add(TvType.NorTempLowVolt, new[] {temp.NorTemp.Value, volt.LowVolt, temp.NorTemp.Delay});
            }
            if (ckbNN.Checked)
            {
                tvItems.Add(TvType.NorTempNorVolt, new[] {temp.NorTemp.Value, volt.NorVolt, temp.NorTemp.Delay});
            }
            if (ckbNH.Checked)
            {
                tvItems.Add(TvType.NorTempHighVolt, new[] {temp.NorTemp.Value, volt.HighVolt, temp.NorTemp.Delay});
            }
            if (ckbHL.Checked)
            {
                tvItems.Add(TvType.HighTempLowVolt, new[] {temp.HighTemp.Value, volt.LowVolt, temp.HighTemp.Delay});
            }
            if (ckbHN.Checked)
            {
                tvItems.Add(TvType.HighTempNorVolt, new[] {temp.HighTemp.Value, volt.NorVolt, temp.HighTemp.Delay});
            }
            if (ckbHH.Checked)
            {
                tvItems.Add(TvType.HighTempHighVolt, new[] {temp.HighTemp.Value, volt.HighVolt, temp.HighTemp.Delay});
            }
            if (tvItems.IsNullOrEmpty())
            {
                tvItems.Add(TvType.NorTempNorVolt, new[] {temp.NorTemp.Value, volt.NorVolt, temp.NorTemp.Delay});
            }
            return new TestCondition
            {
                Temperature = temp,
                Voltage = volt,
                TvItems = tvItems,
                AcuItems = GetAcuItems(),
            };
        }
        /// <summary>
        /// 将Grid中选中的值转换为AcuItems
        /// </summary>
        /// <returns></returns>
        private List<AcuItems> GetAcuItems()
        {
            var res = new List<AcuItems>();
            foreach (var row in dgSource.Rows)
            {
                for (int i = 1; i < 9; i++)
                {
                    if (!Convert.ToBoolean(row.Cells["Acu" + i].Text)) continue;
                    var acuIndex = i - 1;
                    // ReSharper disable once AccessToModifiedClosure
                    var index = res.IndexOf(a => a.Index == acuIndex);
                    var acuItem = res.Find(a => a.Index == acuIndex);

                    if (acuItem!=null)
                    {
                        acuItem.Items.Add(Convert.ToInt32(row.Cells[0].Text));
                    }
                    else
                    {
                        res.Add(new AcuItems{Index = acuIndex,Items = new List<int>{Convert.ToInt32(row.Cells[0].Text)}});
                    }
                }
            }
            return res.OrderBy(a=>a.Index).ToList();
        }

        private void SetAcuItems(List<AcuItems> items)
        {
            for (int i = 1; i < 9; i++)
            {
                var key = "Acu" + i;
                var acuIndex = i - 1;
                // ReSharper disable once AccessToModifiedClosure
                var index = items.IndexOf(a => a.Index == acuIndex);
                if (index < 0 || items[index].Items.IsNullOrEmpty()) continue;
                foreach (var item in items[index].Items)
                {

                    if (!dgSource.DisplayLayout.Bands[0].Columns.Exists(key))
                    {
                        dgSource.DisplayLayout.Bands[0].Columns.Add(key, "ACU #" + i);
                    }
                    var row = dgSource.Rows.FirstOrDefault(r => r.Cells[0].Text == item.ToString());
                    if (row != null)
                    {
                        row.Cells[key].SetValue(true, false);
                        row.Cells[key].Appearance.BackColor = Color.Coral;

                    }
                }
            }
        }
        private void BindingSourceTable()
        {
            var list = new BindingList<SpecItem>(BllConfig.GetSpecItems());
            dgSource.DataSource = list;
            dgSource.SetStyle(_fields);
            for (int i = 1; i < 9; i++)
            {
                dgSource.DisplayLayout.Bands[0].Columns.Add("Acu" + i, "ACU #" + i);
                var col = dgSource.DisplayLayout.Bands[0].Columns["Acu" + i];
                col.DataType = typeof (bool);
                col.Style = ColumnStyle.CheckBox;
                col.CellActivation = Activation.AllowEdit;
                col.DefaultCellValue = false;
            }
        }

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="control"></param>
        /// <param name="message"></param>
        private void SetError(Control control, string message)
        {
            errorProvider1.Clear();
            errorProvider1.SetError(control, message);
        }

        private void dgSource_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key.Contains("Acu"))
            {
                e.Cell.ActiveAppearance.BackColor = Convert.ToBoolean(e.Cell.Text) ? Color.Coral : Color.Empty;
                e.Cell.Appearance.BackColor = Convert.ToBoolean(e.Cell.Text) ? Color.Coral : Color.Empty;
            }
        }
    }
}