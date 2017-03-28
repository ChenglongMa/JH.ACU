using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinToolbars;
using JH.ACU.BLL;
using JH.ACU.Model;
using JH.ACU.Model.Config.TestConfig;

namespace JH.ACU.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            ugDisplay.DataSource = list;
            
        }

        #region 属性字段

        BindingList<SpecUnit> list = new BindingList<SpecUnit>();
        private InitializationForm _conditionForm;

        private TestCondition TestCondition
        {
            get { return _conditionForm == null ? null : _conditionForm.TestCondition; }
        }

        #endregion

        #region 私有方法

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
                    _conditionForm = new InitializationForm();
                    _conditionForm.ShowDialog(this);
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

        #endregion

        private readonly BllMain _bllMain=new BllMain();
        private void button1_Click_1(object sender, EventArgs e)
        {
            _bllMain.TestCondition = new TestCondition
            {
                Temperature = new Temperature {Enable = false},
                Voltage = new Voltage {HighVolt = 13.5, LowVolt = 6.5, NorVolt = 12},
                AcuItems = new List<AcuItems>
                {
                    new AcuItems
                    {
                        Index = 5,
                        Items = new List<int>
                        {
                            1,
                            2,
                            17,
                            18,
                            33,
                            49
                        }
                    }
                },
                TvItems = new List<double[]> {new[] {25D, 12D}}
            };
            _bllMain.AutoRun();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var s = ushort.Parse(textBox1.Text);

            textBox2.Text=_bllMain.Fun(s).ToString();
            textBox3.Text = _bllMain.Fun1(s).ToString();
            foreach (var d in _bllMain.Fun2(s))
            {
                textBox4.Text += d+",";

            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        #region 公有方法


        #endregion

    }
}
