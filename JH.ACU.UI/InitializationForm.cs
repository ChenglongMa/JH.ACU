using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Lib.GridConfig;
using JH.ACU.Model.Config.TestConfig;

namespace JH.ACU.UI
{
    public partial class InitializationForm : Form
    {

        public InitializationForm()
        {
            InitializeComponent();
            _fields = BllFieldConfig.LoadFieldsInfo("SpecInitialForm.xml");
            TestCondition = BllConfig.GetTestCondition(); //从默认路径获取
            BindingSourceTable();
            DisplayTestCondition(TestCondition);
        }

        #region 属性字段

        private readonly List<FieldMetaInfo> _fields;
        public TestCondition TestCondition { get; private set; }
        private readonly List<SpecItem> _specItems = BllConfig.GetSpecItems();

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
            numDuration.Value = (decimal) BLL.Properties.Settings.Default.Duration;
            numHighTemp.Value = (decimal) BLL.Properties.Settings.Default.HighTemp;
            numLowTemp.Value = (decimal) BLL.Properties.Settings.Default.LowTemp;
            numNorTemp.Value = (decimal) BLL.Properties.Settings.Default.NorTemp;
            numHighVolt.Value = (decimal) BLL.Properties.Settings.Default.HighVolt;
            numNorVolt.Value = (decimal) BLL.Properties.Settings.Default.NorVolt;
            numLowVolt.Value = (decimal) BLL.Properties.Settings.Default.LowVolt;
            ckbChamberEnable.Checked = BLL.Properties.Settings.Default.TempEnable;
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
            switch (testCondition.CrashOutType)
            {
                case CrashOutType.Advanced:
                    rdoAdvanced.Checked = true;
                    break;
                case CrashOutType.Conventional:
                    rdoConventional.Checked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            BuildTree(testCondition);
        }

        /// <summary>
        /// 控件值赋给配置
        /// </summary>
        /// <returns></returns>
        private TestCondition BuildTestCondition()
        {
            var temp = new Temperature
            {
                Duration = (double) numDuration.Value, HighTemp = new Temp {Value = (double) numHighTemp.Value, Delay = (double) numHighDelay.Value}, LowTemp = new Temp {Value = (double) numLowTemp.Value, Delay = (double) numLowDelay.Value}, NorTemp = new Temp {Value = (double) numNorTemp.Value, Delay = (double) numNorDelay.Value}, Enable = ckbChamberEnable.Checked
            };
            var volt = new Voltage
            {
                HighVolt = (double) numHighVolt.Value, NorVolt = (double) numNorVolt.Value, LowVolt = (double) numLowVolt.Value
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
            var crashoutType = CrashOutType.Advanced;
            if (rdoAdvanced.Checked)
            {
                crashoutType = CrashOutType.Advanced;
            }
            else if (rdoConventional.Checked)
            {
                crashoutType = CrashOutType.Conventional;
            }
            return new TestCondition
            {
                Temperature = temp, Voltage = volt, TvItems = tvItems, AcuItems = GetAcuItems(), CrashOutType = crashoutType,
            };
        }

        /// <summary>
        /// 将Grid中选中的值转换为AcuItems
        /// </summary>
        /// <returns></returns>
        private List<AcuItems> GetAcuItems()
        {
            var res = new List<AcuItems>();
            var root = treeView1.Nodes["tnAcuRoot"];
            for (int i = 0; i < 8; i++)
            {
                var key = "tnAcu" + i;
                var childNode = root.Nodes.Find(key, false).FirstOrDefault();
                if (childNode == null)
                {
                    continue;
                }
                var list =
                    (from TreeNode node in childNode.Nodes select node.Tag).OfType<SpecItem>()
                        .Select(spec => spec.Index)
                        .ToList();
                res.Add(new AcuItems {Index = i, Items = list});
            }
            return res;
        }

        private void BindingSourceTable()
        {
            var list = new BindingList<SpecItem>(_specItems);
            dgSource.DataSource = list;
            dgSource.SetStyle(_fields);
            dgSource.DisplayLayout.Override.CellClickAction = CellClickAction.RowSelect;
            //for (int i = 1; i < 9; i++)
            //{
            //    dgSource.DisplayLayout.Bands[0].Columns.Add("Acu" + i, "ACU #" + i);
            //    var col = dgSource.DisplayLayout.Bands[0].Columns["Acu" + i];
            //    col.DataType = typeof (bool);
            //    col.Style = ColumnStyle.CheckBox;
            //    col.CellActivation = Activation.AllowEdit;
            //    col.DefaultCellValue = false;
            //}
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
                e.Cell.ActiveAppearance.BackColor = Convert.ToBoolean(e.Cell.Text) ? Color.Coral : DefaultBackColor;
                e.Cell.Appearance.BackColor = Convert.ToBoolean(e.Cell.Text) ? Color.Coral : DefaultBackColor;
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            foreach (TreeNode childTn in node.Nodes)
            {
                childTn.Checked = node.Checked;
            }
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            foreach (var row in dgSource.Rows)
            {
                row.Selected = true;
            }
        }

        private void btnUnselectAll_Click(object sender, EventArgs e)
        {
            foreach (var row in dgSource.Rows)
            {
                row.Selected = false;
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (dgSource.Selected == null || dgSource.Selected.Rows.Count == 0)
            {
                MessageBoxHelper.ShowError("请选择需要测试项");
                return;
            }
            var root = treeView1.Nodes["tnAcuRoot"];
            var specList =
                (from UltraGridRow row in dgSource.Selected.Rows select row.ListObject).OfType<SpecItem>().ToList();
            var hasSelectNode = root.Nodes.Cast<TreeNode>().Any(childNode => childNode.Checked);
            if (!hasSelectNode)
            {
                MessageBoxHelper.ShowError("请选择ACU项");
                return;
            }
            RootNodeAddItems(specList);
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            var root = treeView1.Nodes["tnAcuRoot"];
            foreach (TreeNode childNode in root.Nodes)
            {
                for (var i = childNode.Nodes.Count - 1; i >= 0; i--)
                {
                    if (childNode.Nodes[i].Checked)
                    {
                        childNode.Nodes.Remove(childNode.Nodes[i]);
                    }
                }
            }
        }

        private void BuildTree(TestCondition testCondition)
        {
            var root = treeView1.Nodes["tnAcuRoot"];
            foreach (var acu in testCondition.AcuItems)
            {
                var childNode = root.Nodes.Find("tnAcu" + acu.Index, false).FirstOrDefault();
                if (childNode == null)
                {
                    continue;
                }
                foreach (var item in acu.Items)
                {
                    var spec = _specItems.Find(s => s.Index == item);
                    if (spec == null)
                    {
                        continue;
                    }
                    var node = new TreeNode(spec.Description)
                    {
                        Tag = spec,
                        ImageIndex = 2,
                        Name = spec.Index.ToString()
                    };
                    if (!childNode.Nodes.ContainsKey(node.Name))
                    {
                        childNode.Nodes.Add(node);
                    }
                }
            }
        }

        /// <summary>
        /// 给树控件根节点添加测试项
        /// </summary>
        /// <param name="items"></param>
        private void RootNodeAddItems(List<SpecItem> items)
        {
            var root = treeView1.Nodes["tnAcuRoot"];
            foreach (TreeNode childNode in root.Nodes)
            {
                if (!childNode.Checked) continue;
                foreach (var specItem in items)
                {
                    var node = new TreeNode(specItem.Description)
                    {
                        Tag = specItem,
                        ImageIndex = 2,
                        Name = specItem.Index.ToString()
                    };
                    if (!childNode.Nodes.ContainsKey(node.Name))
                    {
                        childNode.Nodes.Add(node);
                    }
                }
            }
        }
    }
}