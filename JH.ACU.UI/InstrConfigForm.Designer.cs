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
            this.cmbPortNum = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblStopBits = new System.Windows.Forms.Label();
            this.cmbStopBits = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(50, 54);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(53, 12);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "仪器名称";
            // 
            // lblType
            // 
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(40, 93);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(53, 12);
            this.lblType.TabIndex = 1;
            this.lblType.Text = "接口类型";
            // 
            // lblPortNum
            // 
            this.lblPortNum.AutoSize = true;
            this.lblPortNum.Location = new System.Drawing.Point(52, 134);
            this.lblPortNum.Name = "lblPortNum";
            this.lblPortNum.Size = new System.Drawing.Size(41, 12);
            this.lblPortNum.TabIndex = 1;
            this.lblPortNum.Text = "端口号";
            // 
            // lblBaudRate
            // 
            this.lblBaudRate.AutoSize = true;
            this.lblBaudRate.Location = new System.Drawing.Point(50, 183);
            this.lblBaudRate.Name = "lblBaudRate";
            this.lblBaudRate.Size = new System.Drawing.Size(41, 12);
            this.lblBaudRate.TabIndex = 1;
            this.lblBaudRate.Text = "波特率";
            // 
            // lblParity
            // 
            this.lblParity.AutoSize = true;
            this.lblParity.Location = new System.Drawing.Point(40, 233);
            this.lblParity.Name = "lblParity";
            this.lblParity.Size = new System.Drawing.Size(53, 12);
            this.lblParity.TabIndex = 1;
            this.lblParity.Text = "奇偶校验";
            // 
            // lblDataBits
            // 
            this.lblDataBits.AutoSize = true;
            this.lblDataBits.Location = new System.Drawing.Point(40, 280);
            this.lblDataBits.Name = "lblDataBits";
            this.lblDataBits.Size = new System.Drawing.Size(41, 12);
            this.lblDataBits.TabIndex = 1;
            this.lblDataBits.Text = "数据位";
            // 
            // cmbInstrName
            // 
            this.cmbInstrName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstrName.FormattingEnabled = true;
            this.cmbInstrName.Location = new System.Drawing.Point(109, 51);
            this.cmbInstrName.Name = "cmbInstrName";
            this.cmbInstrName.Size = new System.Drawing.Size(121, 20);
            this.cmbInstrName.TabIndex = 4;
            this.cmbInstrName.SelectedIndexChanged += new System.EventHandler(this.cmbInstrName_SelectedIndexChanged);
            // 
            // cmbInstrType
            // 
            this.cmbInstrType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstrType.FormattingEnabled = true;
            this.cmbInstrType.Location = new System.Drawing.Point(109, 85);
            this.cmbInstrType.Name = "cmbInstrType";
            this.cmbInstrType.Size = new System.Drawing.Size(121, 20);
            this.cmbInstrType.TabIndex = 4;
            this.cmbInstrType.SelectedIndexChanged += new System.EventHandler(this.cmbInstrType_SelectedIndexChanged);
            // 
            // cmbDataBits
            // 
            this.cmbDataBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataBits.FormattingEnabled = true;
            this.cmbDataBits.Location = new System.Drawing.Point(109, 277);
            this.cmbDataBits.Name = "cmbDataBits";
            this.cmbDataBits.Size = new System.Drawing.Size(121, 20);
            this.cmbDataBits.TabIndex = 5;
            this.cmbDataBits.SelectedIndexChanged += new System.EventHandler(this.cmbDataBits_SelectedIndexChanged);
            // 
            // cmbParity
            // 
            this.cmbParity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbParity.FormattingEnabled = true;
            this.cmbParity.Location = new System.Drawing.Point(109, 230);
            this.cmbParity.Name = "cmbParity";
            this.cmbParity.Size = new System.Drawing.Size(121, 20);
            this.cmbParity.TabIndex = 6;
            this.cmbParity.SelectedIndexChanged += new System.EventHandler(this.cmbParity_SelectedIndexChanged);
            // 
            // cmbBaudRate
            // 
            this.cmbBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBaudRate.FormattingEnabled = true;
            this.cmbBaudRate.Location = new System.Drawing.Point(109, 180);
            this.cmbBaudRate.Name = "cmbBaudRate";
            this.cmbBaudRate.Size = new System.Drawing.Size(121, 20);
            this.cmbBaudRate.TabIndex = 7;
            this.cmbBaudRate.SelectedIndexChanged += new System.EventHandler(this.cmbBaudRate_SelectedIndexChanged);
            // 
            // cmbPortNum
            // 
            this.cmbPortNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPortNum.FormattingEnabled = true;
            this.cmbPortNum.Location = new System.Drawing.Point(109, 126);
            this.cmbPortNum.Name = "cmbPortNum";
            this.cmbPortNum.Size = new System.Drawing.Size(121, 20);
            this.cmbPortNum.TabIndex = 8;
            this.cmbPortNum.SelectedIndexChanged += new System.EventHandler(this.cmbPortNum_SelectedIndexChanged);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(109, 333);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblStopBits
            // 
            this.lblStopBits.AutoSize = true;
            this.lblStopBits.Location = new System.Drawing.Point(40, 310);
            this.lblStopBits.Name = "lblStopBits";
            this.lblStopBits.Size = new System.Drawing.Size(41, 12);
            this.lblStopBits.TabIndex = 1;
            this.lblStopBits.Text = "停止位";
            // 
            // cmbStopBits
            // 
            this.cmbStopBits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStopBits.FormattingEnabled = true;
            this.cmbStopBits.Location = new System.Drawing.Point(109, 307);
            this.cmbStopBits.Name = "cmbStopBits";
            this.cmbStopBits.Size = new System.Drawing.Size(121, 20);
            this.cmbStopBits.TabIndex = 5;
            this.cmbStopBits.SelectedIndexChanged += new System.EventHandler(this.cmbStopBits_SelectedIndexChanged);
            // 
            // InstrConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 469);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cmbPortNum);
            this.Controls.Add(this.cmbBaudRate);
            this.Controls.Add(this.cmbParity);
            this.Controls.Add(this.cmbStopBits);
            this.Controls.Add(this.cmbDataBits);
            this.Controls.Add(this.cmbInstrType);
            this.Controls.Add(this.lblStopBits);
            this.Controls.Add(this.cmbInstrName);
            this.Controls.Add(this.lblDataBits);
            this.Controls.Add(this.lblParity);
            this.Controls.Add(this.lblBaudRate);
            this.Controls.Add(this.lblPortNum);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.lblName);
            this.Name = "InstrConfigForm";
            this.Text = "仪器接口配置";
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.ComboBox cmbPortNum;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblStopBits;
        private System.Windows.Forms.ComboBox cmbStopBits;


    }
}