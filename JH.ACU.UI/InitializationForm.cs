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

namespace JH.ACU.UI
{
    public partial class InitializationForm : Form
    {

        public InitializationForm()
        {
            InitializeComponent();
            _fields = BllFieldConfig.LoadFieldsInfo("SpecInitialForm.xml");
            TestCondition = BllConfig.GetTestCondition(); //从默认路径获取
            DisplayTestCondition(TestCondition);
            BindingSourceTable();
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

        private void ckbAcuAll_Click(object sender, EventArgs e)
        {
            var count = flowAcu.Controls.Count;
            for (int i = 0; i < count - 1; i++)
            {
                ((CheckBox) flowAcu.Controls[i]).Checked = ckbAcuAll.Checked;
            }

        }

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

        private void ckbAcus_Click(object sender, EventArgs e)
        {
            var isChecked = ((CheckBox) sender).Checked;
            var count = flowAcu.Controls.Count;
            for (int i = 0; i < count - 1; i++)
            {
                if (((CheckBox) flowAcu.Controls[i]).Checked != isChecked)
                {
                    ckbAcuAll.CheckState = CheckState.Indeterminate;
                    return;
                }
            }
            ckbAcuAll.CheckState = CheckState.Checked;
            ckbAcuAll.Checked = isChecked;

        }

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
            numHighTemp.Value = (decimal) testCondition.Temperature.HighTemp;
            numLowTemp.Value = (decimal) testCondition.Temperature.LowTemp;
            numNorTemp.Value = (decimal) testCondition.Temperature.NorTemp;
            numHighVolt.Value = (decimal) testCondition.Voltage.HighVolt;
            numNorVolt.Value = (decimal) testCondition.Voltage.NorVolt;
            numLowVolt.Value = (decimal) testCondition.Voltage.LowVolt;
            ckbChamberEnable.Checked = testCondition.Temperature.Enable;
            //TODO：未完
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
                HighTemp = (double) numHighTemp.Value,
                LowTemp = (double) numLowTemp.Value,
                NorTemp = (double) numNorTemp.Value,
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
                tvItems.Add(TvType.LowTempLowVolt,new[] {temp.LowTemp, volt.LowVolt});
            }
            if (ckbLN.Checked)
            {
                tvItems.Add(TvType.LowTempNorVolt,new[] {temp.LowTemp, volt.NorVolt});
            }
            if (ckbLH.Checked)
            {
                tvItems.Add(TvType.LowTempHighVolt,new[] {temp.LowTemp, volt.HighVolt});
            }
            if (ckbNL.Checked)
            {
                tvItems.Add(TvType.NorTempLowVolt,new[] {temp.NorTemp, volt.LowVolt});
            }
            if (ckbNN.Checked)
            {
                tvItems.Add(TvType.NorTempNorVolt,new[] {temp.NorTemp, volt.NorVolt});
            }
            if (ckbNH.Checked)
            {
                tvItems.Add(TvType.NorTempHighVolt,new[] {temp.NorTemp, volt.HighVolt});
            }
            if (ckbHL.Checked)
            {
                tvItems.Add(TvType.HighTempLowVolt,new[] {temp.HighTemp, volt.LowVolt});
            }
            if (ckbHN.Checked)
            {
                tvItems.Add(TvType.HighTempNorVolt,new[] {temp.HighTemp, volt.NorVolt});
            }
            if (ckbHH.Checked)
            {
                tvItems.Add(TvType.HighTempHighVolt,new[] {temp.HighTemp, volt.HighVolt});
            }
            if (tvItems.IsNullOrEmpty())
            {
                tvItems.Add(TvType.NorTempNorVolt,new[] {temp.NorTemp, volt.NorVolt});
            }
            return new TestCondition
            {
                Temperature = temp,
                Voltage = volt,
                TvItems = tvItems,
                //TODO:ACU Items 未保存
            };
        }

        private void BindingSourceTable()
        {
            var list = new BindingList<SpecItem>(BllConfig.GetSpecItems());
            dgSource.DataSource = list;
            dgSource.SetStyle(_fields);
            dgSource.SetGridDefaultStyle();
            dgSource.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
        }

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="control"></param>
        /// <param name="message"></param>
        private void SetError(Control control,string message)
        {
            errorProvider1.Clear();
            errorProvider1.SetError(control, message);
        }

        private void btnSelectOne_Click(object sender, EventArgs e)
        {
            if (tvTarget.SelectedNode == null||tvTarget.SelectedNode==tvTarget.TopNode)
            {
                SetError(btnSelectOne, "未选择ACU项");
                return;
            }
            if(dgSource.Selected==null)
            {
                SetError(btnSelectOne, "未选择测试项");
                return;
            }
            foreach (var row in dgSource.Selected.Rows)
            {
                var spec = (SpecItem)row.ListObject;
                if (!tvTarget.SelectedNode.Nodes.ContainsKey(spec.Index.ToString()))
                {
                    tvTarget.SelectedNode.Nodes.Add(new TreeNode(spec.Index.ToString(), 2, 3)
                    {
                        Name = spec.Index.ToString(),
                        Tag = spec,
                    });
                }
            }        }
    }
}