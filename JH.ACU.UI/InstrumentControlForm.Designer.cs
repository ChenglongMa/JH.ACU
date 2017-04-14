namespace JH.ACU.UI
{
    partial class InstrumentControlForm
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
            if (_daq != null)
            {
                _daq.Dispose();
            }
            if (_pwr != null)
            {
                _pwr.Dispose();
            }
            if (_prs0 != null)
            {
                _prs0.Dispose();
            }
            if (_dmm != null)
            {
                _dmm.Dispose();
            }
            if (_chamber != null)
            {
                _chamber.Dispose();
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.leds20 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowSubRelays = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel15 = new System.Windows.Forms.FlowLayoutPanel();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.leds21 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.leds22 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.leds23 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.leds24 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
            this.label6 = new System.Windows.Forms.Label();
            this.leds25 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel8 = new System.Windows.Forms.FlowLayoutPanel();
            this.label7 = new System.Windows.Forms.Label();
            this.leds26 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.leds27 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel10 = new System.Windows.Forms.FlowLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.leds28 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel11 = new System.Windows.Forms.FlowLayoutPanel();
            this.label10 = new System.Windows.Forms.Label();
            this.leds31 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel12 = new System.Windows.Forms.FlowLayoutPanel();
            this.label11 = new System.Windows.Forms.Label();
            this.leds32 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel14 = new System.Windows.Forms.FlowLayoutPanel();
            this.label13 = new System.Windows.Forms.Label();
            this.leds33 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.flowLayoutPanel13 = new System.Windows.Forms.FlowLayoutPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.leds34 = new NationalInstruments.UI.WindowsForms.LedArray();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.flowLayoutPanel16 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.swPwrOutput = new NationalInstruments.UI.WindowsForms.Switch();
            this.swPwrOcp = new NationalInstruments.UI.WindowsForms.Switch();
            this.swPwrOpen = new NationalInstruments.UI.WindowsForms.Switch();
            this.btnOvp = new System.Windows.Forms.Button();
            this.btnSetCurr = new System.Windows.Forms.Button();
            this.btnSetVolt = new System.Windows.Forms.Button();
            this.numSetCurr = new System.Windows.Forms.NumericUpDown();
            this.numOvp = new System.Windows.Forms.NumericUpDown();
            this.numSetVolt = new System.Windows.Forms.NumericUpDown();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pagePrs0 = new System.Windows.Forms.TabPage();
            this.swRes0Open = new NationalInstruments.UI.WindowsForms.Switch();
            this.btnSetRes0 = new System.Windows.Forms.Button();
            this.numSetRes0 = new System.Windows.Forms.NumericUpDown();
            this.pagePrs1 = new System.Windows.Forms.TabPage();
            this.swRes1Open = new NationalInstruments.UI.WindowsForms.Switch();
            this.btnSetRes1 = new System.Windows.Forms.Button();
            this.numSetRes1 = new System.Windows.Forms.NumericUpDown();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.swChamRun = new NationalInstruments.UI.WindowsForms.Switch();
            this.btnSetTemp = new System.Windows.Forms.Button();
            this.btnGetTemp = new System.Windows.Forms.Button();
            this.numSetTemp = new System.Windows.Forms.NumericUpDown();
            this.numGetTemp = new System.Windows.Forms.NumericUpDown();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.swDmmOpen = new NationalInstruments.UI.WindowsForms.Switch();
            this.btnGetFreq = new System.Windows.Forms.Button();
            this.btnGetFRes = new System.Windows.Forms.Button();
            this.numGetRes = new System.Windows.Forms.NumericUpDown();
            this.numGetFreq = new System.Windows.Forms.NumericUpDown();
            this.btnGetCurr = new System.Windows.Forms.Button();
            this.numGetFRes = new System.Windows.Forms.NumericUpDown();
            this.numGetVolt = new System.Windows.Forms.NumericUpDown();
            this.btnGetRes = new System.Windows.Forms.Button();
            this.numGetCurr = new System.Windows.Forms.NumericUpDown();
            this.btnGetVolt = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.cmbInstrName = new System.Windows.Forms.ComboBox();
            this.btnWrite = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel18 = new System.Windows.Forms.FlowLayoutPanel();
            this.label23 = new System.Windows.Forms.Label();
            this.numRelayIndex = new System.Windows.Forms.NumericUpDown();
            this.btnRelayEnable = new System.Windows.Forms.Button();
            this.btnRelayDisable = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label24 = new System.Windows.Forms.Label();
            this.numBoardIndex = new System.Windows.Forms.NumericUpDown();
            this.btnBoardOpen = new System.Windows.Forms.Button();
            this.btnBoardClose = new System.Windows.Forms.Button();
            this.swDaqOpen = new NationalInstruments.UI.WindowsForms.Switch();
            this.flowLayoutPanel19 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowBoard = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel17 = new System.Windows.Forms.FlowLayoutPanel();
            this.label29 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.ledBoard = new NationalInstruments.UI.WindowsForms.LedArray();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowMainRelays = new System.Windows.Forms.FlowLayoutPanel();
            this.led300 = new NationalInstruments.UI.WindowsForms.Led();
            this.led301 = new NationalInstruments.UI.WindowsForms.Led();
            this.led302 = new NationalInstruments.UI.WindowsForms.Led();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds20.ItemTemplate)).BeginInit();
            this.flowSubRelays.SuspendLayout();
            this.flowLayoutPanel15.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds21.ItemTemplate)).BeginInit();
            this.flowLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds22.ItemTemplate)).BeginInit();
            this.flowLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds23.ItemTemplate)).BeginInit();
            this.flowLayoutPanel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds24.ItemTemplate)).BeginInit();
            this.flowLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds25.ItemTemplate)).BeginInit();
            this.flowLayoutPanel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds26.ItemTemplate)).BeginInit();
            this.flowLayoutPanel9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds27.ItemTemplate)).BeginInit();
            this.flowLayoutPanel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds28.ItemTemplate)).BeginInit();
            this.flowLayoutPanel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds31.ItemTemplate)).BeginInit();
            this.flowLayoutPanel12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds32.ItemTemplate)).BeginInit();
            this.flowLayoutPanel14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds33.ItemTemplate)).BeginInit();
            this.flowLayoutPanel13.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds34.ItemTemplate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.flowLayoutPanel16.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.swPwrOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.swPwrOcp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.swPwrOpen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetCurr)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOvp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetVolt)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.pagePrs0.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.swRes0Open)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetRes0)).BeginInit();
            this.pagePrs1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.swRes1Open)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetRes1)).BeginInit();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.swChamRun)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetTemp)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetTemp)).BeginInit();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.swDmmOpen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetRes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetFreq)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetFRes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetVolt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetCurr)).BeginInit();
            this.groupBox8.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.flowLayoutPanel18.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRelayIndex)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBoardIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.swDaqOpen)).BeginInit();
            this.flowLayoutPanel19.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowBoard.SuspendLayout();
            this.flowLayoutPanel17.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ledBoard.ItemTemplate)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.flowMainRelays.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.led300)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.led301)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.led302)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 547);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(996, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStatus
            // 
            this.toolStatus.Name = "toolStatus";
            this.toolStatus.Size = new System.Drawing.Size(44, 17);
            this.toolStatus.Text = "Ready";
            // 
            // leds20
            // 
            // 
            // 
            // 
            this.leds20.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds20.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds20.ItemTemplate.Name = "";
            this.leds20.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds20.ItemTemplate.TabIndex = 0;
            this.leds20.ItemTemplate.TabStop = false;
            this.leds20.Location = new System.Drawing.Point(2, 19);
            this.leds20.Margin = new System.Windows.Forms.Padding(2, 3, 0, 3);
            this.leds20.Name = "leds20";
            this.leds20.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds20.Size = new System.Drawing.Size(38, 297);
            this.leds20.TabIndex = 2;
            // 
            // flowSubRelays
            // 
            this.flowSubRelays.AutoSize = true;
            this.flowSubRelays.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel15);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel2);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel3);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel4);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel5);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel6);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel7);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel8);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel9);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel10);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel11);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel12);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel14);
            this.flowSubRelays.Controls.Add(this.flowLayoutPanel13);
            this.flowSubRelays.Location = new System.Drawing.Point(6, 101);
            this.flowSubRelays.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.flowSubRelays.Name = "flowSubRelays";
            this.flowSubRelays.Size = new System.Drawing.Size(520, 319);
            this.flowSubRelays.TabIndex = 3;
            // 
            // flowLayoutPanel15
            // 
            this.flowLayoutPanel15.AutoSize = true;
            this.flowLayoutPanel15.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel15.Controls.Add(this.label14);
            this.flowLayoutPanel15.Controls.Add(this.label15);
            this.flowLayoutPanel15.Controls.Add(this.label16);
            this.flowLayoutPanel15.Controls.Add(this.label17);
            this.flowLayoutPanel15.Controls.Add(this.label18);
            this.flowLayoutPanel15.Controls.Add(this.label19);
            this.flowLayoutPanel15.Controls.Add(this.label20);
            this.flowLayoutPanel15.Controls.Add(this.label21);
            this.flowLayoutPanel15.Controls.Add(this.label22);
            this.flowLayoutPanel15.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel15.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel15.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel15.Name = "flowLayoutPanel15";
            this.flowLayoutPanel15.Size = new System.Drawing.Size(22, 304);
            this.flowLayoutPanel15.TabIndex = 4;
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label14.Location = new System.Drawing.Point(11, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(0, 16);
            this.label14.TabIndex = 0;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label15.Location = new System.Drawing.Point(3, 29);
            this.label15.Margin = new System.Windows.Forms.Padding(3, 13, 3, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(16, 16);
            this.label15.TabIndex = 0;
            this.label15.Text = "0";
            // 
            // label16
            // 
            this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(3, 66);
            this.label16.Margin = new System.Windows.Forms.Padding(3, 21, 3, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(16, 16);
            this.label16.TabIndex = 0;
            this.label16.Text = "1";
            // 
            // label17
            // 
            this.label17.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(3, 103);
            this.label17.Margin = new System.Windows.Forms.Padding(3, 21, 3, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(16, 16);
            this.label17.TabIndex = 0;
            this.label17.Text = "2";
            // 
            // label18
            // 
            this.label18.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label18.Location = new System.Drawing.Point(3, 140);
            this.label18.Margin = new System.Windows.Forms.Padding(3, 21, 3, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(16, 16);
            this.label18.TabIndex = 0;
            this.label18.Text = "3";
            // 
            // label19
            // 
            this.label19.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label19.Location = new System.Drawing.Point(3, 177);
            this.label19.Margin = new System.Windows.Forms.Padding(3, 21, 3, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(16, 16);
            this.label19.TabIndex = 0;
            this.label19.Text = "4";
            // 
            // label20
            // 
            this.label20.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label20.Location = new System.Drawing.Point(3, 214);
            this.label20.Margin = new System.Windows.Forms.Padding(3, 21, 3, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(16, 16);
            this.label20.TabIndex = 0;
            this.label20.Text = "5";
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label21.Location = new System.Drawing.Point(3, 251);
            this.label21.Margin = new System.Windows.Forms.Padding(3, 21, 3, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(16, 16);
            this.label21.TabIndex = 0;
            this.label21.Text = "6";
            // 
            // label22
            // 
            this.label22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label22.Location = new System.Drawing.Point(3, 288);
            this.label22.Margin = new System.Windows.Forms.Padding(3, 21, 3, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(16, 16);
            this.label22.TabIndex = 0;
            this.label22.Text = "7";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.label1);
            this.flowLayoutPanel2.Controls.Add(this.leds20);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(22, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(40, 319);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "20";
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this.label2);
            this.flowLayoutPanel3.Controls.Add(this.leds21);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(62, 0);
            this.flowLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel3.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(7, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 16);
            this.label2.TabIndex = 0;
            this.label2.Text = "21";
            // 
            // leds21
            // 
            // 
            // 
            // 
            this.leds21.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds21.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds21.ItemTemplate.Name = "";
            this.leds21.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds21.ItemTemplate.TabIndex = 0;
            this.leds21.ItemTemplate.TabStop = false;
            this.leds21.Location = new System.Drawing.Point(0, 19);
            this.leds21.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds21.Name = "leds21";
            this.leds21.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds21.Size = new System.Drawing.Size(38, 297);
            this.leds21.TabIndex = 2;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.AutoSize = true;
            this.flowLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel4.Controls.Add(this.label3);
            this.flowLayoutPanel4.Controls.Add(this.leds22);
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(100, 0);
            this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel4.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(7, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 0;
            this.label3.Text = "22";
            // 
            // leds22
            // 
            // 
            // 
            // 
            this.leds22.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds22.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds22.ItemTemplate.Name = "";
            this.leds22.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds22.ItemTemplate.TabIndex = 0;
            this.leds22.ItemTemplate.TabStop = false;
            this.leds22.Location = new System.Drawing.Point(0, 19);
            this.leds22.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds22.Name = "leds22";
            this.leds22.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds22.Size = new System.Drawing.Size(38, 297);
            this.leds22.TabIndex = 2;
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.AutoSize = true;
            this.flowLayoutPanel5.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel5.Controls.Add(this.label4);
            this.flowLayoutPanel5.Controls.Add(this.leds23);
            this.flowLayoutPanel5.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(138, 0);
            this.flowLayoutPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel5.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(7, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "23";
            // 
            // leds23
            // 
            // 
            // 
            // 
            this.leds23.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds23.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds23.ItemTemplate.Name = "";
            this.leds23.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds23.ItemTemplate.TabIndex = 0;
            this.leds23.ItemTemplate.TabStop = false;
            this.leds23.Location = new System.Drawing.Point(0, 19);
            this.leds23.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds23.Name = "leds23";
            this.leds23.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds23.Size = new System.Drawing.Size(38, 297);
            this.leds23.TabIndex = 2;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.AutoSize = true;
            this.flowLayoutPanel6.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel6.Controls.Add(this.label5);
            this.flowLayoutPanel6.Controls.Add(this.leds24);
            this.flowLayoutPanel6.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel6.Location = new System.Drawing.Point(176, 0);
            this.flowLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel6.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(7, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "24";
            // 
            // leds24
            // 
            // 
            // 
            // 
            this.leds24.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds24.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds24.ItemTemplate.Name = "";
            this.leds24.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds24.ItemTemplate.TabIndex = 0;
            this.leds24.ItemTemplate.TabStop = false;
            this.leds24.Location = new System.Drawing.Point(0, 19);
            this.leds24.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds24.Name = "leds24";
            this.leds24.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds24.Size = new System.Drawing.Size(38, 297);
            this.leds24.TabIndex = 2;
            // 
            // flowLayoutPanel7
            // 
            this.flowLayoutPanel7.AutoSize = true;
            this.flowLayoutPanel7.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel7.Controls.Add(this.label6);
            this.flowLayoutPanel7.Controls.Add(this.leds25);
            this.flowLayoutPanel7.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel7.Location = new System.Drawing.Point(214, 0);
            this.flowLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel7.Name = "flowLayoutPanel7";
            this.flowLayoutPanel7.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel7.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(7, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 16);
            this.label6.TabIndex = 0;
            this.label6.Text = "25";
            // 
            // leds25
            // 
            // 
            // 
            // 
            this.leds25.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds25.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds25.ItemTemplate.Name = "";
            this.leds25.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds25.ItemTemplate.TabIndex = 0;
            this.leds25.ItemTemplate.TabStop = false;
            this.leds25.Location = new System.Drawing.Point(0, 19);
            this.leds25.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds25.Name = "leds25";
            this.leds25.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds25.Size = new System.Drawing.Size(38, 297);
            this.leds25.TabIndex = 2;
            // 
            // flowLayoutPanel8
            // 
            this.flowLayoutPanel8.AutoSize = true;
            this.flowLayoutPanel8.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel8.Controls.Add(this.label7);
            this.flowLayoutPanel8.Controls.Add(this.leds26);
            this.flowLayoutPanel8.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel8.Location = new System.Drawing.Point(252, 0);
            this.flowLayoutPanel8.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel8.Name = "flowLayoutPanel8";
            this.flowLayoutPanel8.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel8.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(7, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 16);
            this.label7.TabIndex = 0;
            this.label7.Text = "26";
            // 
            // leds26
            // 
            // 
            // 
            // 
            this.leds26.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds26.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds26.ItemTemplate.Name = "";
            this.leds26.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds26.ItemTemplate.TabIndex = 0;
            this.leds26.ItemTemplate.TabStop = false;
            this.leds26.Location = new System.Drawing.Point(0, 19);
            this.leds26.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds26.Name = "leds26";
            this.leds26.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds26.Size = new System.Drawing.Size(38, 297);
            this.leds26.TabIndex = 2;
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.AutoSize = true;
            this.flowLayoutPanel9.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel9.Controls.Add(this.label8);
            this.flowLayoutPanel9.Controls.Add(this.leds27);
            this.flowLayoutPanel9.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel9.Location = new System.Drawing.Point(290, 0);
            this.flowLayoutPanel9.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            this.flowLayoutPanel9.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel9.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(7, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 16);
            this.label8.TabIndex = 0;
            this.label8.Text = "27";
            // 
            // leds27
            // 
            // 
            // 
            // 
            this.leds27.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds27.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds27.ItemTemplate.Name = "";
            this.leds27.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds27.ItemTemplate.TabIndex = 0;
            this.leds27.ItemTemplate.TabStop = false;
            this.leds27.Location = new System.Drawing.Point(0, 19);
            this.leds27.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds27.Name = "leds27";
            this.leds27.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds27.Size = new System.Drawing.Size(38, 297);
            this.leds27.TabIndex = 2;
            // 
            // flowLayoutPanel10
            // 
            this.flowLayoutPanel10.AutoSize = true;
            this.flowLayoutPanel10.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel10.Controls.Add(this.label9);
            this.flowLayoutPanel10.Controls.Add(this.leds28);
            this.flowLayoutPanel10.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel10.Location = new System.Drawing.Point(328, 0);
            this.flowLayoutPanel10.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel10.Name = "flowLayoutPanel10";
            this.flowLayoutPanel10.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel10.TabIndex = 4;
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(7, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 16);
            this.label9.TabIndex = 0;
            this.label9.Text = "28";
            // 
            // leds28
            // 
            // 
            // 
            // 
            this.leds28.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds28.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds28.ItemTemplate.Name = "";
            this.leds28.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds28.ItemTemplate.TabIndex = 0;
            this.leds28.ItemTemplate.TabStop = false;
            this.leds28.Location = new System.Drawing.Point(0, 19);
            this.leds28.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds28.Name = "leds28";
            this.leds28.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds28.Size = new System.Drawing.Size(38, 297);
            this.leds28.TabIndex = 2;
            // 
            // flowLayoutPanel11
            // 
            this.flowLayoutPanel11.AutoSize = true;
            this.flowLayoutPanel11.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel11.Controls.Add(this.label10);
            this.flowLayoutPanel11.Controls.Add(this.leds31);
            this.flowLayoutPanel11.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel11.Location = new System.Drawing.Point(366, 0);
            this.flowLayoutPanel11.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel11.Name = "flowLayoutPanel11";
            this.flowLayoutPanel11.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel11.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(7, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 16);
            this.label10.TabIndex = 0;
            this.label10.Text = "31";
            // 
            // leds31
            // 
            // 
            // 
            // 
            this.leds31.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds31.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds31.ItemTemplate.Name = "";
            this.leds31.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds31.ItemTemplate.TabIndex = 0;
            this.leds31.ItemTemplate.TabStop = false;
            this.leds31.Location = new System.Drawing.Point(0, 19);
            this.leds31.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds31.Name = "leds31";
            this.leds31.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds31.Size = new System.Drawing.Size(38, 297);
            this.leds31.TabIndex = 2;
            // 
            // flowLayoutPanel12
            // 
            this.flowLayoutPanel12.AutoSize = true;
            this.flowLayoutPanel12.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel12.Controls.Add(this.label11);
            this.flowLayoutPanel12.Controls.Add(this.leds32);
            this.flowLayoutPanel12.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel12.Location = new System.Drawing.Point(404, 0);
            this.flowLayoutPanel12.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel12.Name = "flowLayoutPanel12";
            this.flowLayoutPanel12.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel12.TabIndex = 4;
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(7, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(24, 16);
            this.label11.TabIndex = 0;
            this.label11.Text = "32";
            // 
            // leds32
            // 
            // 
            // 
            // 
            this.leds32.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds32.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds32.ItemTemplate.Name = "";
            this.leds32.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds32.ItemTemplate.TabIndex = 0;
            this.leds32.ItemTemplate.TabStop = false;
            this.leds32.Location = new System.Drawing.Point(0, 19);
            this.leds32.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds32.Name = "leds32";
            this.leds32.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds32.Size = new System.Drawing.Size(38, 297);
            this.leds32.TabIndex = 2;
            // 
            // flowLayoutPanel14
            // 
            this.flowLayoutPanel14.AutoSize = true;
            this.flowLayoutPanel14.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel14.Controls.Add(this.label13);
            this.flowLayoutPanel14.Controls.Add(this.leds33);
            this.flowLayoutPanel14.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel14.Location = new System.Drawing.Point(442, 0);
            this.flowLayoutPanel14.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel14.Name = "flowLayoutPanel14";
            this.flowLayoutPanel14.Size = new System.Drawing.Size(38, 319);
            this.flowLayoutPanel14.TabIndex = 4;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(7, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 16);
            this.label13.TabIndex = 0;
            this.label13.Text = "33";
            // 
            // leds33
            // 
            // 
            // 
            // 
            this.leds33.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds33.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds33.ItemTemplate.Name = "";
            this.leds33.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds33.ItemTemplate.TabIndex = 0;
            this.leds33.ItemTemplate.TabStop = false;
            this.leds33.Location = new System.Drawing.Point(0, 19);
            this.leds33.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.leds33.Name = "leds33";
            this.leds33.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.leds33.Size = new System.Drawing.Size(38, 297);
            this.leds33.TabIndex = 2;
            // 
            // flowLayoutPanel13
            // 
            this.flowLayoutPanel13.AutoSize = true;
            this.flowLayoutPanel13.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel13.Controls.Add(this.label12);
            this.flowLayoutPanel13.Controls.Add(this.leds34);
            this.flowLayoutPanel13.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel13.Location = new System.Drawing.Point(480, 0);
            this.flowLayoutPanel13.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel13.Name = "flowLayoutPanel13";
            this.flowLayoutPanel13.Size = new System.Drawing.Size(40, 319);
            this.flowLayoutPanel13.TabIndex = 4;
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(8, 0);
            this.label12.Margin = new System.Windows.Forms.Padding(3, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(24, 16);
            this.label12.TabIndex = 0;
            this.label12.Text = "34";
            // 
            // leds34
            // 
            // 
            // 
            // 
            this.leds34.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.leds34.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.leds34.ItemTemplate.Name = "";
            this.leds34.ItemTemplate.Size = new System.Drawing.Size(36, 36);
            this.leds34.ItemTemplate.TabIndex = 0;
            this.leds34.ItemTemplate.TabStop = false;
            this.leds34.Location = new System.Drawing.Point(0, 19);
            this.leds34.Margin = new System.Windows.Forms.Padding(0, 3, 2, 3);
            this.leds34.Name = "leds34";
            this.leds34.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(1);
            this.leds34.Size = new System.Drawing.Size(38, 297);
            this.leds34.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanel16);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.flowLayoutPanel19);
            this.splitContainer1.Size = new System.Drawing.Size(996, 547);
            this.splitContainer1.SplitterDistance = 452;
            this.splitContainer1.TabIndex = 6;
            // 
            // flowLayoutPanel16
            // 
            this.flowLayoutPanel16.Controls.Add(this.groupBox4);
            this.flowLayoutPanel16.Controls.Add(this.tabControl1);
            this.flowLayoutPanel16.Controls.Add(this.groupBox7);
            this.flowLayoutPanel16.Controls.Add(this.groupBox6);
            this.flowLayoutPanel16.Controls.Add(this.groupBox8);
            this.flowLayoutPanel16.Controls.Add(this.groupBox9);
            this.flowLayoutPanel16.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel16.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel16.Name = "flowLayoutPanel16";
            this.flowLayoutPanel16.Size = new System.Drawing.Size(452, 547);
            this.flowLayoutPanel16.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.swPwrOutput);
            this.groupBox4.Controls.Add(this.swPwrOcp);
            this.groupBox4.Controls.Add(this.swPwrOpen);
            this.groupBox4.Controls.Add(this.btnOvp);
            this.groupBox4.Controls.Add(this.btnSetCurr);
            this.groupBox4.Controls.Add(this.btnSetVolt);
            this.groupBox4.Controls.Add(this.numSetCurr);
            this.groupBox4.Controls.Add(this.numOvp);
            this.groupBox4.Controls.Add(this.numSetVolt);
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(442, 92);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Power";
            // 
            // swPwrOutput
            // 
            this.swPwrOutput.Caption = "Output";
            this.swPwrOutput.Location = new System.Drawing.Point(132, 20);
            this.swPwrOutput.Name = "swPwrOutput";
            this.swPwrOutput.OffColor = System.Drawing.Color.DarkGreen;
            this.swPwrOutput.OnColor = System.Drawing.Color.Lime;
            this.swPwrOutput.Size = new System.Drawing.Size(63, 70);
            this.swPwrOutput.SwitchStyle = NationalInstruments.UI.SwitchStyle.PushButton3D;
            this.swPwrOutput.TabIndex = 7;
            this.swPwrOutput.StateChanging += new NationalInstruments.UI.ActionCancelEventHandler(this.swPwrOutput_StateChanging);
            // 
            // swPwrOcp
            // 
            this.swPwrOcp.Caption = "OCP";
            this.swPwrOcp.Location = new System.Drawing.Point(69, 20);
            this.swPwrOcp.Name = "swPwrOcp";
            this.swPwrOcp.OffColor = System.Drawing.Color.DarkGreen;
            this.swPwrOcp.OnColor = System.Drawing.Color.Lime;
            this.swPwrOcp.Size = new System.Drawing.Size(63, 70);
            this.swPwrOcp.SwitchStyle = NationalInstruments.UI.SwitchStyle.PushButton3D;
            this.swPwrOcp.TabIndex = 7;
            this.swPwrOcp.StateChanging += new NationalInstruments.UI.ActionCancelEventHandler(this.swPwrOcp_StateChanging);
            // 
            // swPwrOpen
            // 
            this.swPwrOpen.Caption = "Open";
            this.swPwrOpen.Location = new System.Drawing.Point(6, 20);
            this.swPwrOpen.Name = "swPwrOpen";
            this.swPwrOpen.OffColor = System.Drawing.Color.DarkGreen;
            this.swPwrOpen.OnColor = System.Drawing.Color.Lime;
            this.swPwrOpen.Size = new System.Drawing.Size(63, 70);
            this.swPwrOpen.SwitchStyle = NationalInstruments.UI.SwitchStyle.PushButton3D;
            this.swPwrOpen.TabIndex = 7;
            this.swPwrOpen.StateChanging += new NationalInstruments.UI.ActionCancelEventHandler(this.swPwrOpen_StateChanging);
            // 
            // btnOvp
            // 
            this.btnOvp.Location = new System.Drawing.Point(201, 56);
            this.btnOvp.Name = "btnOvp";
            this.btnOvp.Size = new System.Drawing.Size(75, 23);
            this.btnOvp.TabIndex = 0;
            this.btnOvp.Text = "SetOvp";
            this.btnOvp.UseVisualStyleBackColor = true;
            this.btnOvp.Click += new System.EventHandler(this.btnOvp_Click);
            // 
            // btnSetCurr
            // 
            this.btnSetCurr.Location = new System.Drawing.Point(282, 56);
            this.btnSetCurr.Name = "btnSetCurr";
            this.btnSetCurr.Size = new System.Drawing.Size(75, 23);
            this.btnSetCurr.TabIndex = 0;
            this.btnSetCurr.Text = "SetCurr";
            this.btnSetCurr.UseVisualStyleBackColor = true;
            this.btnSetCurr.Click += new System.EventHandler(this.btnSetCurr_Click);
            // 
            // btnSetVolt
            // 
            this.btnSetVolt.Location = new System.Drawing.Point(282, 20);
            this.btnSetVolt.Name = "btnSetVolt";
            this.btnSetVolt.Size = new System.Drawing.Size(75, 23);
            this.btnSetVolt.TabIndex = 0;
            this.btnSetVolt.Text = "SetVolt";
            this.btnSetVolt.UseVisualStyleBackColor = true;
            this.btnSetVolt.Click += new System.EventHandler(this.btnSetVolt_Click);
            // 
            // numSetCurr
            // 
            this.numSetCurr.DecimalPlaces = 2;
            this.numSetCurr.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSetCurr.Location = new System.Drawing.Point(363, 57);
            this.numSetCurr.Name = "numSetCurr";
            this.numSetCurr.Size = new System.Drawing.Size(66, 21);
            this.numSetCurr.TabIndex = 6;
            // 
            // numOvp
            // 
            this.numOvp.DecimalPlaces = 2;
            this.numOvp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numOvp.Location = new System.Drawing.Point(201, 21);
            this.numOvp.Name = "numOvp";
            this.numOvp.Size = new System.Drawing.Size(75, 21);
            this.numOvp.TabIndex = 6;
            // 
            // numSetVolt
            // 
            this.numSetVolt.DecimalPlaces = 2;
            this.numSetVolt.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSetVolt.Location = new System.Drawing.Point(363, 21);
            this.numSetVolt.Name = "numSetVolt";
            this.numSetVolt.Size = new System.Drawing.Size(66, 21);
            this.numSetVolt.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pagePrs0);
            this.tabControl1.Controls.Add(this.pagePrs1);
            this.tabControl1.ItemSize = new System.Drawing.Size(60, 18);
            this.tabControl1.Location = new System.Drawing.Point(0, 98);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(173, 105);
            this.tabControl1.TabIndex = 0;
            // 
            // pagePrs0
            // 
            this.pagePrs0.BackColor = System.Drawing.SystemColors.Control;
            this.pagePrs0.Controls.Add(this.swRes0Open);
            this.pagePrs0.Controls.Add(this.btnSetRes0);
            this.pagePrs0.Controls.Add(this.numSetRes0);
            this.pagePrs0.Location = new System.Drawing.Point(4, 22);
            this.pagePrs0.Name = "pagePrs0";
            this.pagePrs0.Padding = new System.Windows.Forms.Padding(3);
            this.pagePrs0.Size = new System.Drawing.Size(165, 79);
            this.pagePrs0.TabIndex = 0;
            this.pagePrs0.Text = "PRS #1";
            // 
            // swRes0Open
            // 
            this.swRes0Open.Caption = "Open";
            this.swRes0Open.CaptionVerticalOrientation = NationalInstruments.UI.VerticalCaptionOrientation.BottomToTop;
            this.swRes0Open.Location = new System.Drawing.Point(3, 9);
            this.swRes0Open.Name = "swRes0Open";
            this.swRes0Open.OffColor = System.Drawing.Color.DarkGreen;
            this.swRes0Open.OnColor = System.Drawing.Color.Lime;
            this.swRes0Open.Size = new System.Drawing.Size(63, 70);
            this.swRes0Open.SwitchStyle = NationalInstruments.UI.SwitchStyle.PushButton3D;
            this.swRes0Open.TabIndex = 7;
            this.swRes0Open.StateChanging += new NationalInstruments.UI.ActionCancelEventHandler(this.swResOpen_StateChanging);
            // 
            // btnSetRes0
            // 
            this.btnSetRes0.Location = new System.Drawing.Point(72, 44);
            this.btnSetRes0.Name = "btnSetRes0";
            this.btnSetRes0.Size = new System.Drawing.Size(75, 23);
            this.btnSetRes0.TabIndex = 0;
            this.btnSetRes0.Text = "SetRes";
            this.btnSetRes0.UseVisualStyleBackColor = true;
            this.btnSetRes0.Click += new System.EventHandler(this.btnSetRes_Click);
            // 
            // numSetRes0
            // 
            this.numSetRes0.DecimalPlaces = 2;
            this.numSetRes0.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSetRes0.Location = new System.Drawing.Point(72, 12);
            this.numSetRes0.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numSetRes0.Name = "numSetRes0";
            this.numSetRes0.Size = new System.Drawing.Size(75, 21);
            this.numSetRes0.TabIndex = 6;
            // 
            // pagePrs1
            // 
            this.pagePrs1.BackColor = System.Drawing.SystemColors.Control;
            this.pagePrs1.Controls.Add(this.swRes1Open);
            this.pagePrs1.Controls.Add(this.btnSetRes1);
            this.pagePrs1.Controls.Add(this.numSetRes1);
            this.pagePrs1.Location = new System.Drawing.Point(4, 22);
            this.pagePrs1.Name = "pagePrs1";
            this.pagePrs1.Padding = new System.Windows.Forms.Padding(3);
            this.pagePrs1.Size = new System.Drawing.Size(165, 79);
            this.pagePrs1.TabIndex = 1;
            this.pagePrs1.Text = "PRS #2";
            // 
            // swRes1Open
            // 
            this.swRes1Open.Caption = "Open";
            this.swRes1Open.CaptionVerticalOrientation = NationalInstruments.UI.VerticalCaptionOrientation.BottomToTop;
            this.swRes1Open.Location = new System.Drawing.Point(3, 9);
            this.swRes1Open.Name = "swRes1Open";
            this.swRes1Open.OffColor = System.Drawing.Color.DarkGreen;
            this.swRes1Open.OnColor = System.Drawing.Color.Lime;
            this.swRes1Open.Size = new System.Drawing.Size(63, 70);
            this.swRes1Open.SwitchStyle = NationalInstruments.UI.SwitchStyle.PushButton3D;
            this.swRes1Open.TabIndex = 7;
            this.swRes1Open.StateChanging += new NationalInstruments.UI.ActionCancelEventHandler(this.swRes1Open_StateChanging);
            // 
            // btnSetRes1
            // 
            this.btnSetRes1.Location = new System.Drawing.Point(72, 44);
            this.btnSetRes1.Name = "btnSetRes1";
            this.btnSetRes1.Size = new System.Drawing.Size(75, 23);
            this.btnSetRes1.TabIndex = 0;
            this.btnSetRes1.Text = "SetRes";
            this.btnSetRes1.UseVisualStyleBackColor = true;
            this.btnSetRes1.Click += new System.EventHandler(this.btnSetRes1_Click);
            // 
            // numSetRes1
            // 
            this.numSetRes1.DecimalPlaces = 2;
            this.numSetRes1.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSetRes1.Location = new System.Drawing.Point(72, 12);
            this.numSetRes1.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numSetRes1.Name = "numSetRes1";
            this.numSetRes1.Size = new System.Drawing.Size(75, 21);
            this.numSetRes1.TabIndex = 6;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.swChamRun);
            this.groupBox7.Controls.Add(this.btnSetTemp);
            this.groupBox7.Controls.Add(this.btnGetTemp);
            this.groupBox7.Controls.Add(this.numSetTemp);
            this.groupBox7.Controls.Add(this.numGetTemp);
            this.groupBox7.Location = new System.Drawing.Point(176, 101);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(258, 102);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Chamber";
            // 
            // swChamRun
            // 
            this.swChamRun.Caption = "Run";
            this.swChamRun.Location = new System.Drawing.Point(9, 22);
            this.swChamRun.Name = "swChamRun";
            this.swChamRun.OffColor = System.Drawing.Color.DarkGreen;
            this.swChamRun.OnColor = System.Drawing.Color.Lime;
            this.swChamRun.Size = new System.Drawing.Size(63, 70);
            this.swChamRun.SwitchStyle = NationalInstruments.UI.SwitchStyle.PushButton3D;
            this.swChamRun.TabIndex = 7;
            this.swChamRun.StateChanging += new NationalInstruments.UI.ActionCancelEventHandler(this.swChamRun_StateChanging);
            // 
            // btnSetTemp
            // 
            this.btnSetTemp.Location = new System.Drawing.Point(98, 55);
            this.btnSetTemp.Name = "btnSetTemp";
            this.btnSetTemp.Size = new System.Drawing.Size(75, 23);
            this.btnSetTemp.TabIndex = 0;
            this.btnSetTemp.Text = "SetTemp";
            this.btnSetTemp.UseVisualStyleBackColor = true;
            this.btnSetTemp.Click += new System.EventHandler(this.btnSetTemp_Click);
            // 
            // btnGetTemp
            // 
            this.btnGetTemp.Location = new System.Drawing.Point(98, 20);
            this.btnGetTemp.Name = "btnGetTemp";
            this.btnGetTemp.Size = new System.Drawing.Size(75, 23);
            this.btnGetTemp.TabIndex = 0;
            this.btnGetTemp.Text = "GetTemp";
            this.btnGetTemp.UseVisualStyleBackColor = true;
            this.btnGetTemp.Click += new System.EventHandler(this.btnGetTemp_Click);
            // 
            // numSetTemp
            // 
            this.numSetTemp.DecimalPlaces = 1;
            this.numSetTemp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numSetTemp.Location = new System.Drawing.Point(179, 55);
            this.numSetTemp.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numSetTemp.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numSetTemp.Name = "numSetTemp";
            this.numSetTemp.Size = new System.Drawing.Size(66, 21);
            this.numSetTemp.TabIndex = 6;
            // 
            // numGetTemp
            // 
            this.numGetTemp.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.numGetTemp.DecimalPlaces = 1;
            this.numGetTemp.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numGetTemp.Location = new System.Drawing.Point(179, 20);
            this.numGetTemp.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numGetTemp.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.numGetTemp.Name = "numGetTemp";
            this.numGetTemp.ReadOnly = true;
            this.numGetTemp.Size = new System.Drawing.Size(66, 21);
            this.numGetTemp.TabIndex = 6;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.swDmmOpen);
            this.groupBox6.Controls.Add(this.btnGetFreq);
            this.groupBox6.Controls.Add(this.btnGetFRes);
            this.groupBox6.Controls.Add(this.numGetRes);
            this.groupBox6.Controls.Add(this.numGetFreq);
            this.groupBox6.Controls.Add(this.btnGetCurr);
            this.groupBox6.Controls.Add(this.numGetFRes);
            this.groupBox6.Controls.Add(this.numGetVolt);
            this.groupBox6.Controls.Add(this.btnGetRes);
            this.groupBox6.Controls.Add(this.numGetCurr);
            this.groupBox6.Controls.Add(this.btnGetVolt);
            this.groupBox6.Location = new System.Drawing.Point(3, 209);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(442, 124);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Multimeter";
            // 
            // swDmmOpen
            // 
            this.swDmmOpen.Caption = "Open";
            this.swDmmOpen.Location = new System.Drawing.Point(6, 35);
            this.swDmmOpen.Name = "swDmmOpen";
            this.swDmmOpen.OffColor = System.Drawing.Color.DarkGreen;
            this.swDmmOpen.OnColor = System.Drawing.Color.Lime;
            this.swDmmOpen.Size = new System.Drawing.Size(63, 70);
            this.swDmmOpen.SwitchStyle = NationalInstruments.UI.SwitchStyle.PushButton3D;
            this.swDmmOpen.TabIndex = 7;
            this.swDmmOpen.StateChanging += new NationalInstruments.UI.ActionCancelEventHandler(this.swDmmOpen_StateChanging);
            // 
            // btnGetFreq
            // 
            this.btnGetFreq.Location = new System.Drawing.Point(282, 51);
            this.btnGetFreq.Name = "btnGetFreq";
            this.btnGetFreq.Size = new System.Drawing.Size(75, 23);
            this.btnGetFreq.TabIndex = 0;
            this.btnGetFreq.Text = "GetFreq";
            this.btnGetFreq.UseVisualStyleBackColor = true;
            this.btnGetFreq.Click += new System.EventHandler(this.btnGetFreq_Click);
            // 
            // btnGetFRes
            // 
            this.btnGetFRes.Location = new System.Drawing.Point(282, 20);
            this.btnGetFRes.Name = "btnGetFRes";
            this.btnGetFRes.Size = new System.Drawing.Size(75, 23);
            this.btnGetFRes.TabIndex = 0;
            this.btnGetFRes.Text = "GetFRes";
            this.btnGetFRes.UseVisualStyleBackColor = true;
            this.btnGetFRes.Click += new System.EventHandler(this.btnGetFRes_Click);
            // 
            // numGetRes
            // 
            this.numGetRes.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.numGetRes.DecimalPlaces = 2;
            this.numGetRes.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numGetRes.Location = new System.Drawing.Point(171, 88);
            this.numGetRes.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numGetRes.Name = "numGetRes";
            this.numGetRes.ReadOnly = true;
            this.numGetRes.Size = new System.Drawing.Size(66, 21);
            this.numGetRes.TabIndex = 6;
            // 
            // numGetFreq
            // 
            this.numGetFreq.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.numGetFreq.DecimalPlaces = 2;
            this.numGetFreq.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numGetFreq.Location = new System.Drawing.Point(363, 51);
            this.numGetFreq.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numGetFreq.Name = "numGetFreq";
            this.numGetFreq.ReadOnly = true;
            this.numGetFreq.Size = new System.Drawing.Size(66, 21);
            this.numGetFreq.TabIndex = 6;
            // 
            // btnGetCurr
            // 
            this.btnGetCurr.Location = new System.Drawing.Point(90, 54);
            this.btnGetCurr.Name = "btnGetCurr";
            this.btnGetCurr.Size = new System.Drawing.Size(75, 23);
            this.btnGetCurr.TabIndex = 0;
            this.btnGetCurr.Text = "GetCurr";
            this.btnGetCurr.UseVisualStyleBackColor = true;
            this.btnGetCurr.Click += new System.EventHandler(this.btnGetCurr_Click);
            // 
            // numGetFRes
            // 
            this.numGetFRes.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.numGetFRes.DecimalPlaces = 2;
            this.numGetFRes.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numGetFRes.Location = new System.Drawing.Point(363, 20);
            this.numGetFRes.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numGetFRes.Name = "numGetFRes";
            this.numGetFRes.ReadOnly = true;
            this.numGetFRes.Size = new System.Drawing.Size(66, 21);
            this.numGetFRes.TabIndex = 6;
            // 
            // numGetVolt
            // 
            this.numGetVolt.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.numGetVolt.DecimalPlaces = 2;
            this.numGetVolt.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numGetVolt.Location = new System.Drawing.Point(171, 20);
            this.numGetVolt.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numGetVolt.Name = "numGetVolt";
            this.numGetVolt.ReadOnly = true;
            this.numGetVolt.Size = new System.Drawing.Size(66, 21);
            this.numGetVolt.TabIndex = 6;
            // 
            // btnGetRes
            // 
            this.btnGetRes.Location = new System.Drawing.Point(90, 88);
            this.btnGetRes.Name = "btnGetRes";
            this.btnGetRes.Size = new System.Drawing.Size(75, 23);
            this.btnGetRes.TabIndex = 0;
            this.btnGetRes.Text = "GetRes";
            this.btnGetRes.UseVisualStyleBackColor = true;
            this.btnGetRes.Click += new System.EventHandler(this.btnGetRes_Click);
            // 
            // numGetCurr
            // 
            this.numGetCurr.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.numGetCurr.DecimalPlaces = 2;
            this.numGetCurr.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numGetCurr.Location = new System.Drawing.Point(171, 54);
            this.numGetCurr.Maximum = new decimal(new int[] {
            10000000,
            0,
            0,
            0});
            this.numGetCurr.Name = "numGetCurr";
            this.numGetCurr.ReadOnly = true;
            this.numGetCurr.Size = new System.Drawing.Size(66, 21);
            this.numGetCurr.TabIndex = 6;
            // 
            // btnGetVolt
            // 
            this.btnGetVolt.Location = new System.Drawing.Point(90, 20);
            this.btnGetVolt.Name = "btnGetVolt";
            this.btnGetVolt.Size = new System.Drawing.Size(75, 23);
            this.btnGetVolt.TabIndex = 0;
            this.btnGetVolt.Text = "GetVolt";
            this.btnGetVolt.UseVisualStyleBackColor = true;
            this.btnGetVolt.Click += new System.EventHandler(this.btnGetVolt_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.label34);
            this.groupBox8.Controls.Add(this.label25);
            this.groupBox8.Controls.Add(this.txtResult);
            this.groupBox8.Controls.Add(this.txtCommand);
            this.groupBox8.Controls.Add(this.cmbInstrName);
            this.groupBox8.Controls.Add(this.btnWrite);
            this.groupBox8.Location = new System.Drawing.Point(3, 339);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(442, 86);
            this.groupBox8.TabIndex = 4;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Advanced";
            // 
            // label34
            // 
            this.label34.AutoSize = true;
            this.label34.Location = new System.Drawing.Point(119, 56);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(41, 12);
            this.label34.TabIndex = 2;
            this.label34.Text = "Result";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(113, 23);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(47, 12);
            this.label25.TabIndex = 2;
            this.label25.Text = "Command";
            // 
            // txtResult
            // 
            this.txtResult.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtResult.Location = new System.Drawing.Point(171, 48);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ReadOnly = true;
            this.txtResult.Size = new System.Drawing.Size(258, 29);
            this.txtResult.TabIndex = 1;
            // 
            // txtCommand
            // 
            this.txtCommand.Location = new System.Drawing.Point(171, 20);
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.Size = new System.Drawing.Size(258, 21);
            this.txtCommand.TabIndex = 1;
            // 
            // cmbInstrName
            // 
            this.cmbInstrName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInstrName.FormattingEnabled = true;
            this.cmbInstrName.Location = new System.Drawing.Point(6, 20);
            this.cmbInstrName.Name = "cmbInstrName";
            this.cmbInstrName.Size = new System.Drawing.Size(75, 20);
            this.cmbInstrName.TabIndex = 0;
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(6, 51);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(75, 23);
            this.btnWrite.TabIndex = 0;
            this.btnWrite.Text = "Query";
            this.btnWrite.UseVisualStyleBackColor = true;
            this.btnWrite.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.flowLayoutPanel18);
            this.groupBox9.Controls.Add(this.flowLayoutPanel1);
            this.groupBox9.Controls.Add(this.swDaqOpen);
            this.groupBox9.Location = new System.Drawing.Point(3, 431);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(442, 111);
            this.groupBox9.TabIndex = 5;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "DAQ Card";
            // 
            // flowLayoutPanel18
            // 
            this.flowLayoutPanel18.AutoSize = true;
            this.flowLayoutPanel18.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel18.Controls.Add(this.label23);
            this.flowLayoutPanel18.Controls.Add(this.numRelayIndex);
            this.flowLayoutPanel18.Controls.Add(this.btnRelayEnable);
            this.flowLayoutPanel18.Controls.Add(this.btnRelayDisable);
            this.flowLayoutPanel18.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel18.Location = new System.Drawing.Point(193, 14);
            this.flowLayoutPanel18.Name = "flowLayoutPanel18";
            this.flowLayoutPanel18.Size = new System.Drawing.Size(81, 97);
            this.flowLayoutPanel18.TabIndex = 9;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(3, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(35, 12);
            this.label23.TabIndex = 7;
            this.label23.Text = "Relay";
            // 
            // numRelayIndex
            // 
            this.numRelayIndex.Location = new System.Drawing.Point(3, 15);
            this.numRelayIndex.Maximum = new decimal(new int[] {
            340,
            0,
            0,
            0});
            this.numRelayIndex.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numRelayIndex.Name = "numRelayIndex";
            this.numRelayIndex.Size = new System.Drawing.Size(75, 21);
            this.numRelayIndex.TabIndex = 6;
            this.numRelayIndex.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // btnRelayEnable
            // 
            this.btnRelayEnable.Location = new System.Drawing.Point(3, 42);
            this.btnRelayEnable.Name = "btnRelayEnable";
            this.btnRelayEnable.Size = new System.Drawing.Size(75, 23);
            this.btnRelayEnable.TabIndex = 0;
            this.btnRelayEnable.Text = "Enable";
            this.btnRelayEnable.UseVisualStyleBackColor = true;
            this.btnRelayEnable.Click += new System.EventHandler(this.btnRelayEnable_Click);
            // 
            // btnRelayDisable
            // 
            this.btnRelayDisable.Location = new System.Drawing.Point(3, 71);
            this.btnRelayDisable.Name = "btnRelayDisable";
            this.btnRelayDisable.Size = new System.Drawing.Size(75, 23);
            this.btnRelayDisable.TabIndex = 0;
            this.btnRelayDisable.Text = "Disable";
            this.btnRelayDisable.UseVisualStyleBackColor = true;
            this.btnRelayDisable.Click += new System.EventHandler(this.btnRelayDisable_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.label24);
            this.flowLayoutPanel1.Controls.Add(this.numBoardIndex);
            this.flowLayoutPanel1.Controls.Add(this.btnBoardOpen);
            this.flowLayoutPanel1.Controls.Add(this.btnBoardClose);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(90, 14);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(81, 97);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(3, 0);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(35, 12);
            this.label24.TabIndex = 7;
            this.label24.Text = "Board";
            // 
            // numBoardIndex
            // 
            this.numBoardIndex.Location = new System.Drawing.Point(3, 15);
            this.numBoardIndex.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.numBoardIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numBoardIndex.Name = "numBoardIndex";
            this.numBoardIndex.Size = new System.Drawing.Size(75, 21);
            this.numBoardIndex.TabIndex = 6;
            this.numBoardIndex.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnBoardOpen
            // 
            this.btnBoardOpen.Location = new System.Drawing.Point(3, 42);
            this.btnBoardOpen.Name = "btnBoardOpen";
            this.btnBoardOpen.Size = new System.Drawing.Size(75, 23);
            this.btnBoardOpen.TabIndex = 0;
            this.btnBoardOpen.Text = "Open";
            this.btnBoardOpen.UseVisualStyleBackColor = true;
            this.btnBoardOpen.Click += new System.EventHandler(this.btnBoardOpen_Click);
            // 
            // btnBoardClose
            // 
            this.btnBoardClose.Location = new System.Drawing.Point(3, 71);
            this.btnBoardClose.Name = "btnBoardClose";
            this.btnBoardClose.Size = new System.Drawing.Size(75, 23);
            this.btnBoardClose.TabIndex = 0;
            this.btnBoardClose.Text = "Close";
            this.btnBoardClose.UseVisualStyleBackColor = true;
            this.btnBoardClose.Click += new System.EventHandler(this.btnBoardClose_Click);
            // 
            // swDaqOpen
            // 
            this.swDaqOpen.Caption = "Open";
            this.swDaqOpen.Location = new System.Drawing.Point(6, 35);
            this.swDaqOpen.Name = "swDaqOpen";
            this.swDaqOpen.OffColor = System.Drawing.Color.DarkGreen;
            this.swDaqOpen.OnColor = System.Drawing.Color.Lime;
            this.swDaqOpen.Size = new System.Drawing.Size(63, 70);
            this.swDaqOpen.SwitchStyle = NationalInstruments.UI.SwitchStyle.PushButton3D;
            this.swDaqOpen.TabIndex = 7;
            this.swDaqOpen.StateChanging += new NationalInstruments.UI.ActionCancelEventHandler(this.swDaqOpen_StateChanging);
            // 
            // flowLayoutPanel19
            // 
            this.flowLayoutPanel19.AutoSize = true;
            this.flowLayoutPanel19.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel19.Controls.Add(this.groupBox2);
            this.flowLayoutPanel19.Controls.Add(this.groupBox1);
            this.flowLayoutPanel19.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel19.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel19.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel19.Name = "flowLayoutPanel19";
            this.flowLayoutPanel19.Size = new System.Drawing.Size(540, 547);
            this.flowLayoutPanel19.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox2.Controls.Add(this.flowBoard);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(532, 103);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sub Board Group";
            // 
            // flowBoard
            // 
            this.flowBoard.AutoSize = true;
            this.flowBoard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowBoard.Controls.Add(this.flowLayoutPanel17);
            this.flowBoard.Controls.Add(this.ledBoard);
            this.flowBoard.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowBoard.Location = new System.Drawing.Point(31, 12);
            this.flowBoard.Name = "flowBoard";
            this.flowBoard.Size = new System.Drawing.Size(425, 87);
            this.flowBoard.TabIndex = 5;
            // 
            // flowLayoutPanel17
            // 
            this.flowLayoutPanel17.AutoSize = true;
            this.flowLayoutPanel17.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel17.Controls.Add(this.label29);
            this.flowLayoutPanel17.Controls.Add(this.label26);
            this.flowLayoutPanel17.Controls.Add(this.label27);
            this.flowLayoutPanel17.Controls.Add(this.label28);
            this.flowLayoutPanel17.Controls.Add(this.label30);
            this.flowLayoutPanel17.Controls.Add(this.label31);
            this.flowLayoutPanel17.Controls.Add(this.label32);
            this.flowLayoutPanel17.Controls.Add(this.label33);
            this.flowLayoutPanel17.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel17.Name = "flowLayoutPanel17";
            this.flowLayoutPanel17.Size = new System.Drawing.Size(391, 16);
            this.flowLayoutPanel17.TabIndex = 6;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label29.Location = new System.Drawing.Point(17, 0);
            this.label29.Margin = new System.Windows.Forms.Padding(17, 0, 0, 0);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(17, 16);
            this.label29.TabIndex = 1;
            this.label29.Text = "1";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label26.Location = new System.Drawing.Point(68, 0);
            this.label26.Margin = new System.Windows.Forms.Padding(34, 0, 0, 0);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(17, 16);
            this.label26.TabIndex = 1;
            this.label26.Text = "2";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label27.Location = new System.Drawing.Point(119, 0);
            this.label27.Margin = new System.Windows.Forms.Padding(34, 0, 0, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(17, 16);
            this.label27.TabIndex = 1;
            this.label27.Text = "3";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label28.Location = new System.Drawing.Point(170, 0);
            this.label28.Margin = new System.Windows.Forms.Padding(34, 0, 0, 0);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(17, 16);
            this.label28.TabIndex = 1;
            this.label28.Text = "4";
            // 
            // label30
            // 
            this.label30.AutoSize = true;
            this.label30.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label30.Location = new System.Drawing.Point(221, 0);
            this.label30.Margin = new System.Windows.Forms.Padding(34, 0, 0, 0);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(17, 16);
            this.label30.TabIndex = 1;
            this.label30.Text = "5";
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label31.Location = new System.Drawing.Point(272, 0);
            this.label31.Margin = new System.Windows.Forms.Padding(34, 0, 0, 0);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(17, 16);
            this.label31.TabIndex = 1;
            this.label31.Text = "6";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label32.Location = new System.Drawing.Point(323, 0);
            this.label32.Margin = new System.Windows.Forms.Padding(34, 0, 0, 0);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(17, 16);
            this.label32.TabIndex = 1;
            this.label32.Text = "7";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold);
            this.label33.Location = new System.Drawing.Point(374, 0);
            this.label33.Margin = new System.Windows.Forms.Padding(34, 0, 0, 0);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(17, 16);
            this.label33.TabIndex = 1;
            this.label33.Text = "8";
            // 
            // ledBoard
            // 
            // 
            // 
            // 
            this.ledBoard.ItemTemplate.CaptionVisible = false;
            this.ledBoard.ItemTemplate.LedStyle = NationalInstruments.UI.LedStyle.Round3D;
            this.ledBoard.ItemTemplate.Location = new System.Drawing.Point(0, 0);
            this.ledBoard.ItemTemplate.Name = "";
            this.ledBoard.ItemTemplate.Size = new System.Drawing.Size(50, 50);
            this.ledBoard.ItemTemplate.TabIndex = 0;
            this.ledBoard.ItemTemplate.TabStop = false;
            this.ledBoard.LayoutMode = NationalInstruments.UI.ControlArrayLayoutMode.Horizontal;
            this.ledBoard.Location = new System.Drawing.Point(3, 22);
            this.ledBoard.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ledBoard.Name = "ledBoard";
            this.ledBoard.ScaleMode = NationalInstruments.UI.ControlArrayScaleMode.CreateFixedMode(8);
            this.ledBoard.Size = new System.Drawing.Size(419, 65);
            this.ledBoard.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowMainRelays);
            this.groupBox1.Controls.Add(this.flowSubRelays);
            this.groupBox1.Location = new System.Drawing.Point(3, 112);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(532, 430);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Relay Group";
            // 
            // flowMainRelays
            // 
            this.flowMainRelays.AutoSize = true;
            this.flowMainRelays.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowMainRelays.Controls.Add(this.led300);
            this.flowMainRelays.Controls.Add(this.led301);
            this.flowMainRelays.Controls.Add(this.led302);
            this.flowMainRelays.Location = new System.Drawing.Point(28, 20);
            this.flowMainRelays.Name = "flowMainRelays";
            this.flowMainRelays.Size = new System.Drawing.Size(138, 70);
            this.flowMainRelays.TabIndex = 6;
            // 
            // led300
            // 
            this.led300.Caption = "300";
            this.led300.CaptionBackColor = System.Drawing.SystemColors.Control;
            this.led300.CaptionFont = new System.Drawing.Font("宋体", 12F);
            this.led300.CaptionForeColor = System.Drawing.SystemColors.ControlText;
            this.led300.CaptionVerticalOrientation = NationalInstruments.UI.VerticalCaptionOrientation.BottomToTop;
            this.led300.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.led300.Location = new System.Drawing.Point(3, 3);
            this.led300.Name = "led300";
            this.led300.Size = new System.Drawing.Size(40, 64);
            this.led300.TabIndex = 6;
            // 
            // led301
            // 
            this.led301.Caption = "301";
            this.led301.CaptionBackColor = System.Drawing.SystemColors.Control;
            this.led301.CaptionFont = new System.Drawing.Font("宋体", 12F);
            this.led301.CaptionForeColor = System.Drawing.SystemColors.ControlText;
            this.led301.CaptionVerticalOrientation = NationalInstruments.UI.VerticalCaptionOrientation.BottomToTop;
            this.led301.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.led301.Location = new System.Drawing.Point(49, 3);
            this.led301.Name = "led301";
            this.led301.Size = new System.Drawing.Size(40, 64);
            this.led301.TabIndex = 6;
            // 
            // led302
            // 
            this.led302.Caption = "302";
            this.led302.CaptionBackColor = System.Drawing.SystemColors.Control;
            this.led302.CaptionFont = new System.Drawing.Font("宋体", 12F);
            this.led302.CaptionForeColor = System.Drawing.SystemColors.ControlText;
            this.led302.CaptionVerticalOrientation = NationalInstruments.UI.VerticalCaptionOrientation.BottomToTop;
            this.led302.LedStyle = NationalInstruments.UI.LedStyle.Square3D;
            this.led302.Location = new System.Drawing.Point(95, 3);
            this.led302.Name = "led302";
            this.led302.Size = new System.Drawing.Size(40, 64);
            this.led302.TabIndex = 6;
            // 
            // InstrumentControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(996, 569);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "InstrumentControlForm";
            this.Text = "InstrumentControl";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.InstrumentControlForm_FormClosed);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds20.ItemTemplate)).EndInit();
            this.flowSubRelays.ResumeLayout(false);
            this.flowSubRelays.PerformLayout();
            this.flowLayoutPanel15.ResumeLayout(false);
            this.flowLayoutPanel15.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds21.ItemTemplate)).EndInit();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds22.ItemTemplate)).EndInit();
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds23.ItemTemplate)).EndInit();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds24.ItemTemplate)).EndInit();
            this.flowLayoutPanel7.ResumeLayout(false);
            this.flowLayoutPanel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds25.ItemTemplate)).EndInit();
            this.flowLayoutPanel8.ResumeLayout(false);
            this.flowLayoutPanel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds26.ItemTemplate)).EndInit();
            this.flowLayoutPanel9.ResumeLayout(false);
            this.flowLayoutPanel9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds27.ItemTemplate)).EndInit();
            this.flowLayoutPanel10.ResumeLayout(false);
            this.flowLayoutPanel10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds28.ItemTemplate)).EndInit();
            this.flowLayoutPanel11.ResumeLayout(false);
            this.flowLayoutPanel11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds31.ItemTemplate)).EndInit();
            this.flowLayoutPanel12.ResumeLayout(false);
            this.flowLayoutPanel12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds32.ItemTemplate)).EndInit();
            this.flowLayoutPanel14.ResumeLayout(false);
            this.flowLayoutPanel14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds33.ItemTemplate)).EndInit();
            this.flowLayoutPanel13.ResumeLayout(false);
            this.flowLayoutPanel13.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.leds34.ItemTemplate)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.flowLayoutPanel16.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.swPwrOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.swPwrOcp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.swPwrOpen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetCurr)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numOvp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetVolt)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.pagePrs0.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.swRes0Open)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetRes0)).EndInit();
            this.pagePrs1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.swRes1Open)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetRes1)).EndInit();
            this.groupBox7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.swChamRun)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSetTemp)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetTemp)).EndInit();
            this.groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.swDmmOpen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetRes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetFreq)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetFRes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetVolt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGetCurr)).EndInit();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.flowLayoutPanel18.ResumeLayout(false);
            this.flowLayoutPanel18.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRelayIndex)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numBoardIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.swDaqOpen)).EndInit();
            this.flowLayoutPanel19.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowBoard.ResumeLayout(false);
            this.flowBoard.PerformLayout();
            this.flowLayoutPanel17.ResumeLayout(false);
            this.flowLayoutPanel17.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ledBoard.ItemTemplate)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowMainRelays.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.led300)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.led301)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.led302)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStatus;
        private NationalInstruments.UI.WindowsForms.LedArray leds20;
        private System.Windows.Forms.FlowLayoutPanel flowSubRelays;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label2;
        private NationalInstruments.UI.WindowsForms.LedArray leds21;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Label label3;
        private NationalInstruments.UI.WindowsForms.LedArray leds22;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.Label label4;
        private NationalInstruments.UI.WindowsForms.LedArray leds23;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.Label label5;
        private NationalInstruments.UI.WindowsForms.LedArray leds24;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
        private System.Windows.Forms.Label label6;
        private NationalInstruments.UI.WindowsForms.LedArray leds25;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel8;
        private System.Windows.Forms.Label label7;
        private NationalInstruments.UI.WindowsForms.LedArray leds26;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel9;
        private System.Windows.Forms.Label label8;
        private NationalInstruments.UI.WindowsForms.LedArray leds27;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel10;
        private System.Windows.Forms.Label label9;
        private NationalInstruments.UI.WindowsForms.LedArray leds28;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel11;
        private System.Windows.Forms.Label label10;
        private NationalInstruments.UI.WindowsForms.LedArray leds31;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel12;
        private System.Windows.Forms.Label label11;
        private NationalInstruments.UI.WindowsForms.LedArray leds32;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel14;
        private System.Windows.Forms.Label label13;
        private NationalInstruments.UI.WindowsForms.LedArray leds33;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel13;
        private System.Windows.Forms.Label label12;
        private NationalInstruments.UI.WindowsForms.LedArray leds34;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.NumericUpDown numSetVolt;
        private System.Windows.Forms.FlowLayoutPanel flowBoard;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel17;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label label28;
        private System.Windows.Forms.Label label30;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.Label label33;
        private NationalInstruments.UI.WindowsForms.LedArray ledBoard;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowMainRelays;
        private NationalInstruments.UI.WindowsForms.Led led300;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel16;
        private System.Windows.Forms.GroupBox groupBox4;
        private NationalInstruments.UI.WindowsForms.Switch swPwrOpen;
        private System.Windows.Forms.Button btnSetCurr;
        private System.Windows.Forms.Button btnSetVolt;
        private System.Windows.Forms.NumericUpDown numSetCurr;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
        private NationalInstruments.UI.WindowsForms.Switch swPwrOutput;
        private NationalInstruments.UI.WindowsForms.Switch swPwrOcp;
        private NationalInstruments.UI.WindowsForms.Switch swRes0Open;
        private NationalInstruments.UI.WindowsForms.Switch swDmmOpen;
        private System.Windows.Forms.Button btnGetFreq;
        private System.Windows.Forms.Button btnGetFRes;
        private System.Windows.Forms.NumericUpDown numGetRes;
        private System.Windows.Forms.NumericUpDown numGetFreq;
        private System.Windows.Forms.Button btnGetCurr;
        private System.Windows.Forms.NumericUpDown numGetFRes;
        private System.Windows.Forms.NumericUpDown numGetVolt;
        private System.Windows.Forms.Button btnGetRes;
        private System.Windows.Forms.NumericUpDown numGetCurr;
        private System.Windows.Forms.Button btnGetVolt;
        private NationalInstruments.UI.WindowsForms.Switch swChamRun;
        private System.Windows.Forms.Button btnSetTemp;
        private System.Windows.Forms.Button btnGetTemp;
        private System.Windows.Forms.NumericUpDown numSetTemp;
        private System.Windows.Forms.NumericUpDown numGetTemp;
        private NationalInstruments.UI.WindowsForms.Switch swDaqOpen;
        private System.Windows.Forms.Button btnRelayDisable;
        private System.Windows.Forms.Button btnRelayEnable;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel19;
        private System.Windows.Forms.NumericUpDown numBoardIndex;
        private System.Windows.Forms.Button btnBoardClose;
        private System.Windows.Forms.Button btnBoardOpen;
        private System.Windows.Forms.NumericUpDown numRelayIndex;
        private NationalInstruments.UI.WindowsForms.Led led301;
        private NationalInstruments.UI.WindowsForms.Led led302;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.Label label34;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.ComboBox cmbInstrName;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Button btnOvp;
        private System.Windows.Forms.NumericUpDown numOvp;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel18;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pagePrs0;
        private System.Windows.Forms.Button btnSetRes0;
        private System.Windows.Forms.NumericUpDown numSetRes0;
        private System.Windows.Forms.TabPage pagePrs1;
        private NationalInstruments.UI.WindowsForms.Switch swRes1Open;
        private System.Windows.Forms.Button btnSetRes1;
        private System.Windows.Forms.NumericUpDown numSetRes1;
    }
}