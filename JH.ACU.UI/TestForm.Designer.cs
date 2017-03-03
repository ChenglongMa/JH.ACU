﻿namespace JH.ACU.UI
{
    partial class TestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.numBoard = new System.Windows.Forms.NumericUpDown();
            this.btnInitialize = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnEnable = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numSubRelay = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numMainRelay = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDisable = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn12 = new System.Windows.Forms.Button();
            this.btn13 = new System.Windows.Forms.Button();
            this.btn14 = new System.Windows.Forms.Button();
            this.btn15 = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numBoard)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSubRelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMainRelay)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(35, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 21);
            this.textBox1.TabIndex = 2;
            // 
            // numBoard
            // 
            this.numBoard.Location = new System.Drawing.Point(35, 69);
            this.numBoard.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numBoard.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numBoard.Name = "numBoard";
            this.numBoard.Size = new System.Drawing.Size(120, 21);
            this.numBoard.TabIndex = 3;
            // 
            // btnInitialize
            // 
            this.btnInitialize.Location = new System.Drawing.Point(35, 96);
            this.btnInitialize.Name = "btnInitialize";
            this.btnInitialize.Size = new System.Drawing.Size(75, 23);
            this.btnInitialize.TabIndex = 4;
            this.btnInitialize.Text = "初始化";
            this.btnInitialize.UseVisualStyleBackColor = true;
            this.btnInitialize.Click += new System.EventHandler(this.btnInitialize_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(35, 129);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 5;
            this.btnOpen.Text = "打开子板";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnEnable
            // 
            this.btnEnable.Location = new System.Drawing.Point(314, 96);
            this.btnEnable.Name = "btnEnable";
            this.btnEnable.Size = new System.Drawing.Size(75, 23);
            this.btnEnable.TabIndex = 6;
            this.btnEnable.Text = "使能";
            this.btnEnable.UseVisualStyleBackColor = true;
            this.btnEnable.Click += new System.EventHandler(this.btnEnable_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(37, 158);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "关闭子板";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "卡号（0-7）";
            // 
            // numSubRelay
            // 
            this.numSubRelay.Location = new System.Drawing.Point(314, 69);
            this.numSubRelay.Maximum = new decimal(new int[] {
            350,
            0,
            0,
            0});
            this.numSubRelay.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.numSubRelay.Name = "numSubRelay";
            this.numSubRelay.Size = new System.Drawing.Size(120, 21);
            this.numSubRelay.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(314, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "子板继电器";
            // 
            // numMainRelay
            // 
            this.numMainRelay.Location = new System.Drawing.Point(174, 69);
            this.numMainRelay.Maximum = new decimal(new int[] {
            400,
            0,
            0,
            0});
            this.numMainRelay.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numMainRelay.Name = "numMainRelay";
            this.numMainRelay.Size = new System.Drawing.Size(120, 21);
            this.numMainRelay.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(172, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "主板继电器";
            // 
            // btnDisable
            // 
            this.btnDisable.Location = new System.Drawing.Point(314, 125);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(75, 23);
            this.btnDisable.TabIndex = 12;
            this.btnDisable.Text = "取消使能";
            this.btnDisable.UseVisualStyleBackColor = true;
            this.btnDisable.Click += new System.EventHandler(this.btnDisable_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(174, 96);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 34);
            this.button1.TabIndex = 13;
            this.button1.Text = "主继电器闭合";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(174, 136);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(87, 34);
            this.button2.TabIndex = 13;
            this.button2.Text = "主继电器断开";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(33, 212);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(545, 257);
            this.label4.TabIndex = 8;
            this.label4.Text = "测试用例1：\r\n1、单击初始化；\r\n\r\n2、循环打开-1~10子板；（只有0~7有效）\r\n\r\n3、循环打开子板0上的-100、200、208、264、265、27" +
    "3、277、341继电器；（继电器边界测试）\r\n\r\n4、同时打开200、215、256；（测试不同组继电器及同时打开多个继电器及测试delay时间是否合适）\r\n" +
    "\r\n5、同时打开两个子板，如果可以正常开启则进行【测试用例2】；";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(33, 469);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(545, 228);
            this.label5.TabIndex = 8;
            this.label5.Text = resources.GetString("label5.Text");
            // 
            // btn12
            // 
            this.btn12.Location = new System.Drawing.Point(255, 241);
            this.btn12.Name = "btn12";
            this.btn12.Size = new System.Drawing.Size(75, 23);
            this.btn12.TabIndex = 14;
            this.btn12.Text = "测试1.2";
            this.btn12.UseVisualStyleBackColor = true;
            this.btn12.Click += new System.EventHandler(this.btn12_Click);
            // 
            // btn13
            // 
            this.btn13.Location = new System.Drawing.Point(548, 267);
            this.btn13.Name = "btn13";
            this.btn13.Size = new System.Drawing.Size(75, 23);
            this.btn13.TabIndex = 15;
            this.btn13.Text = "测试1.3";
            this.btn13.UseVisualStyleBackColor = true;
            this.btn13.Click += new System.EventHandler(this.btn13_Click);
            // 
            // btn14
            // 
            this.btn14.Location = new System.Drawing.Point(564, 290);
            this.btn14.Name = "btn14";
            this.btn14.Size = new System.Drawing.Size(75, 23);
            this.btn14.TabIndex = 16;
            this.btn14.Text = "测试1.4";
            this.btn14.UseVisualStyleBackColor = true;
            this.btn14.Click += new System.EventHandler(this.btn14_Click);
            // 
            // btn15
            // 
            this.btn15.Location = new System.Drawing.Point(380, 315);
            this.btn15.Name = "btn15";
            this.btn15.Size = new System.Drawing.Size(75, 23);
            this.btn15.TabIndex = 17;
            this.btn15.Text = "测试1.5";
            this.btn15.UseVisualStyleBackColor = true;
            this.btn15.Click += new System.EventHandler(this.btn15_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(453, 69);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 18;
            this.btnReset.Text = "复位";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 706);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btn15);
            this.Controls.Add(this.btn14);
            this.Controls.Add(this.btn13);
            this.Controls.Add(this.btn12);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnDisable);
            this.Controls.Add(this.numMainRelay);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numSubRelay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnEnable);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnInitialize);
            this.Controls.Add(this.numBoard);
            this.Controls.Add(this.textBox1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "TestForm";
            this.Text = "DAQ Test";
            ((System.ComponentModel.ISupportInitialize)(this.numBoard)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSubRelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMainRelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.NumericUpDown numBoard;
        private System.Windows.Forms.Button btnInitialize;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numSubRelay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numMainRelay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDisable;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn12;
        private System.Windows.Forms.Button btn13;
        private System.Windows.Forms.Button btn14;
        private System.Windows.Forms.Button btn15;
        private System.Windows.Forms.Button btnReset;
    }
}