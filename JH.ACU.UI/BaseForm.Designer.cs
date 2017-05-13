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
            this.led2 = new NationalInstruments.UI.WindowsForms.Led();
            this.led3 = new NationalInstruments.UI.WindowsForms.Led();
            this.numericEdit1 = new NationalInstruments.UI.WindowsForms.NumericEdit();
            this.led4 = new NationalInstruments.UI.WindowsForms.Led();
            this.numericEdit2 = new NationalInstruments.UI.WindowsForms.NumericEdit();
            this.numericEdit3 = new NationalInstruments.UI.WindowsForms.NumericEdit();
            ((System.ComponentModel.ISupportInitialize)(this.led1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.led2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.led3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.led4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit3)).BeginInit();
            this.SuspendLayout();
            // 
            // led1
            // 
            this.led1.LedStyle = NationalInstruments.UI.LedStyle.Round3D;
            this.led1.Location = new System.Drawing.Point(219, 343);
            this.led1.Name = "led1";
            this.led1.Size = new System.Drawing.Size(97, 121);
            this.led1.TabIndex = 0;
            // 
            // led2
            // 
            this.led2.Location = new System.Drawing.Point(70, 89);
            this.led2.Name = "led2";
            this.led2.Size = new System.Drawing.Size(64, 99);
            this.led2.TabIndex = 2;
            // 
            // led3
            // 
            this.led3.LedStyle = NationalInstruments.UI.LedStyle.Round3D;
            this.led3.Location = new System.Drawing.Point(135, 234);
            this.led3.Name = "led3";
            this.led3.Size = new System.Drawing.Size(64, 64);
            this.led3.TabIndex = 3;
            // 
            // numericEdit1
            // 
            this.numericEdit1.Location = new System.Drawing.Point(286, 198);
            this.numericEdit1.Name = "numericEdit1";
            this.numericEdit1.Size = new System.Drawing.Size(280, 21);
            this.numericEdit1.TabIndex = 4;
            // 
            // led4
            // 
            this.led4.LedStyle = NationalInstruments.UI.LedStyle.Round3D;
            this.led4.Location = new System.Drawing.Point(254, 233);
            this.led4.Name = "led4";
            this.led4.Size = new System.Drawing.Size(126, 104);
            this.led4.TabIndex = 5;
            // 
            // numericEdit2
            // 
            this.numericEdit2.Location = new System.Drawing.Point(259, 141);
            this.numericEdit2.Name = "numericEdit2";
            this.numericEdit2.Size = new System.Drawing.Size(120, 21);
            this.numericEdit2.TabIndex = 6;
            // 
            // numericEdit3
            // 
            this.numericEdit3.Location = new System.Drawing.Point(415, 141);
            this.numericEdit3.Name = "numericEdit3";
            this.numericEdit3.Size = new System.Drawing.Size(120, 21);
            this.numericEdit3.TabIndex = 7;
            // 
            // BaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1058, 659);
            this.Controls.Add(this.numericEdit3);
            this.Controls.Add(this.numericEdit2);
            this.Controls.Add(this.led4);
            this.Controls.Add(this.numericEdit1);
            this.Controls.Add(this.led3);
            this.Controls.Add(this.led2);
            this.Controls.Add(this.led1);
            this.Name = "BaseForm";
            this.Text = "AbstractForm";
            ((System.ComponentModel.ISupportInitialize)(this.led1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.led2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.led3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.led4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericEdit3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NationalInstruments.UI.WindowsForms.Led led1;
        private NationalInstruments.UI.WindowsForms.Led led2;
        private NationalInstruments.UI.WindowsForms.Led led3;
        private NationalInstruments.UI.WindowsForms.NumericEdit numericEdit1;
        private NationalInstruments.UI.WindowsForms.Led led4;
        private NationalInstruments.UI.WindowsForms.NumericEdit numericEdit2;
        private NationalInstruments.UI.WindowsForms.NumericEdit numericEdit3;






    }
}