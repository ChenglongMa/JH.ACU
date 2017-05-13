using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using JH.ACU.BLL.Config;
using JH.ACU.Lib;
using JH.ACU.Model.Config.TestConfig;

namespace JH.ACU.UI
{
    public partial class SpecConfigForm : Form
    {
        public SpecConfigForm()
        {
            InitializeComponent();
            _specUnit = new BindingList<SpecItem>(BllConfig.GetSpecItems());
            BindingToGrid();
        }

        #region 属性字段

        private BindingList<SpecItem> _specUnit;

        #endregion

        #region 私有方法

        private void BindingToGrid()
        {
            ultraGrid1.DataSource = _specUnit;
            var columns = ultraGrid1.DisplayLayout.Bands[0].Columns;
            //if (columns.Exists("ResultValue"))
            //{
            //    columns.Remove("ResultValue");
            //}
            //if (columns.Exists("Dtc"))
            //{
            //    columns.Remove("Dtc");
            //}
            //if (columns.Exists("DtcString"))
            //{
            //    columns["DtcString"].Key = "DTC";
            //}
            UltraGridHelper.SetGridDefaultStyle(ultraGrid1);
        }

        #endregion

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var fileName = openFileDialog1.FileName;
                _specUnit=new BindingList<SpecItem>(BllConfig.GetSpecItems(fileName));
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var fileName = saveFileDialog1.FileName;
                _specUnit.ToList().SaveToFile(fileName); //保存到指定路径一份
            }
            _specUnit.ToList().SaveToFile(); //保存到默认路径一份
        }

    }
}
