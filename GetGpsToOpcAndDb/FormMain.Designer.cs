namespace GetGpsToOpcAndDb
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_OpcInfo = new System.Windows.Forms.Label();
            this.checkBox_OpcAutoConnect = new System.Windows.Forms.CheckBox();
            this.textBox_RemoteServerIP = new System.Windows.Forms.MaskedTextBox();
            this.comboBox_RemoteServerName = new System.Windows.Forms.ComboBox();
            this.button_ServerEnum = new System.Windows.Forms.Button();
            this.btnConnLocalServer = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip_Opc = new System.Windows.Forms.StatusStrip();
            this.statusLabel_ServerState = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabel_ServerStartTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabel_Version = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numeric_GroupUpdateRate = new System.Windows.Forms.NumericUpDown();
            this.numeric_GroupsDeadband = new System.Windows.Forms.NumericUpDown();
            this.checkBox_IsGroupSubscribed = new System.Windows.Forms.CheckBox();
            this.checkBox_IsGroupActive = new System.Windows.Forms.CheckBox();
            this.checkBox_IsGroupsActive = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button_WriteTestItem = new System.Windows.Forms.Button();
            this.button_AddTestItem = new System.Windows.Forms.Button();
            this.textBox_TestValue = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox_TestItemId = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label_OpcError = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button_UiUpdate = new System.Windows.Forms.Button();
            this.checkBox_UiUpdateRecur = new System.Windows.Forms.CheckBox();
            this.textBox_PitchAngle = new System.Windows.Forms.TextBox();
            this.label_GpsQuality = new System.Windows.Forms.Label();
            this.label_MessageError = new System.Windows.Forms.Label();
            this.textBox_ClaimerId = new System.Windows.Forms.TextBox();
            this.textBox_GpsQuality = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label_TrackDirection = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox_MagDec = new System.Windows.Forms.TextBox();
            this.textBox_TrackLocalNorth = new System.Windows.Forms.TextBox();
            this.textBox_TrackDirection = new System.Windows.Forms.TextBox();
            this.textBox_LatitudeValue = new System.Windows.Forms.TextBox();
            this.textBox_Height = new System.Windows.Forms.TextBox();
            this.textBox_LongitudeValue = new System.Windows.Forms.TextBox();
            this.gpsallstr_txt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.comboBox_ConnMode = new System.Windows.Forms.ComboBox();
            this.label_ReconnCounter = new System.Windows.Forms.Label();
            this.checkBox_AutoCollect = new System.Windows.Forms.CheckBox();
            this.LblTcpState = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.textBox_Command = new System.Windows.Forms.TextBox();
            this.textBox_Port = new System.Windows.Forms.TextBox();
            this.textBox_IpAddress = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.button_Connect = new System.Windows.Forms.Button();
            this.button_StartCollect = new System.Windows.Forms.Button();
            this.button_Disconnect = new System.Windows.Forms.Button();
            this.timer_UiUpdate = new System.Windows.Forms.Timer(this.components);
            this.statusStrip_WebService = new System.Windows.Forms.StatusStrip();
            this.statusLabel_WebService = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer_Reconn = new System.Windows.Forms.Timer(this.components);
            this.timer_OpcUpdate = new System.Windows.Forms.Timer(this.components);
            this.timer_Upload = new System.Windows.Forms.Timer(this.components);
            this.button_Expand = new System.Windows.Forms.Button();
            this.textBox_Info = new System.Windows.Forms.TextBox();
            this.button_CopyToClipboard = new System.Windows.Forms.Button();
            this.label_Copied = new System.Windows.Forms.Label();
            this.button_RecordCoors = new System.Windows.Forms.Button();
            this.timer_RecordCoor = new System.Windows.Forms.Timer(this.components);
            this.statusLabel_DataService = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip_DataService = new System.Windows.Forms.StatusStrip();
            this.tcpServerMain = new SocketHelper.SocketTcpServer(this.components);
            this.tcpClient = new SocketHelper.SocketTcpClient(this.components);
            this.tcpServerConn = new SocketHelper.SocketTcpServer(this.components);
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.textBox_AntePitch = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox_BaselineLength = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.statusStrip_Opc.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_GroupUpdateRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_GroupsDeadband)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.statusStrip_WebService.SuspendLayout();
            this.statusStrip_DataService.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_OpcInfo);
            this.groupBox1.Controls.Add(this.checkBox_OpcAutoConnect);
            this.groupBox1.Controls.Add(this.textBox_RemoteServerIP);
            this.groupBox1.Controls.Add(this.comboBox_RemoteServerName);
            this.groupBox1.Controls.Add(this.button_ServerEnum);
            this.groupBox1.Controls.Add(this.btnConnLocalServer);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(15, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(497, 150);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "连接OPC服务器";
            // 
            // label_OpcInfo
            // 
            this.label_OpcInfo.AutoSize = true;
            this.label_OpcInfo.Location = new System.Drawing.Point(8, 120);
            this.label_OpcInfo.Name = "label_OpcInfo";
            this.label_OpcInfo.Size = new System.Drawing.Size(109, 20);
            this.label_OpcInfo.TabIndex = 6;
            this.label_OpcInfo.Text = "label_OpcInfo";
            // 
            // checkBox_OpcAutoConnect
            // 
            this.checkBox_OpcAutoConnect.AutoSize = true;
            this.checkBox_OpcAutoConnect.Location = new System.Drawing.Point(390, 77);
            this.checkBox_OpcAutoConnect.Name = "checkBox_OpcAutoConnect";
            this.checkBox_OpcAutoConnect.Size = new System.Drawing.Size(91, 24);
            this.checkBox_OpcAutoConnect.TabIndex = 5;
            this.checkBox_OpcAutoConnect.Text = "自动连接";
            this.checkBox_OpcAutoConnect.UseVisualStyleBackColor = true;
            this.checkBox_OpcAutoConnect.CheckedChanged += new System.EventHandler(this.CheckBox_AutoConnect_CheckedChanged);
            // 
            // textBox_RemoteServerIP
            // 
            this.textBox_RemoteServerIP.Location = new System.Drawing.Point(76, 33);
            this.textBox_RemoteServerIP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_RemoteServerIP.Name = "textBox_RemoteServerIP";
            this.textBox_RemoteServerIP.Size = new System.Drawing.Size(234, 27);
            this.textBox_RemoteServerIP.TabIndex = 4;
            this.textBox_RemoteServerIP.Text = "127.0.0.1";
            this.textBox_RemoteServerIP.TextChanged += new System.EventHandler(this.TextBox_RemoteServerIP_TextChanged);
            // 
            // comboBox_RemoteServerName
            // 
            this.comboBox_RemoteServerName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_RemoteServerName.FormattingEnabled = true;
            this.comboBox_RemoteServerName.Location = new System.Drawing.Point(76, 75);
            this.comboBox_RemoteServerName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBox_RemoteServerName.Name = "comboBox_RemoteServerName";
            this.comboBox_RemoteServerName.Size = new System.Drawing.Size(234, 28);
            this.comboBox_RemoteServerName.TabIndex = 3;
            this.comboBox_RemoteServerName.SelectedIndexChanged += new System.EventHandler(this.ComboBox_RemoteServerName_SelectedIndexChanged);
            // 
            // button_ServerEnum
            // 
            this.button_ServerEnum.Location = new System.Drawing.Point(318, 31);
            this.button_ServerEnum.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_ServerEnum.Name = "button_ServerEnum";
            this.button_ServerEnum.Size = new System.Drawing.Size(63, 29);
            this.button_ServerEnum.TabIndex = 2;
            this.button_ServerEnum.Text = "枚举";
            this.button_ServerEnum.UseVisualStyleBackColor = true;
            this.button_ServerEnum.Click += new System.EventHandler(this.Button_ServerEnum_Click);
            // 
            // btnConnLocalServer
            // 
            this.btnConnLocalServer.Location = new System.Drawing.Point(318, 74);
            this.btnConnLocalServer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnConnLocalServer.Name = "btnConnLocalServer";
            this.btnConnLocalServer.Size = new System.Drawing.Size(63, 31);
            this.btnConnLocalServer.TabIndex = 2;
            this.btnConnLocalServer.Text = "连接";
            this.btnConnLocalServer.UseVisualStyleBackColor = true;
            this.btnConnLocalServer.Click += new System.EventHandler(this.BtnConnLocalServer_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 78);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "服务器：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 36);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP地址:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // statusStrip_Opc
            // 
            this.statusStrip_Opc.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip_Opc.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel_ServerState,
            this.statusLabel_ServerStartTime,
            this.statusLabel_Version});
            this.statusStrip_Opc.Location = new System.Drawing.Point(0, 672);
            this.statusStrip_Opc.Name = "statusStrip_Opc";
            this.statusStrip_Opc.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip_Opc.Size = new System.Drawing.Size(1448, 26);
            this.statusStrip_Opc.TabIndex = 15;
            this.statusStrip_Opc.Text = "statusStrip1";
            // 
            // statusLabel_ServerState
            // 
            this.statusLabel_ServerState.Name = "statusLabel_ServerState";
            this.statusLabel_ServerState.Size = new System.Drawing.Size(84, 20);
            this.statusLabel_ServerState.Text = "ServerState";
            // 
            // statusLabel_ServerStartTime
            // 
            this.statusLabel_ServerStartTime.Name = "statusLabel_ServerStartTime";
            this.statusLabel_ServerStartTime.Size = new System.Drawing.Size(114, 20);
            this.statusLabel_ServerStartTime.Text = "ServerStartTime";
            // 
            // statusLabel_Version
            // 
            this.statusLabel_Version.Name = "statusLabel_Version";
            this.statusLabel_Version.Size = new System.Drawing.Size(57, 20);
            this.statusLabel_Version.Text = "Version";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numeric_GroupUpdateRate);
            this.groupBox2.Controls.Add(this.numeric_GroupsDeadband);
            this.groupBox2.Controls.Add(this.checkBox_IsGroupSubscribed);
            this.groupBox2.Controls.Add(this.checkBox_IsGroupActive);
            this.groupBox2.Controls.Add(this.checkBox_IsGroupsActive);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(15, 176);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(497, 71);
            this.groupBox2.TabIndex = 16;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "更改组属性";
            // 
            // numeric_GroupUpdateRate
            // 
            this.numeric_GroupUpdateRate.Location = new System.Drawing.Point(410, 28);
            this.numeric_GroupUpdateRate.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.numeric_GroupUpdateRate.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numeric_GroupUpdateRate.Name = "numeric_GroupUpdateRate";
            this.numeric_GroupUpdateRate.Size = new System.Drawing.Size(73, 27);
            this.numeric_GroupUpdateRate.TabIndex = 15;
            this.numeric_GroupUpdateRate.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
            this.numeric_GroupUpdateRate.ValueChanged += new System.EventHandler(this.Numeric_GroupUpdateRate_ValueChanged);
            // 
            // numeric_GroupsDeadband
            // 
            this.numeric_GroupsDeadband.Location = new System.Drawing.Point(287, 28);
            this.numeric_GroupsDeadband.Name = "numeric_GroupsDeadband";
            this.numeric_GroupsDeadband.Size = new System.Drawing.Size(73, 27);
            this.numeric_GroupsDeadband.TabIndex = 14;
            this.numeric_GroupsDeadband.ValueChanged += new System.EventHandler(this.Numeric_GroupsDeadband_ValueChanged);
            // 
            // checkBox_IsGroupSubscribed
            // 
            this.checkBox_IsGroupSubscribed.AutoSize = true;
            this.checkBox_IsGroupSubscribed.Checked = true;
            this.checkBox_IsGroupSubscribed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_IsGroupSubscribed.Location = new System.Drawing.Point(145, 32);
            this.checkBox_IsGroupSubscribed.Name = "checkBox_IsGroupSubscribed";
            this.checkBox_IsGroupSubscribed.Size = new System.Drawing.Size(61, 24);
            this.checkBox_IsGroupSubscribed.TabIndex = 13;
            this.checkBox_IsGroupSubscribed.Text = "订阅";
            this.checkBox_IsGroupSubscribed.UseVisualStyleBackColor = true;
            this.checkBox_IsGroupSubscribed.CheckedChanged += new System.EventHandler(this.CheckBox_IsGroupSubscribed_CheckedChanged);
            // 
            // checkBox_IsGroupActive
            // 
            this.checkBox_IsGroupActive.AutoSize = true;
            this.checkBox_IsGroupActive.Checked = true;
            this.checkBox_IsGroupActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_IsGroupActive.Location = new System.Drawing.Point(76, 32);
            this.checkBox_IsGroupActive.Name = "checkBox_IsGroupActive";
            this.checkBox_IsGroupActive.Size = new System.Drawing.Size(61, 24);
            this.checkBox_IsGroupActive.TabIndex = 12;
            this.checkBox_IsGroupActive.Text = "激活";
            this.checkBox_IsGroupActive.UseVisualStyleBackColor = true;
            this.checkBox_IsGroupActive.CheckedChanged += new System.EventHandler(this.CheckBox_IsGroupActive_CheckedChanged);
            // 
            // checkBox_IsGroupsActive
            // 
            this.checkBox_IsGroupsActive.AutoSize = true;
            this.checkBox_IsGroupsActive.Checked = true;
            this.checkBox_IsGroupsActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_IsGroupsActive.Location = new System.Drawing.Point(8, 32);
            this.checkBox_IsGroupsActive.Name = "checkBox_IsGroupsActive";
            this.checkBox_IsGroupsActive.Size = new System.Drawing.Size(61, 24);
            this.checkBox_IsGroupsActive.TabIndex = 11;
            this.checkBox_IsGroupsActive.Text = "活动";
            this.checkBox_IsGroupsActive.UseVisualStyleBackColor = true;
            this.checkBox_IsGroupsActive.CheckedChanged += new System.EventHandler(this.CheckBox_IsGroupsActive_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(367, 32);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(39, 20);
            this.label9.TabIndex = 4;
            this.label9.Text = "速度";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(213, 32);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 20);
            this.label6.TabIndex = 1;
            this.label6.Text = "不敏感区";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button_WriteTestItem);
            this.groupBox4.Controls.Add(this.button_AddTestItem);
            this.groupBox4.Controls.Add(this.textBox_TestValue);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.textBox_TestItemId);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Location = new System.Drawing.Point(15, 324);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(497, 139);
            this.groupBox4.TabIndex = 32;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "标签写入测试";
            // 
            // button_WriteTestItem
            // 
            this.button_WriteTestItem.Location = new System.Drawing.Point(415, 83);
            this.button_WriteTestItem.Name = "button_WriteTestItem";
            this.button_WriteTestItem.Size = new System.Drawing.Size(68, 27);
            this.button_WriteTestItem.TabIndex = 5;
            this.button_WriteTestItem.Text = "写入";
            this.button_WriteTestItem.UseVisualStyleBackColor = true;
            this.button_WriteTestItem.Click += new System.EventHandler(this.Button_WriteTestItem_Click);
            // 
            // button_AddTestItem
            // 
            this.button_AddTestItem.Location = new System.Drawing.Point(415, 41);
            this.button_AddTestItem.Name = "button_AddTestItem";
            this.button_AddTestItem.Size = new System.Drawing.Size(68, 28);
            this.button_AddTestItem.TabIndex = 4;
            this.button_AddTestItem.Text = "添加";
            this.button_AddTestItem.UseVisualStyleBackColor = true;
            this.button_AddTestItem.Click += new System.EventHandler(this.Button_AddTestItem_Click);
            // 
            // textBox_TestValue
            // 
            this.textBox_TestValue.Location = new System.Drawing.Point(69, 83);
            this.textBox_TestValue.Name = "textBox_TestValue";
            this.textBox_TestValue.Size = new System.Drawing.Size(339, 27);
            this.textBox_TestValue.TabIndex = 3;
            this.textBox_TestValue.Text = "100";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 87);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(39, 20);
            this.label15.TabIndex = 2;
            this.label15.Text = "值：";
            // 
            // textBox_TestItemId
            // 
            this.textBox_TestItemId.Location = new System.Drawing.Point(69, 41);
            this.textBox_TestItemId.Name = "textBox_TestItemId";
            this.textBox_TestItemId.Size = new System.Drawing.Size(339, 27);
            this.textBox_TestItemId.TabIndex = 1;
            this.textBox_TestItemId.Text = "[RADAR]PRODUCE_Radar1";
            this.textBox_TestItemId.TextChanged += new System.EventHandler(this.TextBox_TestItemId_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(4, 45);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(54, 20);
            this.label14.TabIndex = 0;
            this.label14.Text = "标签：";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label_OpcError);
            this.groupBox3.Location = new System.Drawing.Point(15, 255);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(497, 60);
            this.groupBox3.TabIndex = 33;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "标签";
            // 
            // label_OpcError
            // 
            this.label_OpcError.AutoSize = true;
            this.label_OpcError.ForeColor = System.Drawing.Color.Red;
            this.label_OpcError.Location = new System.Drawing.Point(22, 24);
            this.label_OpcError.Name = "label_OpcError";
            this.label_OpcError.Size = new System.Drawing.Size(70, 20);
            this.label_OpcError.TabIndex = 2;
            this.label_OpcError.Text = "OPC错误";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button_UiUpdate);
            this.groupBox5.Controls.Add(this.checkBox_UiUpdateRecur);
            this.groupBox5.Controls.Add(this.label_GpsQuality);
            this.groupBox5.Controls.Add(this.label_MessageError);
            this.groupBox5.Controls.Add(this.textBox_GpsQuality);
            this.groupBox5.Controls.Add(this.label20);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label_TrackDirection);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.textBox_BaselineLength);
            this.groupBox5.Controls.Add(this.textBox_MagDec);
            this.groupBox5.Controls.Add(this.textBox_AntePitch);
            this.groupBox5.Controls.Add(this.textBox_TrackDirection);
            this.groupBox5.Controls.Add(this.textBox_LatitudeValue);
            this.groupBox5.Controls.Add(this.textBox_Height);
            this.groupBox5.Controls.Add(this.textBox_LongitudeValue);
            this.groupBox5.Controls.Add(this.gpsallstr_txt);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Location = new System.Drawing.Point(521, 215);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(470, 396);
            this.groupBox5.TabIndex = 34;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "GPS采集";
            // 
            // button_UiUpdate
            // 
            this.button_UiUpdate.Location = new System.Drawing.Point(301, 23);
            this.button_UiUpdate.Name = "button_UiUpdate";
            this.button_UiUpdate.Size = new System.Drawing.Size(79, 32);
            this.button_UiUpdate.TabIndex = 18;
            this.button_UiUpdate.Text = "手动刷新";
            this.button_UiUpdate.UseVisualStyleBackColor = true;
            this.button_UiUpdate.Click += new System.EventHandler(this.Button_UiUpdate_Click);
            // 
            // checkBox_UiUpdateRecur
            // 
            this.checkBox_UiUpdateRecur.AutoSize = true;
            this.checkBox_UiUpdateRecur.Checked = true;
            this.checkBox_UiUpdateRecur.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_UiUpdateRecur.Location = new System.Drawing.Point(189, 28);
            this.checkBox_UiUpdateRecur.Name = "checkBox_UiUpdateRecur";
            this.checkBox_UiUpdateRecur.Size = new System.Drawing.Size(106, 24);
            this.checkBox_UiUpdateRecur.TabIndex = 17;
            this.checkBox_UiUpdateRecur.Text = "UI自动刷新";
            this.checkBox_UiUpdateRecur.UseVisualStyleBackColor = true;
            // 
            // textBox_PitchAngle
            // 
            this.textBox_PitchAngle.Location = new System.Drawing.Point(371, 27);
            this.textBox_PitchAngle.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_PitchAngle.Name = "textBox_PitchAngle";
            this.textBox_PitchAngle.Size = new System.Drawing.Size(112, 27);
            this.textBox_PitchAngle.TabIndex = 16;
            // 
            // label_GpsQuality
            // 
            this.label_GpsQuality.AutoSize = true;
            this.label_GpsQuality.Location = new System.Drawing.Point(17, 260);
            this.label_GpsQuality.Name = "label_GpsQuality";
            this.label_GpsQuality.Size = new System.Drawing.Size(39, 20);
            this.label_GpsQuality.TabIndex = 7;
            this.label_GpsQuality.Text = "质量";
            // 
            // label_MessageError
            // 
            this.label_MessageError.AutoSize = true;
            this.label_MessageError.ForeColor = System.Drawing.Color.Red;
            this.label_MessageError.Location = new System.Drawing.Point(100, 29);
            this.label_MessageError.Name = "label_MessageError";
            this.label_MessageError.Size = new System.Drawing.Size(69, 20);
            this.label_MessageError.TabIndex = 6;
            this.label_MessageError.Text = "报文错误";
            // 
            // textBox_ClaimerId
            // 
            this.textBox_ClaimerId.Location = new System.Drawing.Point(43, 26);
            this.textBox_ClaimerId.Name = "textBox_ClaimerId";
            this.textBox_ClaimerId.Size = new System.Drawing.Size(94, 27);
            this.textBox_ClaimerId.TabIndex = 5;
            this.textBox_ClaimerId.TextChanged += new System.EventHandler(this.TextBox_ClaimerId_TextChanged);
            // 
            // textBox_GpsQuality
            // 
            this.textBox_GpsQuality.Location = new System.Drawing.Point(83, 257);
            this.textBox_GpsQuality.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_GpsQuality.Name = "textBox_GpsQuality";
            this.textBox_GpsQuality.Size = new System.Drawing.Size(191, 27);
            this.textBox_GpsQuality.TabIndex = 15;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 29);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 20);
            this.label13.TabIndex = 4;
            this.label13.Text = "ID";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(326, 30);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(39, 20);
            this.label21.TabIndex = 3;
            this.label21.Text = "俯仰";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(18, 364);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(39, 20);
            this.label20.TabIndex = 3;
            this.label20.Text = "海拔";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(18, 330);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 20);
            this.label12.TabIndex = 3;
            this.label12.Text = "纬度/Y";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(296, 364);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 20);
            this.label10.TabIndex = 3;
            this.label10.Text = "磁偏角";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(157, 30);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(39, 20);
            this.label18.TabIndex = 3;
            this.label18.Text = "回转";
            // 
            // label_TrackDirection
            // 
            this.label_TrackDirection.AutoSize = true;
            this.label_TrackDirection.Location = new System.Drawing.Point(310, 260);
            this.label_TrackDirection.Name = "label_TrackDirection";
            this.label_TrackDirection.Size = new System.Drawing.Size(39, 20);
            this.label_TrackDirection.TabIndex = 3;
            this.label_TrackDirection.Text = "航向";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 295);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 20);
            this.label11.TabIndex = 3;
            this.label11.Text = "经度/X";
            // 
            // textBox_MagDec
            // 
            this.textBox_MagDec.Location = new System.Drawing.Point(355, 361);
            this.textBox_MagDec.Name = "textBox_MagDec";
            this.textBox_MagDec.Size = new System.Drawing.Size(109, 27);
            this.textBox_MagDec.TabIndex = 2;
            // 
            // textBox_TrackLocalNorth
            // 
            this.textBox_TrackLocalNorth.Location = new System.Drawing.Point(202, 26);
            this.textBox_TrackLocalNorth.Name = "textBox_TrackLocalNorth";
            this.textBox_TrackLocalNorth.Size = new System.Drawing.Size(118, 27);
            this.textBox_TrackLocalNorth.TabIndex = 2;
            // 
            // textBox_TrackDirection
            // 
            this.textBox_TrackDirection.Location = new System.Drawing.Point(355, 257);
            this.textBox_TrackDirection.Name = "textBox_TrackDirection";
            this.textBox_TrackDirection.Size = new System.Drawing.Size(109, 27);
            this.textBox_TrackDirection.TabIndex = 2;
            // 
            // textBox_LatitudeValue
            // 
            this.textBox_LatitudeValue.Location = new System.Drawing.Point(83, 327);
            this.textBox_LatitudeValue.Name = "textBox_LatitudeValue";
            this.textBox_LatitudeValue.Size = new System.Drawing.Size(191, 27);
            this.textBox_LatitudeValue.TabIndex = 2;
            // 
            // textBox_Height
            // 
            this.textBox_Height.Location = new System.Drawing.Point(82, 362);
            this.textBox_Height.Name = "textBox_Height";
            this.textBox_Height.Size = new System.Drawing.Size(192, 27);
            this.textBox_Height.TabIndex = 2;
            // 
            // textBox_LongitudeValue
            // 
            this.textBox_LongitudeValue.Location = new System.Drawing.Point(83, 292);
            this.textBox_LongitudeValue.Name = "textBox_LongitudeValue";
            this.textBox_LongitudeValue.Size = new System.Drawing.Size(191, 27);
            this.textBox_LongitudeValue.TabIndex = 2;
            // 
            // gpsallstr_txt
            // 
            this.gpsallstr_txt.Location = new System.Drawing.Point(21, 53);
            this.gpsallstr_txt.Multiline = true;
            this.gpsallstr_txt.Name = "gpsallstr_txt";
            this.gpsallstr_txt.Size = new System.Drawing.Size(442, 195);
            this.gpsallstr_txt.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(18, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "原始报文";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.comboBox_ConnMode);
            this.groupBox6.Controls.Add(this.label_ReconnCounter);
            this.groupBox6.Controls.Add(this.checkBox_AutoCollect);
            this.groupBox6.Controls.Add(this.LblTcpState);
            this.groupBox6.Controls.Add(this.label16);
            this.groupBox6.Controls.Add(this.textBox_Command);
            this.groupBox6.Controls.Add(this.textBox_Port);
            this.groupBox6.Controls.Add(this.textBox_IpAddress);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.button_Connect);
            this.groupBox6.Controls.Add(this.button_StartCollect);
            this.groupBox6.Controls.Add(this.button_Disconnect);
            this.groupBox6.Location = new System.Drawing.Point(521, 16);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox6.Size = new System.Drawing.Size(469, 191);
            this.groupBox6.TabIndex = 37;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "连接GPS设备";
            // 
            // comboBox_ConnMode
            // 
            this.comboBox_ConnMode.DisplayMember = "MODE_NAME";
            this.comboBox_ConnMode.FormattingEnabled = true;
            this.comboBox_ConnMode.Items.AddRange(new object[] {
            "TCP",
            "TCPS",
            "HTTP"});
            this.comboBox_ConnMode.Location = new System.Drawing.Point(258, 28);
            this.comboBox_ConnMode.Name = "comboBox_ConnMode";
            this.comboBox_ConnMode.Size = new System.Drawing.Size(91, 28);
            this.comboBox_ConnMode.TabIndex = 19;
            this.comboBox_ConnMode.Text = "TCP";
            this.comboBox_ConnMode.ValueMember = "MODE_ID";
            this.comboBox_ConnMode.SelectedIndexChanged += new System.EventHandler(this.ComboBox_ConnMode_SelectedIndexChanged);
            // 
            // label_ReconnCounter
            // 
            this.label_ReconnCounter.AutoSize = true;
            this.label_ReconnCounter.Location = new System.Drawing.Point(313, 115);
            this.label_ReconnCounter.Name = "label_ReconnCounter";
            this.label_ReconnCounter.Size = new System.Drawing.Size(51, 20);
            this.label_ReconnCounter.TabIndex = 18;
            this.label_ReconnCounter.Text = "recon";
            // 
            // checkBox_AutoCollect
            // 
            this.checkBox_AutoCollect.AutoSize = true;
            this.checkBox_AutoCollect.Location = new System.Drawing.Point(258, 73);
            this.checkBox_AutoCollect.Name = "checkBox_AutoCollect";
            this.checkBox_AutoCollect.Size = new System.Drawing.Size(91, 24);
            this.checkBox_AutoCollect.TabIndex = 17;
            this.checkBox_AutoCollect.Text = "自动采集";
            this.checkBox_AutoCollect.UseVisualStyleBackColor = true;
            this.checkBox_AutoCollect.CheckedChanged += new System.EventHandler(this.CheckBox_AutoCollect_CheckedChanged);
            // 
            // LblTcpState
            // 
            this.LblTcpState.AutoSize = true;
            this.LblTcpState.Location = new System.Drawing.Point(16, 117);
            this.LblTcpState.Name = "LblTcpState";
            this.LblTcpState.Size = new System.Drawing.Size(84, 20);
            this.LblTcpState.TabIndex = 16;
            this.LblTcpState.Text = "连接状态：";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(18, 31);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(22, 20);
            this.label16.TabIndex = 13;
            this.label16.Text = "IP";
            // 
            // textBox_Command
            // 
            this.textBox_Command.Location = new System.Drawing.Point(20, 151);
            this.textBox_Command.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_Command.Name = "textBox_Command";
            this.textBox_Command.Size = new System.Drawing.Size(443, 27);
            this.textBox_Command.TabIndex = 15;
            this.textBox_Command.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_Command_KeyDown);
            this.textBox_Command.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBox_Command_KeyPress);
            // 
            // textBox_Port
            // 
            this.textBox_Port.Location = new System.Drawing.Point(67, 71);
            this.textBox_Port.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(87, 27);
            this.textBox_Port.TabIndex = 15;
            this.textBox_Port.Text = "8888";
            this.textBox_Port.TextChanged += new System.EventHandler(this.TextBox_Port_TextChanged);
            // 
            // textBox_IpAddress
            // 
            this.textBox_IpAddress.Location = new System.Drawing.Point(67, 28);
            this.textBox_IpAddress.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox_IpAddress.Name = "textBox_IpAddress";
            this.textBox_IpAddress.Size = new System.Drawing.Size(167, 27);
            this.textBox_IpAddress.TabIndex = 11;
            this.textBox_IpAddress.Text = "192.168.1.100";
            this.textBox_IpAddress.TextChanged += new System.EventHandler(this.TextBox_IpAddress_TextChanged);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(16, 74);
            this.label17.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(39, 20);
            this.label17.TabIndex = 14;
            this.label17.Text = "端口";
            // 
            // button_Connect
            // 
            this.button_Connect.Location = new System.Drawing.Point(371, 26);
            this.button_Connect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Connect.Name = "button_Connect";
            this.button_Connect.Size = new System.Drawing.Size(92, 32);
            this.button_Connect.TabIndex = 12;
            this.button_Connect.Text = "启动";
            this.button_Connect.UseVisualStyleBackColor = true;
            this.button_Connect.Click += new System.EventHandler(this.Button_Connect_Click);
            // 
            // button_StartCollect
            // 
            this.button_StartCollect.Location = new System.Drawing.Point(371, 109);
            this.button_StartCollect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_StartCollect.Name = "button_StartCollect";
            this.button_StartCollect.Size = new System.Drawing.Size(92, 32);
            this.button_StartCollect.TabIndex = 12;
            this.button_StartCollect.Text = "开始采集";
            this.button_StartCollect.UseVisualStyleBackColor = true;
            this.button_StartCollect.Click += new System.EventHandler(this.Button_StartCollect_Click);
            // 
            // button_Disconnect
            // 
            this.button_Disconnect.Location = new System.Drawing.Point(371, 67);
            this.button_Disconnect.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button_Disconnect.Name = "button_Disconnect";
            this.button_Disconnect.Size = new System.Drawing.Size(92, 31);
            this.button_Disconnect.TabIndex = 12;
            this.button_Disconnect.Text = "停止";
            this.button_Disconnect.UseVisualStyleBackColor = true;
            this.button_Disconnect.Click += new System.EventHandler(this.Button_Disconnect_Click);
            // 
            // timer_UiUpdate
            // 
            this.timer_UiUpdate.Tick += new System.EventHandler(this.Timer_UiUpdate_Tick);
            // 
            // statusStrip_WebService
            // 
            this.statusStrip_WebService.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip_WebService.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel_WebService});
            this.statusStrip_WebService.Location = new System.Drawing.Point(0, 646);
            this.statusStrip_WebService.Name = "statusStrip_WebService";
            this.statusStrip_WebService.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip_WebService.Size = new System.Drawing.Size(1448, 26);
            this.statusStrip_WebService.TabIndex = 38;
            this.statusStrip_WebService.Text = "statusStrip2";
            // 
            // statusLabel_WebService
            // 
            this.statusLabel_WebService.ForeColor = System.Drawing.Color.Red;
            this.statusLabel_WebService.Name = "statusLabel_WebService";
            this.statusLabel_WebService.Size = new System.Drawing.Size(103, 20);
            this.statusLabel_WebService.Text = "Web服务错误";
            // 
            // timer_Reconn
            // 
            this.timer_Reconn.Interval = 5000;
            this.timer_Reconn.Tick += new System.EventHandler(this.Timer_Reconn_Tick);
            // 
            // timer_OpcUpdate
            // 
            this.timer_OpcUpdate.Interval = 1000;
            this.timer_OpcUpdate.Tick += new System.EventHandler(this.Timer_OpcUpdate_Tick);
            // 
            // timer_Upload
            // 
            this.timer_Upload.Interval = 1000;
            this.timer_Upload.Tick += new System.EventHandler(this.Timer_Upload_Tick);
            // 
            // button_Expand
            // 
            this.button_Expand.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button_Expand.Location = new System.Drawing.Point(1430, 279);
            this.button_Expand.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_Expand.Name = "button_Expand";
            this.button_Expand.Size = new System.Drawing.Size(18, 68);
            this.button_Expand.TabIndex = 40;
            this.button_Expand.Text = "▶";
            this.button_Expand.UseVisualStyleBackColor = true;
            this.button_Expand.Click += new System.EventHandler(this.Button_Expand_Click);
            // 
            // textBox_Info
            // 
            this.textBox_Info.Location = new System.Drawing.Point(1015, 16);
            this.textBox_Info.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBox_Info.Multiline = true;
            this.textBox_Info.Name = "textBox_Info";
            this.textBox_Info.Size = new System.Drawing.Size(410, 609);
            this.textBox_Info.TabIndex = 41;
            this.textBox_Info.Visible = false;
            // 
            // button_CopyToClipboard
            // 
            this.button_CopyToClipboard.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_CopyToClipboard.Location = new System.Drawing.Point(1342, 16);
            this.button_CopyToClipboard.Name = "button_CopyToClipboard";
            this.button_CopyToClipboard.Size = new System.Drawing.Size(83, 36);
            this.button_CopyToClipboard.TabIndex = 42;
            this.button_CopyToClipboard.Text = "复制";
            this.button_CopyToClipboard.UseVisualStyleBackColor = true;
            this.button_CopyToClipboard.Click += new System.EventHandler(this.Button_CopyToClipboard_Click);
            // 
            // label_Copied
            // 
            this.label_Copied.AutoSize = true;
            this.label_Copied.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label_Copied.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label_Copied.Font = new System.Drawing.Font("黑体", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_Copied.Location = new System.Drawing.Point(1338, 65);
            this.label_Copied.Name = "label_Copied";
            this.label_Copied.Size = new System.Drawing.Size(85, 24);
            this.label_Copied.TabIndex = 43;
            this.label_Copied.Text = "已复制";
            this.label_Copied.Visible = false;
            // 
            // button_RecordCoors
            // 
            this.button_RecordCoors.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button_RecordCoors.Location = new System.Drawing.Point(1235, 16);
            this.button_RecordCoors.Name = "button_RecordCoors";
            this.button_RecordCoors.Size = new System.Drawing.Size(83, 36);
            this.button_RecordCoors.TabIndex = 44;
            this.button_RecordCoors.Text = "记录开";
            this.button_RecordCoors.UseVisualStyleBackColor = true;
            this.button_RecordCoors.Click += new System.EventHandler(this.Button_RecordCoors_Click);
            // 
            // timer_RecordCoor
            // 
            this.timer_RecordCoor.Interval = 500;
            this.timer_RecordCoor.Tick += new System.EventHandler(this.Timer_RecordCoor_Tick);
            // 
            // statusLabel_DataService
            // 
            this.statusLabel_DataService.ForeColor = System.Drawing.Color.Red;
            this.statusLabel_DataService.Name = "statusLabel_DataService";
            this.statusLabel_DataService.Size = new System.Drawing.Size(121, 20);
            this.statusLabel_DataService.Text = "数据库操作错误";
            // 
            // statusStrip_DataService
            // 
            this.statusStrip_DataService.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip_DataService.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel_DataService});
            this.statusStrip_DataService.Location = new System.Drawing.Point(0, 620);
            this.statusStrip_DataService.Name = "statusStrip_DataService";
            this.statusStrip_DataService.Padding = new System.Windows.Forms.Padding(1, 0, 21, 0);
            this.statusStrip_DataService.Size = new System.Drawing.Size(1448, 26);
            this.statusStrip_DataService.TabIndex = 39;
            this.statusStrip_DataService.Text = "statusStrip2";
            // 
            // tcpServerMain
            // 
            this.tcpServerMain.CheckTime = 1000;
            this.tcpServerMain.HeartBeatCheck = null;
            this.tcpServerMain.HeartBeatPacket = "X";
            this.tcpServerMain.IsHeartCheck = true;
            this.tcpServerMain.IsStartListening = false;
            this.tcpServerMain.LocalEndPoint = null;
            this.tcpServerMain.RemoteEndPoint = null;
            this.tcpServerMain.ServerIp = "127.0.0.1";
            this.tcpServerMain.ServerPort = 25002;
            this.tcpServerMain.ServerSocket = null;
            this.tcpServerMain.StartSockst = null;
            // 
            // tcpClient
            // 
            this.tcpClient.BaseClient = null;
            this.tcpClient.IsReconnection = false;
            this.tcpClient.IsStart = false;
            this.tcpClient.IsStartTcpThreading = false;
            this.tcpClient.LocalEndPoint = null;
            this.tcpClient.LocalIp = null;
            this.tcpClient.ReceiveBufferSize = 2048;
            this.tcpClient.ReConnectedCount = 0;
            this.tcpClient.ReconnectWhenReceiveNone = true;
            this.tcpClient.RemoteEndPoint = null;
            this.tcpClient.ServerIp = null;
            this.tcpClient.ServerPort = 25002;
            this.tcpClient.TcpThread = null;
            this.tcpClient.OnReceive += new SocketHelper.SocketTcpClient.ReceivedEventHandler(this.TcpClient_OnReceive);
            this.tcpClient.OnStateInfo += new SocketHelper.SocketTcpClient.StateInfoEventHandler(this.TcpClient_OnStateInfo);
            // 
            // tcpServerConn
            // 
            this.tcpServerConn.CheckTime = 1000;
            this.tcpServerConn.HeartBeatCheck = null;
            this.tcpServerConn.HeartBeatPacket = "X";
            this.tcpServerConn.IsHeartCheck = false;
            this.tcpServerConn.IsStartListening = false;
            this.tcpServerConn.LocalEndPoint = null;
            this.tcpServerConn.RemoteEndPoint = null;
            this.tcpServerConn.ServerIp = "127.0.0.1";
            this.tcpServerConn.ServerPort = 32333;
            this.tcpServerConn.ServerSocket = null;
            this.tcpServerConn.StartSockst = null;
            this.tcpServerConn.Received += new SocketHelper.SocketTcpServer.ReceivedEventHandler(this.TcpServerConn_Received);
            this.tcpServerConn.OnStateInfo += new SocketHelper.SocketTcpServer.StateInfoEventHandler(this.TcpServerConn_OnStateInfo);
            this.tcpServerConn.OnClientOnline += new SocketHelper.SocketTcpServer.ClientOnlineEventHandler(this.TcpServerConn_OnClientOnline);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.textBox_ClaimerId);
            this.groupBox7.Controls.Add(this.label13);
            this.groupBox7.Controls.Add(this.textBox_PitchAngle);
            this.groupBox7.Controls.Add(this.textBox_TrackLocalNorth);
            this.groupBox7.Controls.Add(this.label18);
            this.groupBox7.Controls.Add(this.label21);
            this.groupBox7.Location = new System.Drawing.Point(15, 469);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(497, 142);
            this.groupBox7.TabIndex = 33;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "单机信息";
            // 
            // textBox_AntePitch
            // 
            this.textBox_AntePitch.Location = new System.Drawing.Point(355, 292);
            this.textBox_AntePitch.Name = "textBox_AntePitch";
            this.textBox_AntePitch.Size = new System.Drawing.Size(109, 27);
            this.textBox_AntePitch.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(280, 295);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "天线俯仰";
            // 
            // textBox_BaselineLength
            // 
            this.textBox_BaselineLength.Location = new System.Drawing.Point(355, 327);
            this.textBox_BaselineLength.Name = "textBox_BaselineLength";
            this.textBox_BaselineLength.Size = new System.Drawing.Size(109, 27);
            this.textBox_BaselineLength.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(280, 330);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "基线长度";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1448, 698);
            this.Controls.Add(this.button_RecordCoors);
            this.Controls.Add(this.label_Copied);
            this.Controls.Add(this.button_CopyToClipboard);
            this.Controls.Add(this.button_Expand);
            this.Controls.Add(this.textBox_Info);
            this.Controls.Add(this.statusStrip_DataService);
            this.Controls.Add(this.statusStrip_WebService);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.statusStrip_Opc);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "大机坐标采集程序";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip_Opc.ResumeLayout(false);
            this.statusStrip_Opc.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_GroupUpdateRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numeric_GroupsDeadband)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.statusStrip_WebService.ResumeLayout(false);
            this.statusStrip_WebService.PerformLayout();
            this.statusStrip_DataService.ResumeLayout(false);
            this.statusStrip_DataService.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.MaskedTextBox textBox_RemoteServerIP;
        private System.Windows.Forms.ComboBox comboBox_RemoteServerName;
        private System.Windows.Forms.Button button_ServerEnum;
        private System.Windows.Forms.Button btnConnLocalServer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button_WriteTestItem;
        private System.Windows.Forms.Button button_AddTestItem;
        private System.Windows.Forms.TextBox textBox_TestValue;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBox_TestItemId;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox gpsallstr_txt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox_LatitudeValue;
        private System.Windows.Forms.TextBox textBox_LongitudeValue;
        private System.Windows.Forms.StatusStrip statusStrip_Opc;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel_ServerState;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel_ServerStartTime;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel_Version;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBox_Port;
        private System.Windows.Forms.TextBox textBox_IpAddress;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button button_Connect;
        private System.Windows.Forms.Button button_Disconnect;
        private System.Windows.Forms.Label LblTcpState;
        private System.Windows.Forms.TextBox textBox_ClaimerId;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button button_StartCollect;
        private System.Windows.Forms.CheckBox checkBox_OpcAutoConnect;
        private System.Windows.Forms.CheckBox checkBox_AutoCollect;
        private System.Windows.Forms.CheckBox checkBox_IsGroupsActive;
        private System.Windows.Forms.CheckBox checkBox_IsGroupSubscribed;
        private System.Windows.Forms.CheckBox checkBox_IsGroupActive;
        private System.Windows.Forms.NumericUpDown numeric_GroupUpdateRate;
        private System.Windows.Forms.NumericUpDown numeric_GroupsDeadband;
        private System.Windows.Forms.Label label_MessageError;
        private System.Windows.Forms.Timer timer_UiUpdate;
        private System.Windows.Forms.Label label_OpcError;
        private System.Windows.Forms.StatusStrip statusStrip_WebService;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel_WebService;
        private System.Windows.Forms.Label label_GpsQuality;
        private System.Windows.Forms.TextBox textBox_GpsQuality;
        private System.Windows.Forms.Label label_OpcInfo;
        private System.Windows.Forms.Timer timer_Reconn;
        private System.Windows.Forms.Label label_TrackDirection;
        private System.Windows.Forms.TextBox textBox_TrackDirection;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBox_MagDec;
        private System.Windows.Forms.TextBox textBox_Command;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBox_Height;
        private System.Windows.Forms.TextBox textBox_PitchAngle;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Timer timer_OpcUpdate;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox textBox_TrackLocalNorth;
        private SocketHelper.SocketTcpServer tcpServerMain;
        private System.Windows.Forms.Timer timer_Upload;
        private System.Windows.Forms.Button button_Expand;
        private System.Windows.Forms.TextBox textBox_Info;
        private System.Windows.Forms.Button button_CopyToClipboard;
        private System.Windows.Forms.Label label_Copied;
        private SocketHelper.SocketTcpClient tcpClient;
        private System.Windows.Forms.Label label_ReconnCounter;
        private System.Windows.Forms.Button button_RecordCoors;
        private System.Windows.Forms.Timer timer_RecordCoor;
        private System.Windows.Forms.Button button_UiUpdate;
        private System.Windows.Forms.CheckBox checkBox_UiUpdateRecur;
        private System.Windows.Forms.ComboBox comboBox_ConnMode;
        private SocketHelper.SocketTcpServer tcpServerConn;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel_DataService;
        private System.Windows.Forms.StatusStrip statusStrip_DataService;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox_AntePitch;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_BaselineLength;
    }
}

