namespace JH.ACU.UI
{
    partial class InstrConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblName = new System.Windows.Forms.Label();
            this.lblType = new System.Windows.Forms.Label();
            this.lblPortNum = new System.Windows.Forms.Label();
            this.lblBaudRate = new System.Windows.Forms.Label();
            this.lblParity = new System.Windows.Forms.Label();
            this.lblDataBits = new System.Windows.Forms.Label();
            this.cmbInstrName = new System.Windows.Forms.ComboBox();
            this.cmbInstrType = new System.Windows.Forms.ComboBox();
            this.cmbDataBits = new System.Windows.Forms.ComboBox();
            this.cmbParity = new System.Windows.Forms.ComboBox();
            this.cmbBaudRate = new System.Windows.Forms.ComboBox();
            this.cmbSerialPort = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblStopBits = new System.Windows.Forms.Label();
            this.cmbStopBits = new System.Windows.Forms.ComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panTcp = new System.Windows.Forms.Panel();
            this.lTcpTimeout = new System.Windows.Forms.Label();
            this.tTcpIp = new System.Windows.Forms.TextBox();
            this.lblIp = new System.Windows.Forms.Label();
            this.nTcpTimeout = new System.Windows.Forms.NumericUpDown();
            this.nTcpPort = new System.Windows.Forms.NumericUpDown();
            this.lTcpPort = new System.Windows.Forms.Label();
            this.panGpib = new System.Windows.Forms.Panel();
            this.cmbGpibAddress = new System.Windows.Forms.ComboBox();
            this.lblGpibAddress = new System.Windows.Forms.Label();
            this.panSerial = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panTcp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nTcpTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nTcpPort)).BeginInit();
            this.panGpib.SuspendLayout();
            this.panSerial.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(64, 53);
            this.lblName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(67, 15);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "仪器名称";
            // 
            // lblType
            // 
            this.lblType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(64, 88);
            this.lblType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(67, 15);
            this.lblType.TabIndex = 1;
            this.lblType.Text = "接口类型";
            // 
            // lblPortNum
            // 
            this.lblPortNum.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPortNum.AutoSize = true;
            this.lblPortNum.Location = new System.Drawing.Point(72, -19);
            this.lblPortNum.Margin = new System.Windows.Forms.Padding(0);
            this.lblPortNum.Name = "lblPortNum";
            this.lblPortNum.Size = new System.Drawing.Size(52, 15);
            this.lblPortNum.TabIndex = 1;
            this.lblPortNum.Text = "端口号";
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblBaudRate.AutoSize = true;
            this.lblBaudRate.Location = new System.Drawing.Point(72, 31);
            this.lblBaudRate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(52, 15);
            this.lblBaudRate.TabIndex = 1;
            this.lblBaudRate.Text = "波特率";
            // 
            // lblParity
            // 
            this.lblParity.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblParity.AutoSize = true;
            this.lblParity.Location = new System.Drawing.Point(64, 81);
            this.lblParity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblParity.Name = "lblParity";
            this.lblParity.Size = new System.Drawing.Size(67, 15);
            this.lblParity.TabIndex = 1;
            this.lblParity.Text = "奇偶校验";
            // 
            // lblDataBits
            // 
            this.lblDataBits.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblDataBits.AutoSize = true;
            this.lblDataBits.Location = new System.Drawing.Point(72, 131);
            this.lblDataBits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDataBits.Name = "lblDataBits";
            this.lblDataBits.Size = new System.Drawing.Size(52, 15);
            this.lblDataBits.TabIndex = 1;
            this.lblDataBits.Text = "数据位";
            // 
            // cmbInstrName
            // 
            this.cmbInstrName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbInstrName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstrName.Enabled = false;
            this.cmbInstrName.FormattingEnabled = true;
            this.cmbInstrName.Location = new System.Drawing.Point(146, 49);
            this.cmbInstrName.Margin = new System.Windows.Forms.Padding(4);
            this.cmbInstrName.Name = "cmbInstrName";
            this.cmbInstrName.Size = new System.Drawing.Size(160, 23);
            this.cmbInstrName.TabIndex = 4;
            this.cmbInstrName.SelectedIndexChanged += new System.EventHandler(this.cmbInstrName_SelectedIndexChanged);
            // 
            // cmbInstrType
            // 
            this.cmbInstrType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbInstrType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstrType.FormattingEnabled = true;
            this.cmbInstrType.Location = new System.Drawing.Point(146, 84);
            this.cmbInstrType.Margin = new System.Windows.Forms.Padding(4);
            this.cmbInstrType.Name = "cmbInstrType";
            this.cmbInstrType.Size = new System.Drawing.Size(160, 23);
            this.cmbInstrType.TabIndex = 4;
            this.cmbInstrType.SelectedIndexChanged += new System.EventHandler(this.cmbInstrType_SelectedIndexChanged);
            // 
            // cmbDataBits
            // 
            this.cmbDataBits.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataBits.FormattingEnabled = true;
            this.cmbDataBits.Location = new System.Drawing.Point(146, 127);
            this.cmbDataBits.Margin = new System.Windows.Forms.Padding(4);
            this.cmbDataBits.Name = "cmbDataBits";
            this.cmbDataBits.Size = new System.Drawing.Size(160, 23);
            this.cmbDataBits.TabIndex = 5;
            this.cmbDataBits.SelectedIndexChanged += new System.EventHandler(this.cmbDataBits_SelectedIndexChanged);
            // 
            // cmbParity
            // 
            this.cmbParity.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Location = new System.Drawing.Point(146, 77);
            this.cmbParity.Margin = new System.Windows.Forms.Padding(4);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(160, 23);
            this.cmbParity.TabIndex = 6;
            this.cmbParity.SelectedIndexChanged += new System.EventHandler(this.cmbParity_SelectedIndexChanged);
            // 
            // cmbBaudRate
            // 
            this.cmbBaudRate.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbBaudRate.FormattingEnabled = true;
            this.cmbBaudRate.Location = new System.Drawing.Point(146, 27);
            this.cmbBaudRate.Margin = new System.Windows.Forms.Padding(4);
            this.cmbBaudRate.Name = "cmbBaudRate";
            this.cmbBaudRate.Size = new System.Drawing.Size(160, 23);
            this.cmbBaudRate.TabIndex = 7;
            this.cmbBaudRate.SelectedIndexChanged += new System.EventHandler(this.cmbBaudRate_SelectedIndexChanged);
            this.cmbBaudRate.Leave += new System.EventHandler(this.cmbBaudRate_Leave);
            // 
            // cmbSerialPort
            // 
            this.cmbSerialPort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbSerialPort.FormattingEnabled = true;
            this.cmbSerialPort.Location = new System.Drawing.Point(146, -23);
            this.cmbSerialPort.Margin = new System.Windows.Forms.Padding(0);
            this.cmbSerialPort.Name = "cmbSerialPort";
            this.cmbSerialPort.Size = new System.Drawing.Size(160, 23);
            this.cmbSerialPort.TabIndex = 8;
            this.cmbSerialPort.SelectedIndexChanged += new System.EventHandler(this.cmbSerialPort_SelectedIndexChanged);
            this.cmbSerialPort.Leave += new System.EventHandler(this.cmbSerialPort_Leave);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(127, 23);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 29);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblStopBits
            // 
            this.lblStopBits.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblStopBits.AutoSize = true;
            this.lblStopBits.Location = new System.Drawing.Point(72, 182);
            this.lblStopBits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStopBits.Name = "lblStopBits";
            this.lblStopBits.Size = new System.Drawing.Size(52, 15);
            this.lblStopBits.TabIndex = 1;
            this.lblStopBits.Text = "停止位";
            // 
            // cmbStopBits
            // 
            this.cmbStopBits.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStopBits.FormattingEnabled = true;
            this.cmbStopBits.Location = new System.Drawing.Point(146, 178);
            this.cmbStopBits.Margin = new System.Windows.Forms.Padding(4);
            this.cmbStopBits.Name = "cmbStopBits";
            this.cmbStopBits.Size = new System.Drawing.Size(160, 23);
            this.cmbStopBits.TabIndex = 5;
            this.cmbStopBits.SelectedIndexChanged += new System.EventHandler(this.cmbStopBits_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.cmbInstrName);
            this.splitContainer1.Panel1.Controls.Add(this.lblName);
            this.splitContainer1.Panel1.Controls.Add(this.lblType);
            this.splitContainer1.Panel1.Controls.Add(this.cmbInstrType);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(364, 461);
            this.splitContainer1.SplitterDistance = 126;
            this.splitContainer1.TabIndex = 10;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panTcp);
            this.splitContainer2.Panel1.Controls.Add(this.panGpib);
            this.splitContainer2.Panel1.Controls.Add(this.panSerial);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel2.Controls.Add(this.btnSave);
            this.splitContainer2.Size = new System.Drawing.Size(364, 331);
            this.splitContainer2.SplitterDistance = 200;
            this.splitContainer2.TabIndex = 0;
            // 
            // panTcp
            // 
            this.panTcp.AutoSize = true;
            this.panTcp.Controls.Add(this.lTcpTimeout);
            this.panTcp.Controls.Add(this.tTcpIp);
            this.panTcp.Controls.Add(this.lblIp);
            this.panTcp.Controls.Add(this.nTcpTimeout);
            this.panTcp.Controls.Add(this.nTcpPort);
            this.panTcp.Controls.Add(this.lTcpPort);
            this.panTcp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panTcp.Location = new System.Drawing.Point(0, 0);
            this.panTcp.Name = "panTcp";
            this.panTcp.Size = new System.Drawing.Size(364, 200);
            this.panTcp.TabIndex = 11;
            this.panTcp.Visible = false;
            this.panTcp.VisibleChanged += new System.EventHandler(this.panTcp_VisibleChanged);
            // 
            // lTcpTimeout
            // 
            this.lTcpTimeout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lTcpTimeout.AutoSize = true;
            this.lTcpTimeout.Location = new System.Drawing.Point(59, 106);
            this.lTcpTimeout.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lTcpTimeout.Name = "lTcpTimeout";
            this.lTcpTimeout.Size = new System.Drawing.Size(67, 15);
            this.lTcpTimeout.TabIndex = 1;
            this.lTcpTimeout.Text = "超时时间";
            // 
            // tTcpIp
            // 
            this.tTcpIp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tTcpIp.Location = new System.Drawing.Point(146, 1);
            this.tTcpIp.Name = "tTcpIp";
            this.tTcpIp.Size = new System.Drawing.Size(160, 25);
            this.tTcpIp.TabIndex = 9;
            this.tTcpIp.Leave += new System.EventHandler(this.tTcpIp_Leave);
            // 
            // lblIp
            // 
            this.lblIp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblIp.AutoSize = true;
            this.lblIp.Location = new System.Drawing.Point(66, 6);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(53, 15);
            this.lblIp.TabIndex = 0;
            this.lblIp.Text = "IP地址";
            // 
            // nTcpTimeout
            // 
            this.nTcpTimeout.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nTcpTimeout.Location = new System.Drawing.Point(146, 101);
            this.nTcpTimeout.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nTcpTimeout.Name = "nTcpTimeout";
            this.nTcpTimeout.Size = new System.Drawing.Size(160, 25);
            this.nTcpTimeout.TabIndex = 10;
            this.nTcpTimeout.Value = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            // 
            // nTcpPort
            // 
            this.nTcpPort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.nTcpPort.Location = new System.Drawing.Point(146, 51);
            this.nTcpPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.nTcpPort.Name = "nTcpPort";
            this.nTcpPort.Size = new System.Drawing.Size(160, 25);
            this.nTcpPort.TabIndex = 10;
            this.nTcpPort.Value = new decimal(new int[] {
            502,
            0,
            0,
            0});
            // 
            // lTcpPort
            // 
            this.lTcpPort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lTcpPort.AutoSize = true;
            this.lTcpPort.Location = new System.Drawing.Point(67, 56);
            this.lTcpPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lTcpPort.Name = "lTcpPort";
            this.lTcpPort.Size = new System.Drawing.Size(52, 15);
            this.lTcpPort.TabIndex = 1;
            this.lTcpPort.Text = "端口号";
            // 
            // panGpib
            // 
            this.panGpib.Controls.Add(this.cmbGpibAddress);
            this.panGpib.Controls.Add(this.lblGpibAddress);
            this.panGpib.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panGpib.Location = new System.Drawing.Point(0, 0);
            this.panGpib.Name = "panGpib";
            this.panGpib.Size = new System.Drawing.Size(364, 200);
            this.panGpib.TabIndex = 11;
            this.panGpib.Visible = false;
            this.panGpib.VisibleChanged += new System.EventHandler(this.panGpib_VisibleChanged);
            // 
            // cmbGpibAddress
            // 
            this.cmbGpibAddress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbGpibAddress.FormattingEnabled = true;
            this.cmbGpibAddress.Location = new System.Drawing.Point(146, 5);
            this.cmbGpibAddress.Margin = new System.Windows.Forms.Padding(4);
            this.cmbGpibAddress.Name = "cmbGpibAddress";
            this.cmbGpibAddress.Size = new System.Drawing.Size(160, 23);
            this.cmbGpibAddress.TabIndex = 8;
            this.cmbGpibAddress.Leave += new System.EventHandler(this.cmbGpibAddress_Leave);
            // 
            // lblGpibAddress
            // 
            this.lblGpibAddress.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblGpibAddress.AutoSize = true;
            this.lblGpibAddress.Location = new System.Drawing.Point(62, 8);
            this.lblGpibAddress.Name = "lblGpibAddress";
            this.lblGpibAddress.Size = new System.Drawing.Size(69, 15);
            this.lblGpibAddress.TabIndex = 0;
            this.lblGpibAddress.Text = "GPIB地址";
            // 
            // panSerial
            // 
            this.panSerial.AutoSize = true;
            this.panSerial.BackColor = System.Drawing.SystemColors.Control;
            this.panSerial.Controls.Add(this.cmbSerialPort);
            this.panSerial.Controls.Add(this.lblPortNum);
            this.panSerial.Controls.Add(this.cmbParity);
            this.panSerial.Controls.Add(this.lblBaudRate);
            this.panSerial.Controls.Add(this.cmbStopBits);
            this.panSerial.Controls.Add(this.lblParity);
            this.panSerial.Controls.Add(this.lblStopBits);
            this.panSerial.Controls.Add(this.cmbDataBits);
            this.panSerial.Controls.Add(this.cmbBaudRate);
            this.panSerial.Controls.Add(this.lblDataBits);
            this.panSerial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panSerial.Location = new System.Drawing.Point(0, 0);
            this.panSerial.Name = "panSerial";
            this.panSerial.Size = new System.Drawing.Size(364, 200);
            this.panSerial.TabIndex = 0;
            this.panSerial.Visible = false;
            this.panSerial.VisibleChanged += new System.EventHandler(this.panSerial_VisibleChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // InstrConfigForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 461);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "InstrConfigForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "仪器接口配置";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panTcp.ResumeLayout(false);
            this.panTcp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nTcpTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nTcpPort)).EndInit();
            this.panGpib.ResumeLayout(false);
            this.panGpib.PerformLayout();
            this.panSerial.ResumeLayout(false);
            this.panSerial.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.Label lblPortNum;
        private System.Windows.Forms.Label lblBaudRate;
        private System.Windows.Forms.Label lblParity;
        private System.Windows.Forms.Label lblDataBits;
        private System.Windows.Forms.ComboBox cmbInstrName;
        private System.Windows.Forms.ComboBox cmbInstrType;
        private System.Windows.Forms.ComboBox cmbDataBits;
        private System.Windows.Forms.ComboBox cmbParity;
        private System.Windows.Forms.ComboBox cmbBaudRate;
        private System.Windows.Forms.ComboBox cmbSerialPort;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblStopBits;
        private System.Windows.Forms.ComboBox cmbStopBits;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panTcp;
        private System.Windows.Forms.Label lTcpTimeout;
        private System.Windows.Forms.NumericUpDown nTcpTimeout;
        private System.Windows.Forms.Label lTcpPort;
        private System.Windows.Forms.TextBox tTcpIp;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.NumericUpDown nTcpPort;
        private System.Windows.Forms.Panel panGpib;
        private System.Windows.Forms.ComboBox cmbGpibAddress;
        private System.Windows.Forms.Label lblGpibAddress;
        private System.Windows.Forms.Panel panSerial;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ErrorProvider errorProvider1;


    }
}