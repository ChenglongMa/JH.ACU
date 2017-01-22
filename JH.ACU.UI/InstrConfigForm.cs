using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JH.ACU.Model;

namespace JH.ACU.UI
{
    public partial class InstrConfigForm : Form
    {
        public InstrConfigForm()
        {
            InitializeComponent();

            #region 绑定数据

            //cmbInstrName.DataSource = new BindingSource {DataSource = ConstParameter.InstrNameString};
            //cmbInstrName.DisplayMember = "Key";
            //cmbInstrName.ValueMember = "Value";
            cmbInstrName.SetDataBinding(ConstParameter.InstrNameString,"ACU");
            #endregion

        }
    }
}
