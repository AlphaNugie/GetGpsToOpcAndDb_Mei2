using CommonLib.Clients;
using CommonLib.Enums;
using CommonLib.Events;
using CommonLib.Extensions.Property;
using CommonLib.Function;
using CommonLib.UIControlUtil;
using GetGpsToOpcAndDb.Core;
using GetGpsToOpcAndDb.Model;
using ProtobufNetLibrary;
using SerializationFactory;
using SocketHelper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace GetGpsToOpcAndDb
{
    public partial class FormMain : Form
    {
        //public delegate void OnReceviceCallBack(object sender, ReceivedEventArgs eventArgs); //TCP接收事件委托
        public delegate void OnReceviceCallBack(string receivedString); //TCP接收事件委托

        #region 私有变量
        //private const string OPCGROUP_NAME_READ = "OpcGroupRead", OPCGROUP_NAME_WRITE = "OpcGroupWrite";
        private string _remoteServerName = string.Empty; //OPC SERVER名称
        private ConnectionMode _connMode = ConnectionMode.TCP;
        //private readonly ClientType _clientModel = ClientType.None;
        private readonly CommandStorage _commandStorage = new CommandStorage();
        private readonly TimerEventRaiser _raiser = new TimerEventRaiser(1000);
        private readonly DerivedHttpListener _httpListener = new DerivedHttpListener();
        //private const int LONGITUDE_WRITE_HANDLE = 101, LATITUDE_WRITE_HANDLE = 102, ALTITUDE_WRITE_HANDLE = 103, WALKING_WRITE_HANDLE = 104, PITCH_WRITE_HANDLE = 105, YAW_WRITE_HANDLE = 106, WALKING_READ_HANDLE = 107, PITCH_READ_HANDLE = 108, YAW_READ_HANDLE = 109, RANDOM_WRITE_HANDLE = 110; //经度，纬度，海拔，俯仰，回转角等OPC项的客户端句柄
        private const string right = "▶", left = "◀";
        private readonly int expand_size = 438, width_narrow = 1029;
        //private TimerEventRaiser raiser = new TimerEventRaiser(1000) { RaiseThreshold = 5000, RaiseInterval = 10000 };
        #endregion

        ///// <summary>
        ///// GPS信息实体类对象
        ///// </summary>
        //public GnssInfoObject GnssInfo { get; set; }

        ///// <summary>
        ///// OPC工具
        ///// </summary>
        //public OpcTask OpcTask { get; private set; }

        private GnssProtoInfo _gnss_proto_info = new GnssProtoInfo();
        /// <summary>
        /// 记录所有需传输北斗数据的通用实体类
        /// </summary>
        public GnssProtoInfo GnssProtoInfo
        {
            get { return _gnss_proto_info; }
            private set { _gnss_proto_info = value; }
        }

        /// <summary>
        /// 是否已连接到设备SOCKET
        /// </summary>
        public bool IsStart
        {
            get
            {
                switch (_connMode)
                {
                    case ConnectionMode.TCP:
                        return tcpClient.IsConnected_Socket;
                    case ConnectionMode.TCPS:
                        return tcpServerConn.ClientSocketList.Count > 0;
                    case ConnectionMode.HTTP:
                        return true;
                    default:
                        return false;
                }
                //return (_connMode == ConnectionMode.TCP && tcpClient.IsConnected_Socket) || (_connMode == ConnectionMode.TCPS && tcpServerConn.ClientSocketList.Count > 0);
            }
        }

        /// <summary>
        /// 构造器
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException_Raising); //未捕获异常触发事件
            BaseConst.GnssInfo.ClaimerId = textBox_ClaimerId.Text;
            //BaseConst.OpcTask = new OpcTask() { Interval = BaseConst.OpcUpdateRate };
            _raiser.RaiseThreshold = 10000;
            _raiser.ThresholdReached += new TimerEventRaiser.ThresholdReachedEventHandler(Raiser_ThresholdReached);
            _raiser.Clicked += new TimerEventRaiser.ClickedEventHandler(Raiser_Clicked);
            _httpListener.ReceiveBufferSize = 2048;
            _httpListener.ServiceStateChanged += new ServiceStateEventHandler(HttpListener_ServiceStateChanged);
            _httpListener.DataReceived += new DataReceivedEventHandler(HttpListener_DataReceived);
            InitControls();

            //TODO 配置服务地址
            //利用WebService构造器的重载方法，在Config.ini文件中修改，重载方法的endpointConfigurationName参数：App.config文件中的system.ServiceModel=>client=>endpoint节点的name属性值
            BaseConst.Log.WriteLogsToFile("主窗体初始化完成");
        }

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Raiser_Clicked(object sender, ClickedEventArgs e)
        {
            BaseConst.GnssInfo.Working = true;
        }

        /// <summary>
        /// 超时未接收数据后引发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Raiser_ThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            BaseConst.GnssInfo.Working = false;
        }

        /// <summary>
        /// 控件初始化
        /// </summary>
        private void InitControls()
        {
            BaseConst.Log.WriteLogsToFile("1");
            Width = width_narrow;
            timer_OpcUpdate.Interval = BaseConst.OpcTask.Interval;
            timer_UiUpdate.Start();
            timer_RecordCoor.Start();

            BaseConst.Log.WriteLogsToFile("2");
            //TCP
            textBox_IpAddress.Text = BaseConst.IniHelper.ReadData("Tcp", "IpAddress");
            textBox_Port.Text = BaseConst.IniHelper.ReadData("Tcp", "Port");
            comboBox_ConnMode.Text = BaseConst.IniHelper.ReadData("Tcp", "ConnMode");
            ConnectionMode mode;
            if (Enum.TryParse(comboBox_ConnMode.Text, out mode))
                _connMode = mode;
            //_connMode = (ConnectionMode)Enum.Parse(typeof(ConnectionMode), comboBox_ConnMode.Text);
            checkBox_AutoCollect.Checked = BaseConst.IniHelper.ReadData("Tcp", "AutoCollect").Equals("1");

            BaseConst.Log.WriteLogsToFile("3");
            //OPC
            textBox_RemoteServerIP.Text = BaseConst.OpcServerIp;
            _remoteServerName = BaseConst.OpcServerName;
            checkBox_OpcAutoConnect.Checked = BaseConst.OpcAutoConnect;

            BaseConst.Log.WriteLogsToFile("4");
            checkBox_IsGroupsActive.Checked = BaseConst.OpcTask.OpcHelper.IsGroupsActive;
            numeric_GroupsDeadband.Value = (decimal)BaseConst.OpcTask.OpcHelper.GroupsDeadband;
            checkBox_IsGroupActive.Checked = BaseConst.OpcTask.OpcHelper.IsGroupActive;
            checkBox_IsGroupSubscribed.Checked = BaseConst.OpcTask.OpcHelper.IsGroupSubscribed;
            numeric_GroupUpdateRate.Value = BaseConst.OpcTask.OpcHelper.GroupUpdateRate;

            BaseConst.Log.WriteLogsToFile("5");
            //textBox_LongitudeItemId.Text = BaseConst.IniHelper.ReadData("Opc", "LongitudeItemId");
            //textBox_LatitudeItemId.Text = BaseConst.IniHelper.ReadData("Opc", "LatitudeItemId");
            //textBox_AltitudeItemId.Text = BaseConst.IniHelper.ReadData("Opc", "AltitudeItemId");
            //textBox_PitchItemId.Text = BaseConst.IniHelper.ReadData("Opc", "PitchItemId");
            textBox_TestItemId.Text = BaseConst.IniHelper.ReadData("Opc", "TestItemId");

            BaseConst.Log.WriteLogsToFile("6");
            textBox_ClaimerId.Text = BaseConst.IniHelper.ReadData("Main", "ClaimerId"); //大机ID
        }

        /// <summary>
        /// 加载
        /// </summary>
        private void Loading()
        {
            BaseConst.Log.WriteLogsToFile("窗体加载...");
            statusLabel_ServerState.Text = string.Empty;
            statusLabel_ServerStartTime.Text = string.Empty;
            statusLabel_Version.Text = string.Empty;
            //OPC自动连接
            //if (checkBox_OpcAutoConnect.Checked)
            if (BaseConst.OpcAutoConnect)
                //ConnectRemoteServer_Init();
                ConnectSlashDisconnect(true);
            //自动采集则自动连接，连接后在状态改变事件内自动发送采集消息
            if (checkBox_AutoCollect.Checked)
                //TcpConnect();
                Connect();
            _raiser.Run();
            tcpServerMain.Start();
            timer_Upload.Start();
            BaseConst.Log.WriteLogsToFile("窗体加载完成");

            #region 测试运行时间
            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //int i;
            //for (i = 0; i < 1000; i++)
            //    BaseConst.GnssInfo.GetLocalCoordinates();
            //watch.Stop();
            //long milli = watch.ElapsedMilliseconds;
            #endregion
        }

        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            Loading();
        }

        /// <summary>
        /// 页面临关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (tcpClient.IsStart)
            //    tcpClient.StopConnection();
            //if (IsStart)
            Disconnect();
            timer_Upload.Stop();
            tcpServerMain.Stop();
            //timer_OpcUpdate.Stop();
            timer_Reconn.Stop();
            timer_UiUpdate.Stop();
            EndOpcTask();
            //BaseConst.OpcTask.OpcHelper.DisconnectRemoteServer(); //断开OPC服务
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
        private void UpdateUI()
        {
            //信息
            textBox_LongitudeValue.Text = BaseConst.GnssInfo.LocalCoor_Ante.X.ToString();
            textBox_LatitudeValue.Text = BaseConst.GnssInfo.LocalCoor_Ante.Y.ToString();
            textBox_Height.Text = BaseConst.GnssInfo.Altitude.ToString();
            textBox_PitchAngle.Text = BaseConst.GnssInfo.PitchAngle.ToString();
            textBox_GpsQuality.Text = BaseConst.GnssInfo.PositionQuality;
            //textBox_GpsQuality.Text = BaseConst.GnssInfo.PositionType.GetDescription();
            textBox_TrackDirection.Text = BaseConst.GnssInfo.TrackDirection_TrueNorth.ToString();
            textBox_TrackLocalNorth.Text = BaseConst.GnssInfo.YawAngle.ToString();
            textBox_BaselineLength.Text = BaseConst.GnssInfo.BaselineLength.ToString();
            textBox_AntePitch.Text = BaseConst.GnssInfo.AntePitch.ToString();
            textBox_MagDec.Text = BaseConst.GnssInfo.MagneticDeclination.ToString();
            //textBox_MagDecDir.Text = BaseConst.GnssInfo.MagneticDeclinationDir;

            //状态指示
            label_GpsQuality.ForeColor = BaseConst.GnssInfo.IsFixedSolution ? Color.Green : Color.Red;
            label_TrackDirection.ForeColor = BaseConst.GnssInfo.TrackDirection_Received ? Color.Green : Color.Red;
            label_MessageError.Text = BaseConst.GnssInfo.DictErrorMessages["GNSS"];
            label_OpcError.Text = BaseConst.GnssInfo.DictErrorMessages["OPC"];
            label_ReconnCounter.Text = tcpClient.ReConnectedCount.ToString();
            statusLabel_WebService.Text = BaseConst.GnssInfo.DictErrorMessages["WebService"];
            statusLabel_DataService.Text = BaseConst.GnssInfo.DictErrorMessages["DataService"];

            textBox_Info.Text = GetCompleteMessage();
            _httpListener.WebExplorerMessage = textBox_Info.Text;
        }

        /// <summary>
        /// 获取所有信息
        /// </summary>
        /// <returns></returns>
        private string GetCompleteMessage()
        {
            string output = string.Format(@"{12:yyyy-MM-dd HH:mm:ss}=>
经度：{0}，
纬度：{1}，
海拔：{2}，
本地XYZ（对内）：{3}，
本地XYZ（对外）：{4}，
俯仰角：{5}，
回转角：{6}，
行走位置：{7}，
落料口（本地）：{8}，
俯仰轴（本地）：{9}，
回转轴（本地）：{10}，
回转轴（单机）：{11}", BaseConst.GnssInfo.Longitude, BaseConst.GnssInfo.Latitude, BaseConst.GnssInfo.Altitude, BaseConst.GnssInfo.LocalCoor_Ante.ToString("default"), BaseConst.GnssInfo.LocalCoor_Ante.ToString("prime"), BaseConst.GnssInfo.PitchAngle, BaseConst.GnssInfo.YawAngle, BaseConst.GnssInfo.WalkingPosition, BaseConst.GnssInfo.LocalCoor_Tip.ToString("prime"), BaseConst.GnssInfo.LocalCoor_PitchAxis.ToString("prime"), BaseConst.GnssInfo.LocalCoor_YawAxis.ToString("prime"), BaseConst.GnssInfo.LocalCoor_YawAxis.ToString("claimer"), DateTime.Now);
            return output;
        }

        /// <summary>
        /// 更新北斗信息
        /// </summary>
        private void UpdateBeidouInfo()
        {
            GnssProtoInfo.CopyPropertyValueFrom(BaseConst.GnssInfo);
        }

        #region OPC方法
        /// <summary>
        /// 枚举OPC SERVER
        /// </summary>
        private void ServerEnum()
        {
            string ipAddress = textBox_RemoteServerIP.Text, message;
            comboBox_RemoteServerName.Items.Clear(); //清空已显示的OPC Server列表
            string[] array = BaseConst.OpcTask.OpcHelper.ServerEnum(ipAddress, out message);
            if (!string.IsNullOrWhiteSpace(message))
            {
                MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //假如Server列表为空，退出方法，否则为ListBoxControl添加Item
            if (array.Length == 0)
                return;
            comboBox_RemoteServerName.Items.AddRange(array);
            comboBox_RemoteServerName.SelectedIndex = 0;
        }

        /// <summary>
        /// 获取服务器信息，并显示在窗体状态栏上
        /// </summary>
        private void GetServerInfo()
        {
            statusLabel_ServerStartTime.Text = BaseConst.OpcTask.OpcHelper.ServerStartTime;
            statusLabel_Version.Text = BaseConst.OpcTask.OpcHelper.ServerVersion;
            statusLabel_ServerState.Text = BaseConst.OpcTask.OpcHelper.ServerState;
        }

        /// <summary>
        /// 连接或断开
        /// </summary>
        /// <param name="connecting">是否要连接</param>
        private void ConnectSlashDisconnect(bool connecting)
        {
            //if (connecting) ConnectRemoteServer_Init();
            //else BaseConst.OpcTask.OpcHelper.DisconnectRemoteServer();
            if (connecting) InitOpcTask();
            else EndOpcTask();
            btnConnLocalServer.Text = connecting ? "断开" : "连接";
        }

        /// <summary>
        /// OPC初始化
        /// </summary>
        private void InitOpcTask()
        {
            BaseConst.OpcTask = new OpcTask();
            //if (!BaseConst.OpcAutoConnect)
            //    return;

            BaseConst.IniHelper.WriteData("Opc", "OpcServerName", _remoteServerName); //保存OPC服务名称
            BaseConst.OpcTask.Init();
            BaseConst.OpcTask.Run();
            GetServerInfo();
            timer_OpcUpdate.Start();
            //label_opc.SafeInvoke(() => label_opc.Text = BaseConst.OpcTask.ErrorMessage);
        }

        private void EndOpcTask()
        {
            //if (BaseConst.OpcTask == null)
            //    return;

            timer_OpcUpdate.Stop();
            BaseConst.OpcTask.Stop();
            BaseConst.OpcTask.OpcHelper.DisconnectRemoteServer();
            //BaseConst.OpcTask = null;
        }
        #endregion

        #region 连接
        /// <summary>
        /// 建立连接（TCP）或建立监听（UDP/TCPS）
        /// </summary>
        private void Connect()
        {
            try
            {
                //string ipStr = textBox_IpAddress.Text;
                IPAddress ip;
                int port;
                if (/*(_connMode == ConnectionMode.HTTP && !textBox_IpAddress.Text.Equals("+")) || */!IPAddress.TryParse(textBox_IpAddress.Text, out ip) || !int.TryParse(textBox_Port.Text, out port))
                {
                    MessageBox.Show("IP或端口 不合法");
                    return;
                }
                switch (_connMode)
                {
                    case ConnectionMode.TCP:
                        TcpConnect(ip.ToString(), port);
                        break;
                    case ConnectionMode.UDP:
                        break;
                    case ConnectionMode.TCPS:
                        TcpServerInit(ip.ToString(), port);
                        break;
                    case ConnectionMode.HTTP:
                        HttpListenerStart(ip.ToString(), port);
                        break;
                }
                //comboBox_ConnMode.Enabled = false;
                //timer1.Start();
            }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("conn_btn_Click", ex); }
        }

        /// <summary>
        /// 断开连接（TCP）或初始化（UDP/TCPS）
        /// </summary>
        private void Disconnect()
        {
            try
            {
                switch (_connMode)
                {
                    case ConnectionMode.TCP:
                        TcpDisconnect();
                        break;
                    case ConnectionMode.UDP:
                        break;
                    case ConnectionMode.TCPS:
                        TcpServerStop();
                        break;
                    case ConnectionMode.HTTP:
                        HttpListenerStop();
                        break;
                }
                //comboBox_ConnMode.Enabled = true;
                //timer1.Stop();
            }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("stop_btn_Click", ex); }
        }

        /// <summary>
        /// TCP连接
        /// </summary>
        /// <param name="ip">远程IP地址</param>
        /// <param name="port">远程端口</param>
        private void TcpConnect(string ip, int port)
        {
            tcpClient.ServerIp = ip;
            tcpClient.ServerPort = port;
            tcpClient.StartConnection();
        }

        /// <summary>
        /// TCP断开
        /// </summary>
        private void TcpDisconnect()
        {
            tcpClient.StopConnection();
        }

        /// <summary>
        /// TCP监听启动
        /// </summary>
        /// <param name="ip">本地IP地址</param>
        /// <param name="port">本地端口</param>
        private void TcpServerInit(string ip, int port)
        {
            tcpServerConn.ServerIp = ip;
            tcpServerConn.ServerPort = port;
            tcpServerConn.Start();
        }

        /// <summary>
        /// TCP监听关闭
        /// </summary>
        private void TcpServerStop()
        {
            tcpServerConn.Stop();
        }

        /// <summary>
        /// HTTP监听服务启动
        /// </summary>
        /// <param name="ip">本地IP地址</param>
        /// <param name="port">HTTP服务的访问端口号</param>
        private void HttpListenerStart(string ip, int port)
        {
            _httpListener.IpAddress = ip;
            _httpListener.Port = port;
            _httpListener.Suffix = "/web/";
            if (_httpListener.Start() == 0)
                GetState(false, _httpListener.LastErrorMessage);
        }

        /// <summary>
        /// HTTP监听服务停止
        /// </summary>
        private void HttpListenerStop()
        {
            if (_httpListener.Stop() == 0)
                GetState(false, _httpListener.LastErrorMessage);
        }

        /// <summary>
        /// 开始或停止采集
        /// </summary>
        /// <param name="flag">开始或停止采集的标志，true则开始采集，否则停止</param>
        private void StartSlashEndCollect(bool flag)
        {
            List<string> list = new List<string>() { "unlog" };
            if (flag)
                list.AddRange(new string[] { "log bestposa ontime " + BaseConst.ReceiveInterval, "log gphdt ontime " + BaseConst.ReceiveInterval });
            SendData(list);
        }

        /// <summary>
        /// 接收数据并解析
        /// </summary>
        /// <param name="received">接收到的字符串类型数据</param>
        public void Receive(string received)
        {
            _raiser.Click(received);
            //string received_data = Encoding.Default.GetString(data); //接收数据
            Invoke(new Action(() =>
            {
                gpsallstr_txt.Text = received; //原始报文
                BaseConst.GnssInfo.Classify(received);
            }));
        }

        ///// <summary>
        ///// 接收数据并解析
        ///// </summary>
        ///// <param name="sender">接收到的十六进制字符串类型数据</param>
        ///// <param name="data">接收到的字节数组类型数据</param>
        //public void Receive(object sender, ReceivedEventArgs eventArgs)
        //{
        //    _raiser.Click(eventArgs.ReceivedString);
        //    //string received_data = Encoding.Default.GetString(data); //接收数据
        //    Invoke(new Action(() =>
        //    {
        //        gpsallstr_txt.Text = eventArgs.ReceivedString; //原始报文
        //        BaseConst.GnssInfo.Classify(eventArgs.ReceivedString);
        //    }));
        //}

        /// <summary>
        /// 根据TCP连接状态修改各控件状态
        /// </summary>
        /// <param name="started">是否启动</param>
        /// <param name="stateInfo">状态信息</param>
        public void GetState(bool started, string stateInfo)
        {
            try
            {
                button_Connect.Enabled = !started;
                //button_Disconnect.Enabled = started;
                LblTcpState.Text = stateInfo;
                LblTcpState.ForeColor = started ? Color.FromArgb(0, 192, 0) : Color.Red; //根据连接状态
                comboBox_ConnMode.Enabled = !started;
                if (started)
                {
                    //tcpServerMain.Start();
                    //timer_Upload.Start();
                    //tcpClient.SendData(string.Format("&{0}&", tcpClient.Name));
                    if (checkBox_AutoCollect.Checked)
                        StartSlashEndCollect(true);
                }
                else
                {
                    //tcpServerMain.Stop();
                    //timer_Upload.Stop();
                }
            }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("GetState", ex); }
        }

        ///// <summary>
        ///// 根据TCP连接状态修改各控件状态
        ///// </summary>
        ///// <param name="stateArgs"></param>
        //public void TcpGetState(bool started, StateInfoEventArgs stateArgs)
        //{
        //    try
        //    {
        //        button_Connect.Enabled = !started;
        //        //button_Disconnect.Enabled = started;
        //        LblTcpState.Text = stateArgs.StateInfo;
        //        LblTcpState.ForeColor = started ? Color.FromArgb(0, 192, 0) : Color.Red; //根据连接状态
        //        comboBox_ConnMode.Enabled = !started;
        //        if (started)
        //        {
        //            //tcpServerMain.Start();
        //            //timer_Upload.Start();
        //            //tcpClient.SendData(string.Format("&{0}&", tcpClient.Name));
        //            if (checkBox_AutoCollect.Checked)
        //                StartSlashEndCollect(true);
        //        }
        //        else
        //        {
        //            //tcpServerMain.Stop();
        //            //timer_Upload.Stop();
        //        }
        //    }
        //    catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("GetState", ex); }
        //}

        /// <summary>
        /// 新客户端上线的处理方法
        /// </summary>
        /// <param name="eventArgs"></param>
        public void TcpServerClientOnline(ClientOnlineEventArgs eventArgs)
        {
            if (eventArgs.Client == null)
                return;

            tcpServerConn.SendData(eventArgs.Client, string.Format("&{0}&", tcpServerConn.Name));
            //if (checkBox_AutoCollect.Checked)
            //    StartSlashEndCollect(true);
        }

        /// <summary>
        /// 批量发送命令
        /// </summary>
        /// <param name="commands"></param>
        private void SendData(IEnumerable<string> commands)
        {
            //if (!tcpClient.IsStart)
            if (!IsStart)
            {
                MessageBox.Show("未连接设备", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (var command in commands)
                SendDataEach(command);
        }

        /// <summary>
        /// 向移动站发送指令
        /// </summary>
        /// <param name="command"></param>
        private void SendDataEach(string command)
        {
            //if (!tcpClient.IsStart)
            if (!IsStart)
                return;

            string command_after = command + "\r\n\r\n";
            switch (_connMode)
            {
                case ConnectionMode.TCP:
                    tcpClient.SendData(Encoding.Default.GetBytes(command_after));
                    break;
                case ConnectionMode.TCPS:
                    tcpServerConn.SendData(Encoding.Default.GetBytes(command_after));
                    break;
            }
            //lastCommand = command;
            _commandStorage.Push(command);
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
            ServerEnum();
        }

        /// <summary>
        /// 【按钮】连接ＯＰＣ服务器
        /// </summary>
        private void BtnConnLocalServer_Click(object sender, EventArgs e)
        {
            ConnectSlashDisconnect(btnConnLocalServer.Text.Equals("连接"));
        }

        /// <summary>
        /// 添加测试写入标签，假如已添加标签，移除标签再重新添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_AddTestItem_Click(object sender, EventArgs e)
        {
            string message;
            if (!BaseConst.OpcTask.OpcHelper.SetItem(textBox_TestItemId.Text, 1, out message))
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
            if (!BaseConst.OpcTask.OpcHelper.WriteItemValue(textBox_TestValue.Text, out message))
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
            BaseConst.OpcTask.OpcHelper.Reconn(out message);
            label_OpcInfo.Text = message;
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
            Connect();
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Disconnect_Click(object sender, EventArgs e)
        {
            Disconnect();
        }

        /// <summary>
        /// 开始采集按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_StartCollect_Click(object sender, EventArgs e)
        {
            bool starting = button_StartCollect.Text == "开始采集";
            StartSlashEndCollect(starting);
            button_StartCollect.Text = starting ? "停止采集" : "开始采集";
        }

        /// <summary>
        /// 接收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_OnReceive(object sender, ReceivedEventArgs e)
        {
            try { Invoke(new OnReceviceCallBack(Receive), e.ReceivedString); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("iTcpClient1_OnRecevice", ex); }
        }

        /// <summary>
        /// TCP状态改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TcpClient_OnStateInfo(object sender, StateInfoEventArgs e)
        {
            bool connected = e.State == SocketState.Connected;
            if (connected)
                tcpClient.SendData(string.Format("&{0}&", tcpClient.Name));
            try { Invoke(new MethodInvoker(() => { GetState(connected, e.StateInfo); })); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("iTcpClient1_OnStateInfo", ex); }
        }

        private void TcpServerConn_Received(object sender, ReceivedEventArgs e)
        {
            try { Invoke(new OnReceviceCallBack(Receive), e.ReceivedString); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("iTcpServer2_OnReceive", ex); }
        }

        private void TcpServerConn_OnStateInfo(object sender, StateInfoEventArgs e)
        {
            bool started = e.State != SocketState.StopListening;
            try { Invoke(new MethodInvoker(() => { GetState(started, e.StateInfo); })); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("iTcpServer2_OnStateInfo", ex); }
        }

        private void TcpServerConn_OnClientOnline(object sender, ClientOnlineEventArgs eventArgs)
        {
            try { Invoke(new MethodInvoker(() => { TcpServerClientOnline(eventArgs); })); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("iTcpServer2_ClientOnline", ex); }
        }

        private void HttpListener_ServiceStateChanged(object sender, ServiceStateEventArgs e)
        {
            bool started = e.State == ServiceState.Started;
            try { Invoke(new MethodInvoker(() => { GetState(started, e.StateInfo); })); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("HttpListener_ServiceStateChanged", ex); }
        }

        private void HttpListener_DataReceived(object sender, DataReceivedEventArgs e)
        {
            //int length1 = e.ReceivedData.Length, length2 = e.ReceivedInfo_String.Length;
            //尝试对可能存在的乱码进行解码
            //例如 deviceId=Rrrrrryyyghh&command=%23HEADINGA%2CCOM3%2C0%2C60.0%2CFINESTEERING%2C2174%2C203295.400%2C00000000%2C0000%2C1114%3BSOL_COMPUTED%2CNARROW_FLOAT%2C1.883086681%2C217.139678955%2C-36.801155090%2C0.000000000%2C82.640968323%2C89.471420288%2C%220004%22%2C16%2C12%2C16%2C16%2C0%2C0%2C5%2C121*33a3a265
            //string received = HttpUtility.UrlDecode(e.ReceivedInfo_String, Encoding.GetEncoding("GB2312"));
            //try { Invoke(new OnReceviceCallBack(Receive), received); }
            try { Invoke(new OnReceviceCallBack(Receive), e.ReceivedInfo_String); }
            catch (Exception ex) { YsuSoftHelper.Helper.logHelper.WriteErrLog("iTcpServer2_OnReceive", ex); }
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
            BaseConst.IniHelper.WriteData("Tcp", "IpAddress", textBox_IpAddress.Text);
        }

        /// <summary>
        /// 输入的端口变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_Port_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Tcp", "Port", textBox_Port.Text);
        }

        private void ComboBox_ConnMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionMode mode;
            if (Enum.TryParse(comboBox_ConnMode.Text, out mode))
            {
                BaseConst.IniHelper.WriteData("Tcp", "ConnMode", comboBox_ConnMode.Text);
                _connMode = mode;
                //“开始采集”按钮根据是否为HTTP服务模式决定是否可用
                textBox_IpAddress.Enabled = _connMode != ConnectionMode.HTTP;
                button_StartCollect.Enabled = _connMode != ConnectionMode.HTTP;
            }
        }

        /// <summary>
        /// OPC SERVER IP改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_RemoteServerIP_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Opc", "OpcServerIp", textBox_RemoteServerIP.Text);
        }

        /// <summary>
        /// 选择的OPC SERVER变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_RemoteServerName_SelectedIndexChanged(object sender, EventArgs e)
        {
            _remoteServerName = comboBox_RemoteServerName.Text;
        }

        /// <summary>
        /// 是否自动连接改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_AutoConnect_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Opc", "OpcAutoConnect", checkBox_OpcAutoConnect.Checked ? "1" : "0");
        }

        ///// <summary>
        ///// 经度标签变化
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void TextBox_LongitudeItemId_TextChanged(object sender, EventArgs e)
        //{
        //    BaseConst.GnssInfo.LongitudeItemId = textBox_LongitudeItemId.Text;
        //    BaseConst.IniHelper.WriteData("Opc", "LongitudeItemId", BaseConst.GnssInfo.LongitudeItemId);
        //}

        ///// <summary>
        ///// 纬度标签变化
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void TextBox_LatitudeItemId_TextChanged(object sender, EventArgs e)
        //{
        //    BaseConst.GnssInfo.LatitudeItemId = textBox_LatitudeItemId.Text;
        //    BaseConst.IniHelper.WriteData("Opc", "LatitudeItemId", BaseConst.GnssInfo.LatitudeItemId);
        //}

        //private void TextBox_AltitudeItemId_TextChanged(object sender, EventArgs e)
        //{
        //    BaseConst.GnssInfo.AltitudeItemId = textBox_AltitudeItemId.Text;
        //    BaseConst.IniHelper.WriteData("Opc", "AltitudeItemId", BaseConst.GnssInfo.AltitudeItemId);
        //}

        ///// <summary>
        ///// 俯仰角度标签变化
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void TextBox_PitchItemId_TextChanged(object sender, EventArgs e)
        //{
        //    BaseConst.GnssInfo.PitchItemId = textBox_PitchItemId.Text;
        //    BaseConst.IniHelper.WriteData("Opc", "PitchItemId", BaseConst.GnssInfo.PitchItemId);
        //}

        /// <summary>
        /// 测试标签变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TestItemId_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Opc", "TestItemId", textBox_TestItemId.Text);
        }

        /// <summary>
        /// 大机ID变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ClaimerId_TextChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Main", "ClaimerId", textBox_ClaimerId.Text);
            BaseConst.GnssInfo.ClaimerId = textBox_ClaimerId.Text;
        }

        /// <summary>
        /// 自动采集变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_AutoCollect_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.IniHelper.WriteData("Tcp", "AutoCollect", checkBox_AutoCollect.Checked ? "1" : "0");
        }

        /// <summary>
        /// 活动状态改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_IsGroupsActive_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.OpcTask.OpcHelper.IsGroupsActive = checkBox_IsGroupsActive.Checked;
        }

        /// <summary>
        /// 激活状态改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_IsGroupActive_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.OpcTask.OpcHelper.IsGroupActive = checkBox_IsGroupActive.Checked;
        }

        /// <summary>
        /// 窗口右侧伸展与收缩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Expand_Click(object sender, EventArgs e)
        {
            bool going_right = button_Expand.Text.Equals(right);
            textBox_Info.Visible = going_right;
            Width += expand_size * (going_right ? 1 : -1);
            button_Expand.Text = going_right ? left : right;
        }

        bool _start = false;
        private readonly string _filePath = @"D:\Mei2\GpsInfoRecording\";
        //private string _fileName = string.Empty;
        private string _fullPath;

        private void Button_RecordCoors_Click(object sender, EventArgs e)
        {
            _start = button_RecordCoors.Text.Equals("记录开");
            button_RecordCoors.Text = _start ? "记录关" : "记录开";
            if (_start)
            {
                _fullPath = string.Format(@"{0}{1:yyyyMMddHHmmss}.csv", _filePath, DateTime.Now);
                if (!Directory.Exists(_filePath))
                    Directory.CreateDirectory(_filePath);
                File.AppendAllLines(_fullPath, new string[] { "time,left_front,left_middle,left_back,right_front,right_middle,right_back" });
            }
        }

        private void Timer_RecordCoor_Tick(object sender, EventArgs e)
        {
            if (_start)
                File.AppendAllLines(_fullPath, new string[] { string.Format("{0:yyyy-MM-dd HH:mm:ss.fff},{1},{2},{3},{4},{5},{6}", DateTime.Now, BaseConst.GnssInfo.Longitude, BaseConst.GnssInfo.Latitude, BaseConst.GnssInfo.Altitude, BaseConst.GnssInfo.LocalCoor_Ante.ToString("default").Trim(' '), BaseConst.GnssInfo.LocalCoor_Ante.ToString("prime").Trim(' '), BaseConst.GnssInfo.PitchAngle, BaseConst.GnssInfo.YawAngle, BaseConst.GnssInfo.WalkingPosition, BaseConst.GnssInfo.LocalCoor_Tip.ToString("prime").Trim(' '), BaseConst.GnssInfo.LocalCoor_PitchAxis.ToString("prime").Trim(' '), BaseConst.GnssInfo.LocalCoor_YawAxis.ToString("prime").Trim(' '), BaseConst.GnssInfo.LocalCoor_YawAxis.ToString("claimer").Trim(' ')) });
        }

        private void Button_UiUpdate_Click(object sender, EventArgs e)
        {
            UpdateUI();
        }

        /// <summary>
        /// 复制到粘贴板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_CopyToClipboard_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox_Info.Text))
                return;

            Clipboard.SetDataObject(textBox_Info.Text);
            Clipboard.SetText(textBox_Info.Text);
            label_Copied.Visible = true;
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(1500);
                label_Copied.SafeInvoke(() =>
                {
                    label_Copied.Visible = false;
                });
            }))
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// 订阅状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_IsGroupSubscribed_CheckedChanged(object sender, EventArgs e)
        {
            BaseConst.OpcTask.OpcHelper.IsGroupSubscribed = checkBox_IsGroupSubscribed.Checked;
        }

        /// <summary>
        /// 不敏感区
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_GroupsDeadband_ValueChanged(object sender, EventArgs e)
        {
            BaseConst.OpcTask.OpcHelper.GroupsDeadband = (float)numeric_GroupsDeadband.Value;
        }

        /// <summary>
        /// 更新速度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_GroupUpdateRate_ValueChanged(object sender, EventArgs e)
        {
            BaseConst.OpcTask.OpcHelper.GroupUpdateRate = (int)numeric_GroupUpdateRate.Value;
        }

        /// <summary>
        /// UI更新Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_UiUpdate_Tick(object sender, EventArgs e)
        {
            if (checkBox_UiUpdateRecur.Checked)
                UpdateUI();
        }

        /// <summary>
        /// OPC更新Timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_OpcUpdate_Tick(object sender, EventArgs e)
        {
            if (BaseConst.PostureMode == PostureMode.OPC)
                BaseConst.GnssInfo.GetTipCoordinatesByPostures();
        }

        private void Timer_Upload_Tick(object sender, EventArgs e)
        {
            timer_Upload.Interval = BaseConst.UploadInterval;
            UpdateBeidouInfo();
            byte[] array = ProtobufNetWrapper.SerializeToBytes(GnssProtoInfo, (int)ProtoInfoType.GNSS);
            tcpServerMain.SendData(array);
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
                    textBox_Command.Text = _commandStorage.Last();
                    break;
                case Keys.Down:
                    textBox_Command.Text = _commandStorage.Next();
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
                    if (!string.IsNullOrWhiteSpace(textBox_Command.Text) && tcpClient.IsStart)
                    {
                        SendDataEach(textBox_Command.Text);
                        textBox_Command.Text = string.Empty;
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
