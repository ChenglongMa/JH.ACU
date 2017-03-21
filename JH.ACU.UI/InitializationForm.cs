using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JH.ACU.BLL.Config;
using JH.ACU.Model.Config.TestConfig;

namespace JH.ACU.UI
{
    public partial class InitializationForm : Form
    {
        public InitializationForm()
        {
            InitializeComponent();
        }

        #region 属性字段

        public TestCondition TestCondition = new TestCondition();

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
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var fileName = saveFileDialog1.FileName;
                var testCondition = new TestCondition//TODO:items未保存
                {
                    Temperature =
                    {
                        Duration = (double) numDuration.Value,
                        HighTemp = (double) numHighTemp.Value,
                        LowTemp = (double) numLowTemp.Value,
                        NorTemp = (double) numNorTemp.Value,
                        Enable = ckbChamberEnable.Checked
                    },
                    Voltage =
                    {
                        HighVolt = (double) numHighVolt.Value,
                        NorVolt = (double) numNorVolt.Value,
                        LowVolt = (double) numLowVolt.Value
                    }
                };
                testCondition.SaveToFile(fileName);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var fileName = openFileDialog1.FileName;
                var testCondition = BllConfig.GetTestCondition(fileName);
                numDuration.Value = (decimal) testCondition.Temperature.Duration;
                numHighTemp.Value = (decimal) testCondition.Temperature.HighTemp;
                numLowTemp.Value = (decimal) testCondition.Temperature.LowTemp;
                numNorTemp.Value = (decimal) testCondition.Temperature.NorTemp;
                numHighVolt.Value = (decimal) testCondition.Voltage.HighVolt;
                numNorVolt.Value = (decimal) testCondition.Voltage.NorVolt;
                numLowVolt.Value = (decimal) testCondition.Voltage.LowVolt;
                ckbChamberEnable.Checked = testCondition.Temperature.Enable;
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            //TODO:items未保存
        }
    }
}
