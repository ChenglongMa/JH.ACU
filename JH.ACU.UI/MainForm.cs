using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Infragistics.UltraChart.Shared.Styles;

namespace JH.ACU.UI
{
    public partial class MainForm : Form
    {
        private object[] List = new object[10];
        private DataTable dt = new DataTable();
        private DataRow row;
        public MainForm()
        {
            InitializeComponent();
            row = dt.NewRow();
            dt.Rows.Add(row);
            ultraChart1.DataSource = dt;
            ultraChart1.DataBind();
            //ultraChart1.Data.SwapRowsAndColumns = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ultraChart1.Data.SwapRowsAndColumns = !ultraChart1.Data.SwapRowsAndColumns;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ultraChart1.Series.DataBind();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            dt.Columns.Clear();
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                backgroundWorker1.ReportProgress(2 * i * i, DateTime.Now.ToString("T"));
                Thread.Sleep(1000);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var name = e.UserState.ToString();//DateTime.Now.ToString("T");
            dt.Columns.Add(name, typeof (int));
            row[name] = e.ProgressPercentage;
        }
    }
}
