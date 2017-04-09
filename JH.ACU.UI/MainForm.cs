﻿using System;
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

namespace JH.ACU.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
        }

        #region 属性字段

        private bool _isAuto;
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

        #endregion

        private void ledAutoRun_StateChanged(object sender, NationalInstruments.UI.ActionEventArgs e)
        {
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


        #region 公有方法


        #endregion

    }
}
