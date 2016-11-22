using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JH.ACU.UI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            //var log = log4net.LogManager.GetLogger("testApp.Logging");//获取一个日志记录器

            //log.Info(DateTime.Now + ": login success");//写入一条新log
            ultraGrid1.Show();
        }

    }
}
