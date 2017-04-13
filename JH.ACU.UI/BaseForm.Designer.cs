namespace JH.ACU.UI
{
    partial class BaseForm
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
            this.led1 = new NationalInstruments.UI.WindowsForms.Led();
            this.numericEdit1 = new NationalInstruments.UI.WindowsForms.NumericEdit();
            this.numericEdit2 = new NationalInstruments.UI.WindowsForms.NumericEdit();
            this.led2 = new NationalInstruments.UI.WindowsForms.Led();
            ((System.ComponentModel.ISupportInitialize)(this.led1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.led2)).BeginInit();
            this.SuspendLayout();
            // 
            // led1
            // 
            this.led1.LedStyle = NationalInstruments.UI.LedStyle.Round3D;
            this.led1.Location = new System.Drawing.Point(64, 53);
            this.led1.Name = "led1";
            this.led1.Size = new System.Drawing.Size(64, 64);
            this.led1.TabIndex = 0;
            // 
            // numericEdit1
            // 
            this.numericEdit1.Location = new System.Drawing.Point(97, 141);
            this.numericEdit1.Name = "numericEdit1";
            this.numericEdit1.Size = new System.Drawing.Size(120, 21);
            this.numericEdit1.TabIndex = 1;
            // 
            // numericEdit2
            // 
            this.numericEdit2.Location = new System.Drawing.Point(97, 198);
            this.numericEdit2.Name = "numericEdit2";
            this.numericEdit2.Size = new System.Drawing.Size(120, 21);
            this.numericEdit2.TabIndex = 2;
            // 
            // led2
            // 
            this.led2.LedStyle = NationalInstruments.UI.LedStyle.Round3D;
            this.led2.Location = new System.Drawing.Point(135, 72);
            this.led2.Name = "led2";
            this.led2.Size = new System.Drawing.Size(64, 64);
            this.led2.TabIndex = 3;
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.led2);
            this.Controls.Add(this.numericEdit2);
            this.Controls.Add(this.numericEdit1);
            this.Controls.Add(this.led1);
            this.Name = "BaseForm";
            this.Text = "AbstractForm";
            ((System.ComponentModel.ISupportInitialize)(this.led1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.led2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NationalInstruments.UI.WindowsForms.Led led1;
        private NationalInstruments.UI.WindowsForms.NumericEdit numericEdit1;
        private NationalInstruments.UI.WindowsForms.NumericEdit numericEdit2;
        private NationalInstruments.UI.WindowsForms.Led led2;


    }
}