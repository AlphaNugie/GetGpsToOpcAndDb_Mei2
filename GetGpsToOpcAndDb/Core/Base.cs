using CommonLib.Clients;
using CommonLib.Function;
using GetGpsToOpcAndDb.Model;
using GetGpsToOpcAndDb.WbService;
using Microsoft.SqlServer.Types;
using OPCAutomation;
using OpcLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Core
{
    /// <summary>
    /// 公共参数
    /// </summary>
    public struct BaseConst
    {
        /// <summary>
        /// 日志
        /// </summary>
        public static readonly LogClient Log = new LogClient("logs", "getgps", "executable_logs.txt", false, true);

        #region 正则表达式
        /// <summary>
        /// 提取NMEA0183 GNSS消息类型的正则表达式（如$GPGGA, 或 #BESTPOSA,）
        /// </summary>
        public static readonly string RegexPattern_MessageType = @"(\$|\#)[A-Za-z0-9]+\,";

        /// <summary>
        /// 提取完整NMEA0183 GNSS消息的正则表达式，查找由[$],[*]包裹并由2位16进制数结尾的字符串，中间不得包含其它[$],[*]
        /// 或查找由[#],[;],[*]包裹并由8位16进制数结尾的字符串，中间不得包含其它[#],[;],[*]
        /// </summary>
        public static readonly string RegexPattern_FullMessage = @"(\$[^$*]+\*[A-Fa-f0-9]{2})|(\#[^#;]+\;[^;*]+\*[A-Fa-f0-9]{8})";
        //public static readonly string RegexPattern_FullMessage = @"\$[^$*]+\*[A-Fa-f0-9]{2}";
        #endregion

        /// <summary>
        /// 刷新基础配置的线程
        /// </summary>
        public static Thread Thread_RefreshConfigs = new Thread(new ThreadStart(BaseFunc.RefreshConfigs)) { IsBackground = true };

        /// <summary>
        /// GPS信息实体类对象
        /// </summary>
        public static GnssInfoObject GnssInfo { get; set; }

        /// <summary>
        /// OPC工具
        /// </summary>
        public static OpcTask OpcTask { get; set; }

        /// <summary>
        /// OPC数据源
        /// </summary>
        public static OpcDataSource OpcDataSource { get; set; }

        /// <summary>
        /// INI文件操作工具
        /// </summary>
        public static readonly IniFileHelper IniHelper = new IniFileHelper("Config.ini");

        /// <summary>
        /// 定位定向消息(bestposa/gpgga/gphdt)发送时间间隔，可接受的值为0.05/0.1/0.2/0.5/任意正整数，假如不在这些值的范围内，则自动向下匹配（最低匹配到0.05）
        /// </summary>
        public static double ReceiveInterval { get; set; }

        /// <summary>
        /// 通过TcpServer向外发送定位数据的速率（毫秒，默认为1000）
        /// </summary>
        public static int UploadInterval { get; set; }

        /// <summary>
        /// WebService的IP地址
        /// </summary>
        public static readonly string ServiceIp = IniHelper.ReadData("WebService", "ServiceIp");

        /// <summary>
        /// WebService的端口
        /// </summary>
        public static readonly ushort ServicePort = ushort.Parse(IniHelper.ReadData("WebService", "ServicePort"));

        /// <summary>
        /// WebService全地址
        /// </summary>
        public static readonly string ServiceAddress = string.Format(IniHelper.ReadData("WebService", "AddressFormat"), ServiceIp, ServicePort);

        /// <summary>
        /// WebService操作类对象
        /// 如何配置服务地址
        /// 1、找到App.config文件中的system.ServiceModel=>client=>endpoint节点的address属性，根据需要修改
        /// 2、利用WebService构造器的重载方法，在Config.ini文件中修改，重载方法的endpointConfigurationName参数：App.config文件中的system.ServiceModel=>client=>endpoint节点的name属性值
        /// </summary>
        public static readonly WbServicePortTypeClient WebClient = new WbServicePortTypeClient("IWbServiceHttpPort", ServiceAddress);

        ///// <summary>
        ///// 俯仰角度获取模式
        ///// </summary>
        //public static PitchAngleMode PitchAngleMode { get; set; }

        /// <summary>
        /// 单机姿态获取模式，默认从北斗获取
        /// </summary>
        public static PostureMode PostureMode { get; set; }

        #region Sqlite
        private static string _sqlite_file_dir = string.Empty;
        /// <summary>
        /// Sqlite文件路径，可为相对路径
        /// </summary>
        public static string SqliteFileDir
        {
            get { return _sqlite_file_dir; }
            set { _sqlite_file_dir = string.IsNullOrWhiteSpace(value) || value.Contains("not found") ? string.Empty : value; }
        }

        private static string _sqlite_file_name = "base.db";
        /// <summary>
        /// Sqlite文件名称，包括后缀
        /// </summary>
        public static string SqliteFileName
        {
            get { return _sqlite_file_name; }
            set { _sqlite_file_name = string.IsNullOrWhiteSpace(value) || value.Contains("not found") ? "base.db" : value; }
        }
        #endregion

        #region OPC
        /// <summary>
        /// OPC是否自动连接
        /// </summary>
        public static bool OpcAutoConnect { get; set; }

        /// <summary>
        /// OPC SERVER IP地址
        /// </summary>
        public static string OpcServerIp { get; set; }

        /// <summary>
        /// OPC SERVER 名称
        /// </summary>
        public static string OpcServerName { get; set; }

        /// <summary>
        /// OPC更新速率（毫秒）
        /// </summary>
        public static int OpcUpdateRate { get; set; }

        ///// <summary>
        ///// OPC数值更新的系数，默认为1
        ///// </summary>
        //public static int OpcUpdateRatio = 1;

        /// <summary>
        /// OPC功能接口
        /// </summary>
        //public static OpcUtilHelper OpcHelper = new OpcUtilHelper(int.Parse(BaseConst.IniHelper.ReadData("Opc", "OpcUpdateRate")), false);
        //public static OpcUtilHelper OpcHelper = new OpcUtilHelper(int.Parse(BaseConst.IniHelper.ReadData("Opc", "OpcUpdateRate")), true);

        ///// <summary>
        ///// 供读取的OPC组信息
        ///// </summary>
        //public static OpcGroupInfo OpcRead { get; set; }

        ///// <summary>
        ///// 供写入的OPC组信息
        ///// </summary>
        //public static OpcGroupInfo OpcWrite { get; set; }

        ///// <summary>
        ///// OPC写入项：经度
        ///// </summary>
        //public static OpcItemInfo OpcWriteLongitude { get; set; }

        ///// <summary>
        ///// OPC写入项：俯仰
        ///// </summary>
        //public static OpcItemInfo OpcWritePitch { get; set; }

        ///// <summary>
        ///// OPC写入项：回转
        ///// </summary>
        //public static OpcItemInfo OpcWriteYaw { get; set; }

        ///// <summary>
        ///// OPC写入项：纬度
        ///// </summary>
        //public static OpcItemInfo OpcWriteLatitude { get; set; }

        ///// <summary>
        ///// OPC写入项：海拔
        ///// </summary>
        //public static OpcItemInfo OpcWriteAltitude { get; set; }

        //public static OpcItemInfo OpcWriteRandom { get; set; }

        ///// <summary>
        ///// OPC读取项：俯仰角
        ///// </summary>
        //public static OpcItemInfo OpcReadPitch { get; set; }
        #endregion

        #region 坐标转换
        /// <summary>
        /// 是否启用转换（经纬度转换为本地坐标）
        /// </summary>
        public static bool ConvertEnabled { get; set; }

        /// <summary>
        /// 坐标转换方法类型：1 原生方法，2 外来方法（默认为1）
        /// </summary>
        public static int ConvertMethodType { get; set; }

        /// <summary>
        /// XY坐标轴坐标值系数、偏移量数组，长度为4，分别代表X系数,X偏移量,Y系数,Y偏移量
        /// </summary>
        public static double[] AxisRatios = new double[] { 1, 0, 1, 0 };

        ///// <summary>
        ///// XY坐标轴坐标值偏移量
        ///// </summary>
        //public static double[] AxisValueOffset = new double[] { 0, 0 };

        /// <summary>
        /// 是否交换XY轴
        /// </summary>
        public static bool AxisSwapped = false;

        /// <summary>
        /// 本地原点的经纬度、海拔
        /// </summary>
        public static Position GroundZeroPosition = new Position();

        /// <summary>
        /// 大机轨道起点的经纬度、海拔
        /// </summary>
        public static Position TrackPosition = new Position();

        /// <summary>
        /// 大机轨道起点的本地坐标
        /// </summary>
        public static Coordinate TrackCoordinate = new Coordinate();

        /// <summary>
        /// WGS84大地坐标系的ID
        /// </summary>
        public static int SystemId = 4326;

        /// <summary>
        /// 真北到本地北的夹角，单位°（逆时针为正，顺时针为负）
        /// </summary>
        public static double LocalNorthingRotated { get; set; }
        #endregion

        #region 坐标转换（PLC模式）
        /// <summary>
        /// 行走为0时大臂回转轴心的X坐标
        /// </summary>
        public static double TrackOriginX { get; set; }

        /// <summary>
        /// 行走为0时大臂回转轴心的Y坐标
        /// </summary>
        public static double TrackOriginY { get; set; }

        /// <summary>
        /// 行走为0时大臂回转轴心的Z坐标
        /// </summary>
        public static double TrackOriginZ { get; set; }
        #endregion

        #region 姿态
        /// <summary>
        /// 航向角校正值（°）
        /// </summary>
        public static double HeadingOffset { get; set; }

        /// <summary>
        /// 行走位置校正值（米）
        /// </summary>
        public static double WalkingOffset { get; set; }

        /// <summary>
        /// 俯仰角校正值（°）
        /// </summary>
        public static double PitchOffset { get; set; }

        /// <summary>
        /// 回转角校正值（°）
        /// </summary>
        public static double YawOffset { get; set; }

        private static double height_zero_ante = 0;
        /// <summary>
        /// 大臂平放时接收机的海拔高度（米）
        /// </summary>
        public static double HeightZero_Ante
        {
            get { return height_zero_ante; }
            set
            {
                height_zero_ante = value;
                HeightZero = height_zero_ante - ante_raised_height;
            }
        }

        private static double ante_raised_height = 0;
        /// <summary>
        /// 定位天线被抬起的高度（米，距离大臂所在平面的垂直距离）
        /// </summary>
        public static double AnteRaisedHeight
        {
            get { return ante_raised_height; }
            set
            {
                ante_raised_height = value;
                HeightZero = height_zero_ante - ante_raised_height;
                PitchAngleRadian_AnteRedun = Math.Atan(ante_raised_height / dist_pitch_axis);
                Dist2PitchAxis_Real = Math.Sqrt(Math.Pow(ante_raised_height, 2) + Math.Pow(dist_pitch_axis, 2));
            }
        }

        private static double ante_dist_sym = 0;
        /// <summary>
        /// 定位天线距离大臂对称轴偏移的距离（右侧为正）
        /// </summary>
        public static double AnteDist2SymAxis
        {
            get { return ante_dist_sym; }
            set
            {
                ante_dist_sym = value;
                YawAngleRadian_AnteRedun = Math.Atan(ante_dist_sym / (dist_pitch_axis + dist_pitch_yaw_axis));
            }
        }

        /// <summary>
        /// 俯仰轴海拔高度（用大臂平放时天线海拔高度与天线被抬起高度作差）
        /// </summary>
        public static double HeightZero { get; private set; }

        private static double dist_pitch_axis = 0;
        /// <summary>
        /// 定位天线在大臂方向距俯仰轴的距离
        /// </summary>
        public static double Dist2PitchAxis
        {
            get { return dist_pitch_axis; }
            set
            {
                dist_pitch_axis = value;
                PitchAngleRadian_AnteRedun = Math.Atan(ante_raised_height / dist_pitch_axis);
                YawAngleRadian_AnteRedun = Math.Atan(ante_dist_sym / (dist_pitch_axis + dist_pitch_yaw_axis));
                Dist2PitchAxis_Real = Math.Sqrt(Math.Pow(ante_raised_height, 2) + Math.Pow(dist_pitch_axis, 2));
            }
        }

        /// <summary>
        /// 定位天线到大臂俯仰轴实际距离（直线距离）
        /// </summary>
        public static double Dist2PitchAxis_Real { get; private set; }

        private static double dist_pitch_yaw_axis = 0;
        /// <summary>
        /// 俯仰轴到回转轴的距离（米）
        /// </summary>
        public static double DistPitch2YawAxis
        {
            get { return dist_pitch_yaw_axis; }
            set
            {
                dist_pitch_yaw_axis = value;
                YawAngleRadian_AnteRedun = Math.Atan(ante_dist_sym / (dist_pitch_axis + dist_pitch_yaw_axis));
            }
        }

        private static double dist_tip_pitch_h = 0;
        /// <summary>
        /// 定位天线距臂架顶端的水平距离
        /// </summary>
        public static double DistTip2PitchAxisHor
        {
            get { return dist_tip_pitch_h; }
            set
            {
                dist_tip_pitch_h = value;
                PitchAngleRadian_TipRedun = Math.Atan(dist_tip_pitch_v / dist_tip_pitch_h);
                DistTip2PitchAxis_Real = Math.Sqrt(Math.Pow(dist_tip_pitch_v, 2) + Math.Pow(dist_tip_pitch_h, 2));
            }
        }

        private static double dist_tip_pitch_v = 0;
        /// <summary>
        /// 定位天线距臂架顶端的竖直距离
        /// </summary>
        public static double DistTip2PitchAxisVer
        {
            get { return dist_tip_pitch_v; }
            set
            {
                dist_tip_pitch_v = value;
                PitchAngleRadian_TipRedun = Math.Atan(dist_tip_pitch_v / dist_tip_pitch_h);
                DistTip2PitchAxis_Real = Math.Sqrt(Math.Pow(dist_tip_pitch_v, 2) + Math.Pow(dist_tip_pitch_h, 2));
            }
        }

        /// <summary>
        /// 大臂顶端到大臂俯仰轴实际距离（直线距离）
        /// </summary>
        public static double DistTip2PitchAxis_Real { get; private set; }

        /// <summary>
        /// 是否沿本地南北行走（目前设定恒定为true）（否则沿本地东西行走）
        /// </summary>
        public static bool WalkingNorth = true;

        /// <summary>
        /// 大臂俯仰角（弧度）的定位天线高度冗余部分
        /// </summary>
        public static double PitchAngleRadian_AnteRedun { get; private set; }

        /// <summary>
        /// 大臂回转角（弧度）的定位天线距大臂对称轴偏移量冗余部分（向右侧偏移为正）
        /// </summary>
        public static double YawAngleRadian_AnteRedun { get; private set; }

        /// <summary>
        /// 根据定位天线得到的大臂俯仰角（弧度，定位天线、俯仰轴所在直线与地平面的夹角）
        /// </summary>
        public static double PitchAngleAnteRadian { get; set; }

        /// <summary>
        /// 大臂俯仰角（弧度）的大臂顶端冗余部分
        /// </summary>
        public static double PitchAngleRadian_TipRedun { get; private set; }

        /// <summary>
        /// 根据大臂顶端得到的大臂俯仰角（弧度，大臂顶端、俯仰轴所在直线与地平面的夹角）
        /// </summary>
        public static double PitchAngleTipRadian { get; set; }
        #endregion
    }

    /// <summary>
    /// 公共方法
    /// </summary>
    public static class BaseFunc
    {
        /// <summary>
        /// 配置文件初始化
        /// </summary>
        public static void InitConfigs()
        {
            BaseConst.Thread_RefreshConfigs.Start();
            try
            {
                BaseConst.GnssInfo = new GnssInfoObject();
                BaseConst.OpcTask = new OpcTask();
                BaseConst.OpcDataSource = new OpcDataSource();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 根据输入数值匹配定位消息发送时间间隔，可接受的值为0.05/0.1/0.2/0.5/任意正整数，假如不在这些值的范围内，则自动向下匹配（最低匹配到0.05）
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double GetMatchedReceiveInterval(double input)
        {
            if (input >= 1)
                input = Math.Floor(input);
            else if (input >= 0.5)
                input = 0.5;
            else if (input >= 0.2)
                input = 0.2;
            else if (input >= 0.1)
                input = 0.1;
            else
                input = 0.05;
            return input;
        }

        /// <summary>
        /// 持续刷新基础配置
        /// </summary>
        public static void RefreshConfigs()
        {
            short interval = 1000;
            double x = 0, y = 0, z = 0;
            while (true)
            {
                try
                {
                    int postureMode, methodType, uploadInterval;
                    //BaseConst.PitchAngleMode = (PitchAngleMode)int.Parse(BaseConst.IniHelper.ReadData("Main", "PitchAngleMode"));
                    BaseConst.PostureMode = int.TryParse(BaseConst.IniHelper.ReadData("Main", "PostureMode"), out postureMode) ? (PostureMode)postureMode : PostureMode.Beidou;
                    //BaseConst.PostureMode = (PostureMode)int.Parse(BaseConst.IniHelper.ReadData("Main", "PostureMode"));
                    BaseConst.ReceiveInterval = GetMatchedReceiveInterval(double.Parse(BaseConst.IniHelper.ReadData("Main", "ReceiveInterval")));
                    BaseConst.UploadInterval = int.TryParse(BaseConst.IniHelper.ReadData("Main", "UploadInterval"), out uploadInterval) ? uploadInterval : 1000; //通过TcpServer向外发送定位数据的速率（毫秒，默认为1000）

                    #region Sqlite
                    BaseConst.SqliteFileDir = BaseConst.IniHelper.ReadData("Sqlite", "FileDir");
                    BaseConst.SqliteFileName = BaseConst.IniHelper.ReadData("Sqlite", "FileName");
                    #endregion
                    #region OPC
                    BaseConst.OpcAutoConnect = BaseConst.IniHelper.ReadData("Opc", "OpcAutoConnect").Equals("1");
                    BaseConst.OpcServerIp = BaseConst.IniHelper.ReadData("Opc", "OpcServerIp");
                    BaseConst.OpcServerName = BaseConst.IniHelper.ReadData("Opc", "OpcServerName");
                    BaseConst.OpcUpdateRate = int.Parse(BaseConst.IniHelper.ReadData("Opc", "OpcUpdateRate"));
                    BaseConst.OpcTask.Interval = BaseConst.OpcUpdateRate;
                    //BaseConst.OpcUpdateRatio = int.Parse(BaseConst.IniHelper.ReadData("Opc", "OpcUpdateRatio"));
                    #endregion
                    #region 转换
                    BaseConst.ConvertEnabled = BaseConst.IniHelper.ReadData("Conversion", "ConvertEnabled").Equals("1");
                    BaseConst.ConvertMethodType = int.TryParse(BaseConst.IniHelper.ReadData("Conversion", "ConvertMethodType"), out methodType) ? methodType : 1; //坐标转换方法类型
                    BaseConst.GroundZeroPosition.Update(double.Parse(BaseConst.IniHelper.ReadData("Conversion", "GroundZeroLatitude")), double.Parse(BaseConst.IniHelper.ReadData("Conversion", "GroundZeroLongitude")), double.Parse(BaseConst.IniHelper.ReadData("Conversion", "GroundZeroAltitude")));
                    BaseConst.TrackPosition.Update(double.Parse(BaseConst.IniHelper.ReadData("Conversion", "TrackLatitude")), double.Parse(BaseConst.IniHelper.ReadData("Conversion", "TrackLongitude")), double.Parse(BaseConst.IniHelper.ReadData("Conversion", "TrackAltitude")));
                    BaseConst.LocalNorthingRotated = double.Parse(BaseConst.IniHelper.ReadData("Conversion", "LocalNorthingRotated"));
                    #endregion
                    #region 转换（PLC模式）
                    double trackOriginx, trackOriginy, trackOriginz;
                    BaseConst.TrackOriginX = double.TryParse(BaseConst.IniHelper.ReadData("ConversionPlc", "TrackOriginX"), out trackOriginx) ? trackOriginx : 0;
                    BaseConst.TrackOriginY = double.TryParse(BaseConst.IniHelper.ReadData("ConversionPlc", "TrackOriginY"), out trackOriginy) ? trackOriginy : 0;
                    BaseConst.TrackOriginZ = double.TryParse(BaseConst.IniHelper.ReadData("ConversionPlc", "TrackOriginZ"), out trackOriginz) ? trackOriginz : 0;
                    #endregion
                    #region 姿态
                    BaseConst.AxisRatios = BaseConst.IniHelper.ReadData("Conversion", "AxisValueExp").Split(',', 'm').Select(p => double.Parse(p.Equals("-") || string.IsNullOrWhiteSpace(p) ? p + "1" : p)).ToArray();
                    BaseConst.AxisSwapped = BaseConst.IniHelper.ReadData("Conversion", "AxisSwapped").Equals("1");
                    BaseConst.HeadingOffset = double.Parse(BaseConst.IniHelper.ReadData("Posture", "HeadingOffset"));
                    BaseConst.WalkingOffset = double.Parse(BaseConst.IniHelper.ReadData("Posture", "WalkingOffset"));
                    BaseConst.PitchOffset = double.Parse(BaseConst.IniHelper.ReadData("Posture", "PitchOffset"));
                    BaseConst.YawOffset = double.Parse(BaseConst.IniHelper.ReadData("Posture", "YawOffset"));
                    BaseConst.HeightZero_Ante = double.Parse(BaseConst.IniHelper.ReadData("Posture", "HeightZero_Ante"));
                    BaseConst.AnteRaisedHeight = double.Parse(BaseConst.IniHelper.ReadData("Posture", "AnteRaisedHeight"));
                    BaseConst.Dist2PitchAxis = double.Parse(BaseConst.IniHelper.ReadData("Posture", "Dist2PitchAxis"));
                    BaseConst.DistPitch2YawAxis = double.Parse(BaseConst.IniHelper.ReadData("Posture", "DistPitch2YawAxis"));
                    BaseConst.DistTip2PitchAxisHor = double.Parse(BaseConst.IniHelper.ReadData("Posture", "DistTip2PitchAxisHor"));
                    BaseConst.DistTip2PitchAxisVer = double.Parse(BaseConst.IniHelper.ReadData("Posture", "DistTip2PitchAxisVer"));
                    BaseConst.AnteDist2SymAxis = double.Parse(BaseConst.IniHelper.ReadData("Posture", "Dist2SymAxis"));
                    //BaseConst.WalkingNorth = BaseConst.IniHelper.ReadData("Posture", "WalkingNorth").Equals("1");
                    #endregion

                    GetCoordinates(BaseConst.TrackPosition.Latitude, BaseConst.TrackPosition.Longitude, BaseConst.TrackPosition.Altitude, ref x, ref y, ref z);
                    //BaseConst.TrackCoordinate.Update(x, y, BaseConst.TrackPosition.Altitude);
                    BaseConst.TrackCoordinate.Update(x, y, z);
                }
                catch (Exception) { }

                Thread.Sleep(interval);
            }
        }

        #region 坐标转换
        /// <summary>
        /// 通过指定的坐标转换方法，以给定的原点经纬度、较WGS84坐标系的旋转角度为本地坐标系基础数据，将给定的另外一组经纬度转换为本地坐标系坐标值
        /// </summary>
        /// <param name="methodType">坐标转换方法类型：1 原生方法，2 外来方法</param>
        /// <param name="lat">待转换点的纬度</param>
        /// <param name="lon">待转换点的经度</param>
        /// <param name="alt">待转换点的海拔</param>
        /// <param name="x">转换后的本地坐标X</param>
        /// <param name="y">转换后的本地坐标Y</param>
        /// <param name="z">转换后的本地坐标Z</param>
        public static void GetLocalCoordinates(int methodType, double lat, double lon, double alt, ref double x, ref double y, ref double z)
        {
            if (methodType == 1)
                GetCoordinates(lat, lon, alt, ref x, ref y, ref z);
            else if (methodType == 2)
                GpsChange(lat, lon, alt, ref x, ref y, ref z);
        }

        /// <summary>
        /// 通过配置文件中配置的坐标转换方法，以给定的原点经纬度、较WGS84坐标系的旋转角度为本地坐标系基础数据，将给定的另外一组经纬度转换为本地坐标系坐标值
        /// </summary>
        /// <param name="lat">待转换点的纬度</param>
        /// <param name="lon">待转换点的经度</param>
        /// <param name="alt">待转换点的海拔</param>
        /// <param name="x">转换后的本地坐标X</param>
        /// <param name="y">转换后的本地坐标Y</param>
        /// <param name="z">转换后的本地坐标Z</param>
        public static void GetLocalCoordinates(double lat, double lon, double alt, ref double x, ref double y, ref double z)
        {
            GetLocalCoordinates(BaseConst.ConvertMethodType, lat, lon, alt, ref x, ref y, ref z);
        }

        #region 原有方法
        /// <summary>
        /// 以给定的原点经纬度、较WGS84坐标系的旋转角度为本地坐标系基础数据，将给定的另外一组经纬度转换为本地坐标系坐标值
        /// </summary>
        /// <param name="start_lat">原点纬度</param>
        /// <param name="start_lon">原点经度</param>
        /// <param name="start_alt">原点海拔</param>
        /// <param name="rotate">真北到本地北的角度</param>
        /// <param name="lat">待转换点的纬度</param>
        /// <param name="lon">待转换点的经度</param>
        /// <param name="alt">待转换点的海拔</param>
        /// <param name="x">转换后的本地坐标X</param>
        /// <param name="y">转换后的本地坐标Y</param>
        /// <param name="z">转换后的本地坐标Z</param>
        public static void GetCoordinates(double start_lat, double start_lon, double start_alt, double rotate, double lat, double lon, double alt, ref double x, ref double y, ref double z)
        {
            SqlGeography zero = SqlGeography.Point(start_lat, start_lon, BaseConst.SystemId); //原点坐标点
            //计算经度距离差，保持纬度相同，经度不同
            //两个坐标点的媒介，具有一个点的纬度以及另一个点的经度
            SqlGeography medium = SqlGeography.Point(start_lat, lon, BaseConst.SystemId), target = SqlGeography.Point(lat, lon, BaseConst.SystemId);
            //m为经度距离差，n为纬度距离差
            double m = medium.STDistance(zero).Value * Math.Sign(lon - start_lon),
                   n = medium.STDistance(target).Value * Math.Sign(lat - start_lat);
            double radrot = rotate * Math.PI / 180, cosrot = Math.Cos(radrot), sinrot = Math.Sin(radrot);
            //本地坐标
            x = m * cosrot + n * sinrot;
            y = n * cosrot - m * sinrot;
            z = alt - start_alt;
        }

        /// <summary>
        /// 以给定的原点经纬度、较WGS84坐标系的旋转角度为本地坐标系基础数据，将给定的另外一组经纬度转换为本地坐标系坐标值
        /// </summary>
        /// <param name="lat">待转换点的纬度</param>
        /// <param name="lon">待转换点的经度</param>
        /// <param name="alt">待转换点的海拔</param>
        /// <param name="x">转换后的本地坐标X</param>
        /// <param name="y">转换后的本地坐标Y</param>
        /// <param name="z">转换后的本地坐标Z</param>
        public static void GetCoordinates(double lat, double lon, double alt, ref double x, ref double y, ref double z)
        {
            GetCoordinates(BaseConst.GroundZeroPosition.Latitude, BaseConst.GroundZeroPosition.Longitude, BaseConst.GroundZeroPosition.Altitude, BaseConst.LocalNorthingRotated, lat, lon, alt, ref x, ref y, ref z);
        }
        #endregion

        #region 外来方法
        /// <summary>
        /// 经纬度转为WGS84坐标系坐标
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="altitude">海拔</param>
        /// <param name="x">待输出的X坐标</param>
        /// <param name="y">待输出的Y坐标</param>
        /// <param name="z">待输出的Z坐标</param>
        public static void GpsPoint2DescartesPoint(double latitude, double longitude, double altitude, out double x, out double y, out double z)
        {
            //WGS84 地球坐标系常数
            double wgs84a = 6378.137; //椭球长半径
            double wgs84f = 1.0 / 298.257223563; //扁率（扁轴）
            double wgs84b = wgs84a * (1.0 - wgs84f); //椭球短半径
            double eccsq = 1 - Math.Pow(wgs84b, 2) / Math.Pow(wgs84a, 2); //第一偏心率的平方
            //double ecc = Math.Sqrt(eccsq); //第一偏心率

            double dtr = Math.PI / 180.0; //转换为弧度的系数
            double clat = Math.Cos(dtr * latitude), slat = Math.Sin(dtr * latitude);
            double clon = Math.Cos(dtr * longitude), slon = Math.Sin(dtr * longitude);
            double dsq = 1.0 - eccsq * Math.Pow(slat, 2), d = Math.Sqrt(dsq);
            double rn = wgs84a / d;

            x = (rn + altitude) * clat * clon;
            y = (rn + altitude) * clat * slon;
            z = ((1 - eccsq) * rn + altitude) * slat;
        }

        /// <summary>
        /// 以给定的经纬度海拔作为原点，将另外一组经纬度海拔转换为本地XYZ坐标并（逆时针）旋转指定角度
        /// </summary>
        /// <param name="start_lat">原点纬度</param>
        /// <param name="start_lon">原点经度</param>
        /// <param name="start_alt">原点海拔</param>
        /// <param name="rotate">转换后的旋转角度（逆时针为正）</param>
        /// <param name="lat">待转换纬度</param>
        /// <param name="lon">待转换经度</param>
        /// <param name="alt">待转换海拔</param>
        /// <param name="x">待输出X坐标</param>
        /// <param name="y">待输出Y坐标</param>
        /// <param name="z">待输出Z坐标</param>
        public static void GpsChange(double start_lat, double start_lon, double start_alt, double rotate, double lat, double lon, double alt, ref double x, ref double y, ref double z)
        {
            #region 废弃
            //double x0, y0, z0;
            //GpsPoint2DescartesPoint(start_lat, start_lon, start_alt, out x0, out y0, out z0);
            //double x1, y1, z1;
            //GpsPoint2DescartesPoint(lat, lon, alt, out x1, out y1, out z1);

            //x1 -= x0;
            //y1 -= y0;
            //z1 -= z0;
            //double da = rotate * Math.PI / 180.0; //旋转角度
            //x = x1 * Math.Sin(da) + y1 * Math.Cos(da);
            //y = x1 * Math.Cos(da) - y1 * Math.Sin(da);
            //z = z1;
            #endregion

            double x1 = (lat - start_lat) * 111000;
            double y1 = (lon - start_lon) * 111449 * Math.Cos(lat * Math.PI / 180.0);
            double da = rotate * Math.PI / 180.0; //旋转角度
            double x3 = x1 * Math.Cos(da) - y1 * Math.Sin(da);
            double y3 = x1 * Math.Sin(da) + y1 * Math.Cos(da);
            x = y3;
            y = x3;
            z = alt - start_alt;
        }

        /// <summary>
        /// 以配置文件中的经纬度海拔作为原点，将另外一组经纬度海拔转换为本地XYZ坐标并（逆时针）旋转配置文件中设定的角度
        /// </summary>
        /// <param name="lat">待转换纬度</param>
        /// <param name="lon">待转换经度</param>
        /// <param name="alt">待转换海拔</param>
        /// <param name="x">待输出X坐标</param>
        /// <param name="y">待输出Y坐标</param>
        /// <param name="z">待输出Z坐标</param>
        public static void GpsChange(double lat, double lon, double alt, ref double x, ref double y, ref double z)
        {
            GpsChange(BaseConst.GroundZeroPosition.Latitude, BaseConst.GroundZeroPosition.Longitude, BaseConst.GroundZeroPosition.Altitude, BaseConst.LocalNorthingRotated, lat, lon, alt, ref x, ref y, ref z);
        }
        #endregion
        #endregion

        #region 消息验证
        #endregion

        /// <summary>
        /// 换算经纬度（DDmm.mm转换为DD.mmmm）
        /// </summary>
        /// <param name="temp">角度，DDmm.mm</param>
        /// <returns></returns>
        public static double GetDegree(string temp)
        {
            double d;
            double.TryParse(temp, out d);
            return GetDegree(d);
        }

        /// <summary>
        /// 换算经纬度（DDmm.mm转换为DD.mmmm）
        /// </summary>
        /// <param name="temp">角度，DDmm.mm</param>
        /// <returns></returns>
        public static double GetDegree(double temp)
        {
            int m = (int)temp / 100;
            double n = (temp - m * 100) / 60;
            n += m;
            return n;
        }
    }

    /// <summary>
    /// 俯仰角度获取模式
    /// </summary>
    public enum PitchAngleMode
    {
        /// <summary>
        /// OPC
        /// </summary>
        OPC = 1,

        /// <summary>
        /// 天线高度
        /// </summary>
        AntennaHeight = 2
    }

    /// <summary>
    /// 单机姿态获取模式
    /// </summary>
    public enum PostureMode
    {
        /// <summary>
        /// 从OPC读取，不向PLC写入
        /// </summary>
        OPC = 1,

        /// <summary>
        /// 从北斗获取，假如OPC连接则向PLC写入
        /// </summary>
        Beidou = 2
    }

    /// <summary>
    /// 连接模式
    /// </summary>
    public enum ConnectionMode
    {
        /// <summary>
        /// TCP客户端
        /// </summary>
        TCP = 1,

        /// <summary>
        /// UDP
        /// </summary>
        UDP = 2,

        /// <summary>
        /// TCP服务端
        /// </summary>
        TCPS = 3,

        /// <summary>
        /// HTTP监听服务
        /// </summary>
        HTTP = 4
    }
}
