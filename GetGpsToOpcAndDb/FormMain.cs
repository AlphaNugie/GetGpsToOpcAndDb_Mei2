using CarServer;
using CommonLib.Clients;
using CommonLib.Enums;
using CommonLib.Function;
using GetGpsToOpcAndDb.Core;
using GetGpsToOpcAndDb.Model;
using OPCAutomation;
using OpcLibrary;
using SocketHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using YsuSoftHelper;
using YsuSoftHelper.ICommond;
//using YsuSoftHelper.TClass;

namespace GetGpsToOpcAndDb
{
    public partial class FormMain : Form
    {
        public delegate void OnReceviceCallBack(string msg, byte[] data); //TCP接收事件委托

        #region 私有变量
        private const string OPCGROUP_NAME_READ = "OpcGroupRead", OPCGROUP_NAME_WRITE = "OpcGroupWrite";
        private string remoteServerName = string.Empty; //OPC SERVER名称
        private readonly ClientType clientModel = ClientType.None;
        private readonly CommandStorage commandStorage = new CommandStorage();
        private const int LONGITUDE_WRITE_HANDLE = 101, LATITUDE_WRITE_HANDLE = 102, ALTITUDE_WRITE_HANDLE = 103, PITCH_WRITE_HANDLE = 106, PITCH_READ_HANDLE = 104, YAW_WRITE_HANDLE = 105, RANDOM_WRITE_HANDLE = 107; //经度，纬度，海拔，俯仰，回转角等OPC项的客户端句柄
        #endregion

        /// <summary>
        /// GPS信息实体类对象
        /// </summary>
        public GnssInfoObject GnssInfo { get; set; }

        /// <summary>
        /// 构造器
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.UnhandledException_Raising); //未捕获异常触发事件
            this.GnssInfo = new GnssInfoObject() { ClaimerId = this.textBox_ClaimerId.Text };
            this.InitControls();

            //TODO 配置服务地址
            //利用WebService构造器的重载方法，在Config.ini文件中修改，重载方法的endpointConfigurationName参数：App.config文件中的system.ServiceModel=>client=>endpoint节点的name属性值
        }

        /// <summary>
        /// 控件初始化
        /// </summary>
        private void InitControls()
        {
            this.timer_OpcUpdate.Interval = BaseConst.OpcHelper.OpcUpdateRate;
            this.timer_UiUpdate.Start();

            //TCP
            this.textBox_IpAddress.Text = BaseConst.IniHelper.ReadData("Tcp", "IpAddress");
            this.textBox_Port.Text = BaseConst.IniHelper.ReadData("Tcp", "Port");
            this.checkBox_AutoCollect.Checked = BaseConst.IniHelper.ReadData("Tcp", "AutoCollect").Equals("1");

            //OPC
            this.textBox_RemoteServerIP.Text = BaseConst.IniHelper.ReadData("Opc", "OpcServerIp");
            this.remoteServerName = BaseConst.IniHelper.ReadData("Opc", "OpcServerName");
            this.checkBox_OpcAutoConnect.Checked = BaseConst.IniHelper.ReadData("Opc", "OpcAutoConnect").Equals("1");

            this.checkBox_IsGroupsActive.Checked = BaseConst.OpcHelper.IsGroupsActive;
            this.numeric_GroupsDeadband.Value = (decimal)BaseConst.OpcHelper.GroupsDeadband;
            this.checkBox_IsGroupActive.Checked = BaseConst.OpcHelper.IsGroupActive;
            this.checkBox_IsGroupSubscribed.Checked = BaseConst.OpcHelper.IsGroupSubscribed;
            this.numeric_GroupUpdateRate.Value = (decimal)BaseConst.OpcHelper.GroupUpdateRate;

            this.textBox_LongitudeItemId.Text = BaseConst.IniHelper.ReadData("Opc", "LongitudeItemId");
            this.textBox_LatitudeItemId.Text = BaseConst.IniHelper.ReadData("Opc", "LatitudeItemId");
            this.textBox_AltitudeItemId.Text = BaseConst.IniHelper.ReadData("Opc", "AltitudeItemId");
            this.textBox_PitchItemId.Text = BaseConst.IniHelper.ReadData("Opc", "PitchItemId");
            this.textBox_TestItemId.Text = BaseConst.IniHelper.ReadData("Opc", "TestItemId");

            this.textBox_ClaimerId.Text = BaseConst.IniHelper.ReadData("Main", "ClaimerId"); //大机ID
        }

        /// <summary>
        /// 加载
        /// </summary>
        private void Loading()
        {
            statusLabel_ServerState.Text = string.Empty;
            statusLabel_ServerStartTime.Text = string.Empty;
            statusLabel_Version.Text = string.Empty;
            //OPC自动连接
            if (this.checkBox_OpcAutoConnect.Checked)
                //this.ConnectRemoteServer_Init();
                this.ConnectSlashDisconnect(true);
            //自动采集则TCP自动连接，连接后在状态改变事件内自动发送采集消息
            if (this.checkBox_AutoCollect.Checked)
                this.TcpConnect();
        }

        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Loading();
        }

        /// <summary>
        /// 页面临关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ysuTcpClient1.IsStart)
                ysuTcpClient1.StopConnect();
            this.tcpServerMain.Stop();
            this.timer_OpcUpdate.Stop();
            this.timer_Reconn.Stop();
            this.timer_UiUpdate.Stop();

            BaseConst.OpcHelper.DisconnectRemoteServer(); //断开OPC服务
        }

        /// <summary>
        /// 未捕获异常触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void UnhandledException_Raising(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            FileClient.WriteExceptionInfo(e, "未处理异常被触发，运行时是否即将终止：" + args.IsTerminating, true);
        }

        #region 功能
        #region OPC方法
        /// <summary>
        /// 枚举OPC SERVER
        /// </summary>
        private void ServerEnum()
        {
            string ipAddress = this.textBox_RemoteServerIP.Text, message;
            this.comboBox_RemoteServerName.Items.Clear(); //清空已显示的OPC Server列表
            string[] array = BaseConst.OpcHelper.ServerEnum(ipAddress, out message);
            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //假如Server列表为空，退出方法，否则为ListBoxControl添加Item
            if (array.Length == 0)
                return;
            this.comboBox_RemoteServerName.Items.AddRange(array);
            this.comboBox_RemoteServerName.SelectedIndex = 0;
        }

        /// <summary>
        /// 获取服务器信息，并显示在窗体状态栏上
        /// </summary>
        private void GetServerInfo()
        {
            this.statusLabel_ServerStartTime.Text = BaseConst.OpcHelper.ServerStartTime;
            this.statusLabel_Version.Text = BaseConst.OpcHelper.ServerVersion;
            this.statusLabel_ServerState.Text = BaseConst.OpcHelper.ServerState;
        }

        /// <summary>
        /// 连接或断开
        /// </summary>
        /// <param name="connecting">是否要连接</param>
        private void ConnectSlashDisconnect(bool connecting)
        {
            //new Thread(new ThreadStart(() =>
            //{
            //    if (connecting) this.ConnectRemoteServer_Init();
            //    else BaseConst.OpcHelper.DisconnectRemoteServer();
            //})).Start();
            if (connecting) this.ConnectRemoteServer_Init();
            else BaseConst.OpcHelper.DisconnectRemoteServer();
            this.btnConnLocalServer.Text = connecting ? "断开" : "连接";
        }

        /// <summary>
        /// 连接OPC SERVER并创建组
        /// </summary>
        private bool ConnectRemoteServer_Init()
        {
            try
            {
                string message = string.Empty, ip = this.textBox_RemoteServerIP.Text;
                bool result = false;
                new Thread(new ThreadStart(() =>
                {
                    result = BaseConst.OpcHelper.ConnectRemoteServer(ip, this.remoteServerName, out message);
                })).Start();
                while (true)
                {
                    if (result || !string.IsNullOrWhiteSpace(message))
                        break;
                    Thread.Sleep(1);
                }
                if (!result)
                {
                    MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                //string message;
                //if (!BaseConst.OpcHelper.ConnectRemoteServer(this.textBox_RemoteServerIP.Text, this.remoteServerName, out message))
                //{
                //    MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    return false;
                //}

                BaseConst.IniHelper.WriteData("Opc", "OpcServerName", remoteServerName); //保存OPC服务名称
                //TODO 连接OPC服务后启动重连TIMER
                //this.timer_Reconn.Start();
                this.GetServerInfo();
                List<OpcGroupInfo> list = new List<OpcGroupInfo>() {
                    new OpcGroupInfo(null, OPCGROUP_NAME_WRITE) {
                        //TODO 是否添加回转标签（yaw_write_handle）
                        //ListItemInfo = new List<OpcItemInfo>() { new OpcItemInfo(this.GnssInfo.LongitudeItemId, LONGITUDE_WRITE_HANDLE), new OpcItemInfo(this.GnssInfo.LatitudeItemId, LATITUDE_WRITE_HANDLE), new OpcItemInfo(this.GnssInfo.AltitudeItemId, ALTITUDE_WRITE_HANDLE), new OpcItemInfo("[top]GPS[4]", YAW_WRITE_HANDLE) }
                        ListItemInfo = new List<OpcItemInfo>() { new OpcItemInfo(this.GnssInfo.LongitudeItemId, LONGITUDE_WRITE_HANDLE), new OpcItemInfo(this.GnssInfo.LatitudeItemId, LATITUDE_WRITE_HANDLE), new OpcItemInfo(this.GnssInfo.AltitudeItemId, ALTITUDE_WRITE_HANDLE), new OpcItemInfo("[SL7_COLL]ANTICOLL_SYS.Spare_Int[1]", RANDOM_WRITE_HANDLE) }
                    },
                    new OpcGroupInfo(null, OPCGROUP_NAME_READ) {
                        ListItemInfo = new List<OpcItemInfo>() { new OpcItemInfo(this.GnssInfo.PitchItemId, PITCH_READ_HANDLE) }
                    }
                };
                //TODO 假如俯仰角获取模式为天线高度，向PLC写入俯仰角
                if (BaseConst.PitchAngleMode == PitchAngleMode.AntennaHeight)
                    list.Find(group => group.GroupName.Equals(OPCGROUP_NAME_WRITE)).ListItemInfo.Add(new OpcItemInfo(this.GnssInfo.PitchItemId, PITCH_WRITE_HANDLE));
                BaseConst.OpcHelper.ListGroupInfo = list;
                //if (!BaseConst.OpcHelper.CreateGroups(list, out message))
                //    MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                if (!BaseConst.OpcHelper.CreateGroups(BaseConst.OpcHelper.ListGroupInfo, out message))
                    MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {
                    BaseConst.OpcRead = BaseConst.OpcHelper.ListGroupInfo.Find(g => g.GroupName.Equals(OPCGROUP_NAME_READ));
                    BaseConst.OpcWrite = BaseConst.OpcHelper.ListGroupInfo.Find(g => g.GroupName.Equals(OPCGROUP_NAME_WRITE));
                    if (BaseConst.OpcRead != null)
                        BaseConst.OpcReadPitch = BaseConst.OpcRead.ListItemInfo.Find(i => i.ClientHandle == PITCH_READ_HANDLE);
                    if (BaseConst.OpcWrite != null)
                    {
                        BaseConst.OpcWriteLongitude = BaseConst.OpcWrite.ListItemInfo.Find(i => i.ClientHandle == LONGITUDE_WRITE_HANDLE);
                        BaseConst.OpcWriteLatitude = BaseConst.OpcWrite.ListItemInfo.Find(i => i.ClientHandle == LATITUDE_WRITE_HANDLE);
                        BaseConst.OpcWriteAltitude = BaseConst.OpcWrite.ListItemInfo.Find(i => i.ClientHandle == ALTITUDE_WRITE_HANDLE);
                        BaseConst.OpcWriteYaw = BaseConst.OpcWrite.ListItemInfo.Find(i => i.ClientHandle == YAW_WRITE_HANDLE);
                        BaseConst.OpcWritePitch = BaseConst.OpcWrite.ListItemInfo.Find(i => i.ClientHandle == PITCH_WRITE_HANDLE);
                        BaseConst.OpcWriteRandom = BaseConst.OpcWrite.ListItemInfo.Find(i => i.ClientHandle == RANDOM_WRITE_HANDLE);
                    }
                }
                this.timer_OpcUpdate.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show("初始化出错：" + err.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        ///// <summary>
        ///// 重新连接OPC
        ///// </summary>
        //private void Reconn()
        //{
        //    string info = string.Empty;
        //    try
        //    {
        //        if (BaseConst.OpcHelper.OpcServer.ServerState != (int)OPCServerState.OPCRunning)
        //            this.ReconnDetail(out info);
        //    }
        //    //假如捕捉到COMException
        //    catch (COMException)
        //    {
        //        this.ReconnDetail(out info);
        //    }
        //    this.label_OpcInfo.Text = info;
        //}

        ///// <summary>
        ///// 重新连接OPC
        ///// </summary>
        ///// <param name="info">返回信息</param>
        //private void ReconnDetail(out string info)
        //{
        //    info = string.Empty;
        //    try
        //    {
        //        string hostIp = this.textBox_RemoteServerIP.Text;
        //        BaseConst.OpcHelper.OpcServer = new OPCServer();
        //        info = string.Format("Opc server {0} at {1} has failed, trying reconnecting", this.remoteServerName, hostIp);
        //        BaseConst.OpcHelper.OpcServer.Connect(this.remoteServerName, hostIp);
        //        info = string.Format("Reconnecting opc server {0} at {1} succeeded.", this.remoteServerName, hostIp);
        //        BaseConst.OpcHelper.OpcServer.OPCGroups.RemoveAll();
        //        if (BaseConst.OpcHelper.CreateDefaultGroup(out info))
        //            info = string.Format("OpcGroup of opcServer {0} at {1} created alright", this.remoteServerName, hostIp);
        //    }
        //    catch (Exception) { }
        //}
        #endregion

        #region TCPCLIENT
        /// <summary>
        /// TCP连接
        /// </summary>
        private void TcpConnect()
        {
            try
            {
                string state = button_Connect.Text;
                string ipStr = textBox_IpAddress.Text;
                IPAddress ip;
                int tmp;
                if (IPAddress.TryParse(ipStr, out ip) && int.TryParse(textBox_Port.Text, out tmp))
                {
                    ysuTcpClient1.ServerIp = ipStr;
                    ysuTcpClient1.ServerPort = tmp;
                    ysuTcpClient1.StartConnect();
                    this.tcpServerMain.Start();
                    this.timer1.Start();
                }
                else
                    MessageBox.Show("IP或端口 不合法");
            }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("conn_btn_Click", ex); }
        }

        /// <summary>
        /// TCP断开
        /// </summary>
        private void TcpDisconnect()
        {
            try
            {
                ysuTcpClient1.StopConnect();
                this.timer1.Stop();
                this.tcpServerMain.Stop();
            }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("stop_btn_Click", ex); }
        }

        /// <summary>
        /// 开始或停止采集
        /// </summary>
        /// <param name="flag">开始或停止采集的标志，true则开始采集，否则停止</param>
        private void StartSlashEndCollect(bool flag)
        {
            List<string> list = new List<string>() { "unlog" };
            if (flag)
                list.AddRange(new string[] { "log bestposa ontime 1", "log gphdt ontime 1" });
            this.SendData(list);
        }

        /// <summary>
        /// 接收数据并解析
        /// </summary>
        /// <param name="msg">接收到的十六进制字符串类型数据</param>
        /// <param name="data">接收到的字节数组类型数据</param>
        public void Receive(string msg, byte[] data)
        {
            string received_data = Encoding.Default.GetString(data); //接收数据
            this.Invoke(new Action(() =>
            {
                gpsallstr_txt.Text = received_data; //原始报文
                this.GnssInfo.Classify(received_data);
            }));
        }

        /// <summary>
        /// 根据TCP连接状态修改各控件状态
        /// </summary>
        /// <param name="state"></param>
        public void GetState(TcpClientStateEventArgs stateArgs)
        {
            try
            {
                this.button_Connect.Enabled = stateArgs.State != YsuSoftHelper.SocketState.Connected;
                this.ysuTcpClient1.IsStart = YsuSoftHelper.SocketState.Connected == stateArgs.State;
                if (stateArgs.State == YsuSoftHelper.SocketState.Connected)
                {
                    LblTcpState.Text = "已连接服务器";
                    LblTcpState.ForeColor = Color.FromArgb(0, 192, 0);
                    ysuTcpClient1.SendData(Encoding.Default.GetBytes(string.Format("&{0}&", clientModel.ToString())));
                    if (this.checkBox_AutoCollect.Checked)
                        this.StartSlashEndCollect(true);
                }
                else
                {
                    LblTcpState.Text = stateArgs.StateInfo;
                    LblTcpState.ForeColor = Color.Red;
                }
            }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("GetState", ex); }
        }

        /// <summary>
        /// 批量发送命令
        /// </summary>
        /// <param name="commands"></param>
        private void SendData(IEnumerable<string> commands)
        {
            if (!this.ysuTcpClient1.IsStart)
            {
                MessageBox.Show("未连接设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var command in commands)
                this.SendDataEach(command);
        }

        /// <summary>
        /// 向移动站发送指令
        /// </summary>
        /// <param name="command"></param>
        private void SendDataEach(string command)
        {
            if (!this.ysuTcpClient1.IsStart)
                return;

            string command_after = command + "\r\n\r\n";
            this.ysuTcpClient1.SendData(Encoding.Default.GetBytes(command_after));
            //this.lastCommand = command;
            this.commandStorage.PushCommand(command);
        }
        #endregion

        #region TCPSERVER
        /// <summary>
        /// 向客户端发送命令
        /// </summary>
        /// <param name="data"></param>
        private void SendData(object data)
        {
            List<ClientModel> list = this.tcpServerMain.ClientSocketList;
            if (list == null || list.Count == 0)
                return;

            foreach (ClientModel clientModel in list)
            {
                if (clientModel == null)
                    continue;
                if (clientModel.ClientStyle == CarServer.ClientStyle.WebSocket)
                    SocketTcpServer.SendToWebClient(clientModel, data.ToString());
                else
                    this.tcpServerMain.SendData(clientModel, data.ToString());
            }
        }
        #endregion
        #endregion

        #region 事件
        #region OPC
        /// <summary>
        /// OPC SERVER枚举按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ServerEnum_Click(object sender, EventArgs e)
        {
            this.ServerEnum();
        }

        /// <summary>
        /// 【按钮】连接ＯＰＣ服务器
        /// </summary>
        private void BtnConnLocalServer_Click(object sender, EventArgs e)
        {
            this.ConnectSlashDisconnect(this.btnConnLocalServer.Text.Equals("连接"));
            //bool connecting = this.btnConnLocalServer.Text.Equals("连接");
            //if (connecting) this.ConnectRemoteServer_Init();
            //else BaseConst.OpcHelper.DisconnectRemoteServer();
            //this.btnConnLocalServer.Text = connecting ? "断开" : "连接";
        }

        /// <summary>
        /// 添加测试写入标签，假如已添加标签，移除标签再重新添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddTestItem_Click(object sender, EventArgs e)
        {
            string message;
            if (!BaseConst.OpcHelper.SetItem(this.textBox_TestItemId.Text, 1, out message))
                MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 测试标签写入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_WriteTestItem_Click(object sender, EventArgs e)
        {
            string message;
            if (!BaseConst.OpcHelper.WriteItemValue(this.textBox_TestValue.Text, out message))
                MessageBox.Show(message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// OPC重连
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Reconn_Tick(object sender, EventArgs e)
        {
            string message;
            BaseConst.OpcHelper.Reconn(out message);
            this.label_OpcInfo.Text = message;
        }
        #endregion

        #region TCP
        /// <summary>
        /// TCP连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Connect_Click(object sender, EventArgs e)
        {
            this.TcpConnect();
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Disconnect_Click(object sender, EventArgs e)
        {
            this.TcpDisconnect();
        }

        /// <summary>
        /// 开始采集按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_StartCollect_Click(object sender, EventArgs e)
        {
            bool starting = this.button_StartCollect.Text == "开始采集";
            this.StartSlashEndCollect(starting);
            this.button_StartCollect.Text = starting ? "停止采集" : "开始采集";
        }

        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient1_OnReceive(object sender, YsuSoftHelper.ICommond.TcpClientReceviceEventArgs e)
        {
            try { this.Invoke(new OnReceviceCallBack(Receive), sender.ToString(), e.Data); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("iTcpClient1_OnRecevice", ex); }
        }

        /// <summary>
        /// TCP状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient1_OnStateInfo(object sender, TcpClientStateEventArgs e)
        {
            try { this.Invoke(new MethodInvoker(() => { GetState(e); })); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("iTcpClient1_OnStateInfo", ex); }
        }
        #endregion

        #region 控件
        /// <summary>
        /// 输入的IP地址变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_IpAddress_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Tcp", "IpAddress", this.textBox_IpAddress.Text);
        }

        /// <summary>
        /// 输入的端口变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Port_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Tcp", "Port", this.textBox_Port.Text);
        }

        /// <summary>
        /// OPC SERVER IP改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_RemoteServerIP_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Opc", "OpcServerIp", this.textBox_RemoteServerIP.Text);
        }

        /// <summary>
        /// 选择的OPC SERVER变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_RemoteServerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.remoteServerName = this.comboBox_RemoteServerName.Text;
        }

        /// <summary>
        /// 是否自动连接改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_AutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Opc", "OpcAutoConnect", this.checkBox_OpcAutoConnect.Checked ? "1" : "0");
        }

        /// <summary>
        /// 经度标签变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LongitudeItemId_TextChanged(object sender, EventArgs e)
        {
            this.GnssInfo.LongitudeItemId = this.textBox_LongitudeItemId.Text;
            BaseConst.IniHelper.WriteData("Opc", "LongitudeItemId", this.GnssInfo.LongitudeItemId);
        }

        /// <summary>
        /// 纬度标签变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LatitudeItemId_TextChanged(object sender, EventArgs e)
        {
            this.GnssInfo.LatitudeItemId = this.textBox_LatitudeItemId.Text;
            BaseConst.IniHelper.WriteData("Opc", "LatitudeItemId", this.GnssInfo.LatitudeItemId);
        }

        private void textBox_AltitudeItemId_TextChanged(object sender, EventArgs e)
        {
            this.GnssInfo.AltitudeItemId = this.textBox_AltitudeItemId.Text;
            BaseConst.IniHelper.WriteData("Opc", "AltitudeItemId", this.GnssInfo.AltitudeItemId);
        }

        /// <summary>
        /// 俯仰角度标签变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PitchItemId_TextChanged(object sender, EventArgs e)
        {
            this.GnssInfo.PitchItemId = this.textBox_PitchItemId.Text;
            BaseConst.IniHelper.WriteData("Opc", "PitchItemId", this.GnssInfo.PitchItemId);
        }

        /// <summary>
        /// 测试标签变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TestItemId_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Opc", "TestItemId", this.textBox_TestItemId.Text);
        }

        /// <summary>
        /// 大机ID变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ClaimerId_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "ClaimerId", this.textBox_ClaimerId.Text);
            this.GnssInfo.ClaimerId = this.textBox_ClaimerId.Text;
        }

        /// <summary>
        /// 自动采集变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_AutoCollect_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Tcp", "AutoCollect", this.checkBox_AutoCollect.Checked ? "1" : "0");
        }

        /// <summary>
        /// 活动状态改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_IsGroupsActive_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.OpcHelper.IsGroupsActive = this.checkBox_IsGroupsActive.Checked;
        }

        /// <summary>
        /// 激活状态改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_IsGroupActive_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.OpcHelper.IsGroupActive = this.checkBox_IsGroupActive.Checked;
        }

        /// <summary>
        /// 订阅状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_IsGroupSubscribed_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.OpcHelper.IsGroupSubscribed = this.checkBox_IsGroupSubscribed.Checked;
        }

        /// <summary>
        /// 不敏感区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_GroupsDeadband_ValueChanged(object sender, EventArgs e)
        {
            BaseConst.OpcHelper.GroupsDeadband = (float)this.numeric_GroupsDeadband.Value;
        }

        /// <summary>
        /// 更新速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_GroupUpdateRate_ValueChanged(object sender, EventArgs e)
        {
            BaseConst.OpcHelper.GroupUpdateRate = (int)this.numeric_GroupUpdateRate.Value;
        }

        /// <summary>
        /// UI更新Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_UiUpdate_Tick(object sender, EventArgs e)
        {
            //信息
            //this.textBox_LongitudeValue.Text = this.GnssInfo.Longitude.ToString();
            //this.textBox_LatitudeValue.Text = this.GnssInfo.Latitude.ToString();
            this.textBox_LongitudeValue.Text = this.GnssInfo.LocalCoor_Ante.X.ToString();
            this.textBox_LatitudeValue.Text = this.GnssInfo.LocalCoor_Ante.Y.ToString();
            this.textBox_Height.Text = this.GnssInfo.Altitude.ToString();
            this.textBox_PitchAngle.Text = this.GnssInfo.PitchAngle.ToString();
            this.textBox_GpsQuality.Text = this.GnssInfo.PositionQuality;
            //this.textBox_GpsQuality.Text = this.GnssInfo.PositionType.GetDescription();
            this.textBox_TrackDirection.Text = this.GnssInfo.TrackDirection_TrueNorth.ToString();
            this.textBox_TrackLocalNorth.Text = this.GnssInfo.YawAngle.ToString();
            this.textBox_MagDec.Text = this.GnssInfo.MagneticDeclination.ToString();
            //this.textBox_MagDecDir.Text = this.GnssInfo.MagneticDeclinationDir;

            //状态指示
            this.label_MessageError.Text = this.GnssInfo.DictErrorMessages["GNSS"];
            this.label_OpcError.Text = this.GnssInfo.DictErrorMessages["OPC"];
            this.statusLabel_WebService.Text = this.GnssInfo.DictErrorMessages["WebService"];
            this.statusLabel_DataService.Text = this.GnssInfo.DictErrorMessages["DataService"];
        }

        /// <summary>
        /// OPC更新Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_OpcUpdate_Tick(object sender, EventArgs e)
        {
            //假如俯仰角获取模式为OPC，则从标签中读取
            if (BaseConst.PitchAngleMode == PitchAngleMode.OPC)
            {
                string message;
                try
                {
                    BaseConst.OpcRead.ReadValues(out message);
                    this.GnssInfo.DictErrorMessages["OPC"] = message;
                    this.GnssInfo.PitchAngle = double.Parse(BaseConst.OpcReadPitch.Value);
                }
                catch (Exception ex)
                {
                    message = "从OPC组读取数据并转换俯仰角时出现异常：" + ex.Message;
                    this.GnssInfo.DictErrorMessages["OPC"] = message;
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.SendData(string.Format("beidou-x:{0};y:{1};tip_height:{2}", Math.Round(this.GnssInfo.LocalCoor_Tip.XPrime, 4), Math.Round(this.GnssInfo.LocalCoor_Tip.YPrime, 4), Math.Round(this.GnssInfo.LocalCoor_Tip.Z, 4)));
        }

        /// <summary>
        /// 指令输入框按键按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Command_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //向上键，找到上一个发送的命令
                case Keys.Up:
                    this.textBox_Command.Text = this.commandStorage.LastCommand();
                    break;
                case Keys.Down:
                    this.textBox_Command.Text = this.commandStorage.NextCommand();
                    break;
                default:
                    return;
            }

            e.Handled = true;
        }

        /// <summary>
        /// 指令输入框按键按下弹起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Command_KeyPress(object sender, KeyPressEventArgs e)
        {
            Keys key = (Keys)e.KeyChar;
            switch (key)
            {
                //Enter键，发送命令
                case Keys.Enter:
                    if (!string.IsNullOrWhiteSpace(this.textBox_Command.Text) && this.ysuTcpClient1.IsStart)
                    {
                        this.SendDataEach(this.textBox_Command.Text);
                        this.textBox_Command.Text = string.Empty;
                    }
                    break;
                default:
                    return;
            }

            e.Handled = true;
        }
        #endregion
        #endregion
    }
}
