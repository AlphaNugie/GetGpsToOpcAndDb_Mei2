using CommonLib.Enums;
using GetGpsToOpcAndDb.Core;
using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// GNSS信息实体类
    /// </summary>
    public class GnssInfoObject
    {
        #region 私有变量
        private Random random = new Random();
        private readonly Regex regex_type = new Regex(BaseConst.RegexPattern_MessageType, RegexOptions.Compiled); //提取消息类型的正则
        private readonly DataService_GnssInfo dataService = new DataService_GnssInfo();
        private double _latitude, _longitude, _altitude, _pitch_angle;
        #endregion

        #region 属性
        #region 基础属性
        /// <summary>
        /// 错误信息键值对：GNSS/WebService/OPC/DataService
        /// </summary>
        public Dictionary<string, string> DictErrorMessages { get; set; }

        /// <summary>
        /// 大机ID
        /// </summary>
        public string ClaimerId { get; set; }

        /// <summary>
        /// GNSS消息类型
        /// </summary>
        public GnssMessageType MessageType { get; set; }

        /// <summary>
        /// RTK低延迟定位数据
        /// </summary>
        public Rtkpos RTKPOS { get; set; }

        /// <summary>
        /// 接收机的时间，位置和定位相关数据
        /// </summary>
        public Gpgga GPGGA { get; set; }

        /// <summary>
        /// 接收机计算的时间、日期、位置、航向和速度信息
        /// </summary>
        public Gprmc GPRMC { get; set; }

        /// <summary>
        /// 包含以度为单位相对真北方向的航向信息
        /// </summary>
        public Gphdt GPHDT { get; set; }
        #endregion

        #region 通用
        /// <summary>
        /// 定位质量
        /// </summary>
        public string PositionQuality { get; set; }

        /// <summary>
        /// 位置对应的UTC事件，格式：hhmmss.ss
        /// </summary>
        public string Utc { get; set; }

        /// <summary>
        /// 纬度（DDmm.mm）
        /// </summary>
        public double Latitude
        {
            get { return this._latitude; }
            set
            {
                this._latitude = Math.Round(value, 8);
            }
        }

        /// <summary>
        /// 纬度方向（N = 北，S =南）
        /// </summary>
        public string LatitudeDirection { get; set; }

        /// <summary>
        /// 经度（DDDmm.mm）
        /// </summary>
        public double Longitude
        {
            get { return this._longitude; }
            set
            {
                this._longitude = Math.Round(value, 8);
            }
        }

        /// <summary>
        /// 经度方向（E = 东，W = 西）
        /// </summary>
        public string LongitudeDirection { get; set; }

        private double trackdir_truenorth = 0;
        /// <summary>
        /// 真北航迹方向（到真北的角度，单位度，赋值前进行校正，偏东为正，偏西为负）
        /// </summary>
        public double TrackDirection_TrueNorth
        {
            get { return this.trackdir_truenorth; }
            set
            {
                double temp = value + BaseConst.HeadingOffset; //航向角加校正
                this.trackdir_truenorth = Math.Round(temp > 180 ? temp - 360 : temp, 4); //大于180则减360
                this.YawAngle = this.trackdir_truenorth + BaseConst.LocalNorthingRotated;
            }
        }

        /// <summary>
        /// 位置信息解出类型
        /// </summary>
        public SolutionStatus SolutionStatus { get; set; }

        /// <summary>
        /// 位置或速度类型
        /// </summary>
        public PositionVelocityType PositionType { get; set; }

        /// <summary>
        /// 天线高度，高于/低于平均海平面，单位：米
        /// </summary>
        public double Altitude
        {
            get { return this._altitude; }
            set
            {
                this._altitude = value;
                //当俯仰角度获取模式为天线高度时：计算大臂俯仰角，其正弦为(海拔提升高度/安装位置距俯仰轴距离)，然后再减去因为天线抬起所造成的角度冗余
                if (BaseConst.PitchAngleMode == PitchAngleMode.AntennaHeight)
                {
                    BaseConst.PitchAngleAnteRadian = Math.Asin((this._altitude - BaseConst.HeightZero) / BaseConst.Dist2PitchAxis_Real);
                    BaseConst.PitchAngleTipRadian = BaseConst.PitchAngleAnteRadian - BaseConst.PitchAngleRadian_AnteRedun + BaseConst.PitchAngleRadian_TipRedun;
                    this.PitchAngle = Math.Round((BaseConst.PitchAngleAnteRadian - BaseConst.PitchAngleRadian_AnteRedun) * 180 / Math.PI, 2);
                }
            }
        }

        /// <summary>
        /// 大臂俯仰角
        /// </summary>
        public double PitchAngle
        {
            get { return this._pitch_angle; }
            set
            {
                this._pitch_angle = value;
            }
        }

        /// <summary>
        /// 回转角（到本地北的角度，单位度）
        /// </summary>
        public double YawAngle { get; set; }

        /// <summary>
        /// 行走位置
        /// </summary>
        public double WalkingPosition { get; set; }
        #endregion

        #region 本地坐标
        private Coordinate local_coor_ante = new Coordinate();
        private Coordinate local_coor_pitch = new Coordinate();
        private Coordinate local_coor_yaw = new Coordinate();
        private Coordinate local_coor_tip = new Coordinate();
        /// <summary>
        /// 定位天线本地坐标，转换后XYZ分别指向东、北、上，不转换分别为经度、纬度、海拔
        /// </summary>
        public Coordinate LocalCoor_Ante
        {
            get { return this.local_coor_ante; }
            set { this.local_coor_ante = value; }
        }

        /// <summary>
        /// 俯仰轴本地坐标，转换后XYZ分别指向东、北、上，不转换分别为经度、纬度、海拔
        /// </summary>
        public Coordinate LocalCoor_PitchAxis
        {
            get { return this.local_coor_pitch; }
            set { this.local_coor_pitch = value; }
        }

        /// <summary>
        /// 回转轴本地坐标，转换后XYZ分别指向东、北、上，不转换分别为经度、纬度、海拔
        /// </summary>
        public Coordinate LocalCoor_YawAxis
        {
            get { return this.local_coor_yaw; }
            set { this.local_coor_yaw = value; }
        }

        /// <summary>
        /// 臂架顶端本地坐标，转换后XYZ分别指向东、北、上，不转换分别为经度、纬度、海拔
        /// </summary>
        public Coordinate LocalCoor_Tip
        {
            get { return this.local_coor_tip; }
            set { this.local_coor_tip = value; }
        }
        #endregion

        #region GPGGA
        /// <summary>
        /// GPS质量指示符
        /// </summary>
        public GpsQuality Quality { get; set; }

        /// <summary>
        /// 使用中的卫星数。可能与所见数不一致
        /// </summary>
        public int SatelliteNumber { get; set; }

        /// <summary>
        /// 水平精度因子
        /// </summary>
        public double HorizontalDOP { get; set; }

        /// <summary>
        /// 大地水准面差距（米）–大地水准面和WGS84椭球面之间的距离。大地水准面高于椭球面为正值，否则，为负值
        /// </summary>
        public double Undulation { get; set; }

        /// <summary>
        /// 差分数据龄期，没有差分数据时为0，单位：秒
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 差分基站ID，0000-1023，没有差分数据时为00
        /// </summary>
        public string StationId { get; set; }
        #endregion

        #region GPHDT
        /// <summary>
        /// 真北
        /// </summary>
        public bool IsTrueNorth { get; set; }
        #endregion

        #region GPRMC
        /// <summary>
        /// 位置状态是否有效
        /// </summary>
        public bool IsPositionValid { get; set; }

        /// <summary>
        /// 地速，相对于地面的速度，单位：节
        /// </summary>
        public double GroundSpeed { get; set; }

        /// <summary>
        /// 日期，格式：ddmmyy
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 磁偏角，单位：度
        /// </summary>
        public double MagneticDeclination { get; set; }

        /// <summary>
        /// 磁偏角方向，E/W
        /// </summary>
        public string MagneticDeclinationDir { get; set; }

        /// <summary>
        /// 定位模式指示符：A 单点定位, D 差分定位, E 推算定位, M 用户输入, N 数据无效
        /// </summary>
        public string ModeIndicator { get; set; }
        #endregion

        /// <summary>
        /// 经度标签名称
        /// </summary>
        public string LongitudeItemId { get; set; }

        /// <summary>
        /// 纬度标签名称
        /// </summary>
        public string LatitudeItemId { get; set; }

        /// <summary>
        /// 高度标签名称
        /// </summary>
        public string AltitudeItemId { get; set; }

        /// <summary>
        /// 俯仰角度标签名称
        /// </summary>
        public string PitchItemId { get; set; }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="claimerId">大机ID</param>
        public GnssInfoObject()
        {
            this.DictErrorMessages = new Dictionary<string, string>() { { "GNSS", string.Empty }, { "WebService", string.Empty }, { "OPC", string.Empty }, { "DataService", string.Empty } };
            this.MessageType = GnssMessageType.Unknown;
            this.RTKPOS = new Rtkpos(this);
            this.GPGGA = new Gpgga(this);
            this.GPRMC = new Gprmc(this);
            this.GPHDT = new Gphdt(this);
            this.LongitudeItemId = string.Empty;
            this.LatitudeItemId = string.Empty;
            this.AltitudeItemId = string.Empty;
        }

        #region 信息处理与分类
        /// <summary>
        /// 对所有消息按行分组并分类处理
        /// </summary>
        /// <param name="input"></param>
        public void Classify(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return;
            string[] messages = input.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var message in messages)
                this.ClassifyEach(message);

            //TODO 假如经纬度有任何一个为0，退出
            if (this.Latitude == 0 || this.Longitude == 0)
                return;
            this.GetLocalCoordinates();
            this.SaveInfo();
        }

        /// <summary>
        /// 对GNSS消息分类处理
        /// </summary>
        /// <param name="message"></param>
        public void ClassifyEach(string message)
        {
            string typeName = this.regex_type.Match(message).Value.Trim('$', ',', '#').ToUpper(); //GNSS消息类型，去除多余符号
            GnssMessageType type;
            //尝试转换为枚举，假如转换失败，标记为未知类型
            Enum.TryParse<GnssMessageType>(typeName, out type);
            if (!Enum.IsDefined(typeof(GnssMessageType), type))
                type = GnssMessageType.Unknown;
            this.MessageType = type;

            switch (this.MessageType)
            {
                //RTKPOS, MATCHEDPOS, PSRPOS与BESTPOS的结构一样，可以使用相同的解析方法
                case GnssMessageType.MATCHEDPOSA:
                case GnssMessageType.RTKPOSA:
                case GnssMessageType.PSRPOSA:
                case GnssMessageType.BESTPOSA:
                    this.RTKPOS.Analyze(ref message);
                    break;
                case GnssMessageType.GNGGA:
                case GnssMessageType.GPGGA:
                    this.GPGGA.Analyze(ref message);
                    break;
                case GnssMessageType.GPRMC:
                    this.GPRMC.Analyze(ref message);
                    break;
                case GnssMessageType.GNHDT:
                case GnssMessageType.GPHDT:
                    this.GPHDT.Analyze(ref message);
                    break;
                case GnssMessageType.Unknown:
                    return;
            }
        }
        #endregion

        #region 坐标转换
        /// <summary>
        /// 将一个坐标的经纬度转换为本地坐标
        /// </summary>
        public void GetLocalCoordinates()
        {
            GetLocalCoordinates(BaseConst.Dist2PitchAxis_Real, BaseConst.PitchAngleAnteRadian, this.YawAngle * Math.PI / 180, BaseConst.DistPitch2YawAxis, BaseConst.DistTip2PitchAxis_Real, BaseConst.PitchAngleTipRadian);
        }

        /// <summary>
        /// 将一个坐标的经纬度转换为本地坐标，指定定位天线距俯仰轴的距离与大臂俯仰角
        /// </summary>
        /// <param name="dist_pitch">定位天线距俯仰轴的实际距离（米）</param>
        /// <param name="dist_tip_pitch">大臂顶端距俯仰轴的实际距离（米）</param>
        /// <param name="dist_pitch_yaw">俯仰轴到回转轴的距离（米）</param>
        /// <param name="pitch_rad">根据定位天线得到的大臂俯仰角（弧度）</param>
        /// <param name="yaw_rad">大臂回转角（弧度）</param>
        public void GetLocalCoordinates(double dist_pitch, double pitch_rad, double yaw_rad, double dist_pitch_yaw, double dist_tip_pitch, double pitch_tip_rad)
        {
            double lat = this.Latitude, lon = this.Longitude, alt = this.Altitude;
            double x1, x2, x3, x4, y1, y2, y3, y4, z1, z2, z3, z4; //分别为定位天线坐标，俯仰轴坐标，回转轴坐标，臂架顶点坐标
            //double xc = 0, yc = 0; //回转轴在单机轨道坐标系下的坐标
            x1 = x2 = x3 = x4 = lon;
            y1 = y2 = y3 = y4 = lat;
            z1 = z2 = z3 = z4 = alt;
            if (!BaseConst.ConvertEnabled)
                goto Ending;

            BaseFunc.GetCoordinates(lat, lon, ref x1, ref y1);
            double cospitch = Math.Cos(pitch_rad), sinpitch = Math.Sin(pitch_rad), cosyaw = Math.Cos(yaw_rad), sinyaw = Math.Sin(yaw_rad);
            //俯仰轴本地坐标
            x2 = x1 - dist_pitch * cospitch * sinyaw;
            y2 = y1 - dist_pitch * cospitch * cosyaw;
            //y2 = y1 - BaseConst.Dist2SymAxis;
            //当大臂向上摆俯仰角为正时系数为-1，否则为1
            z2 = z1 + -1 * dist_pitch * sinpitch;
            //回转轴本地坐标
            x3 = x2 - dist_pitch_yaw * sinyaw;
            y3 = y2 - dist_pitch_yaw * cosyaw;
            z3 = z2;
            //臂架顶端本地坐标
            x4 = x2 + dist_tip_pitch * Math.Cos(pitch_tip_rad) * sinyaw;
            y4 = y2 + dist_tip_pitch * Math.Cos(pitch_tip_rad) * cosyaw;
            z4 = z2 + dist_tip_pitch * Math.Sin(pitch_tip_rad);
        Ending:
            this.LocalCoor_Ante.Update(x1, y1, z1);
            this.LocalCoor_PitchAxis.Update(x2, y2, z2);
            this.LocalCoor_YawAxis.Update(x3, y3, z3);
            this.LocalCoor_Tip.Update(x4, y4, z4);
            this.WalkingPosition = BaseConst.WalkingNorth == BaseConst.AxisSwapped ? this.LocalCoor_YawAxis.XClaimer : this.LocalCoor_YawAxis.YClaimer;
        }
        #endregion

        #region 信息保存与服务调用
        /// <summary>
        /// 保存信息（调用服务，写入OPC，保存到数据库等）
        /// </summary>
        public void SaveInfo()
        {
            this.WriteOpcValue();
            this.CallWebService();
            this.CallDataService();
        }

        /// <summary>
        /// 调用WebService
        /// </summary>
        public void CallWebService()
        {
            new Thread(new ThreadStart(() =>
            {
                string message = string.Empty;
                try
                {
                    if (BaseConst.WebClient.State != System.ServiceModel.CommunicationState.Opened)
                        BaseConst.WebClient.Open();
                    //根据大机编号更新t_gps_machines
                    //TODO 是否将此处经纬度改为本地XYZ坐标
                    if (!BaseConst.WebClient.save_gps_machines(this.ClaimerId, this.Longitude, this.Latitude))
                        message = string.Format("为ID为{0}的大机调用Web服务返回失败结果", this.ClaimerId);
                }
                catch (Exception e)
                {
                    //message = string.Format("为ID为{0}的大机调用Web服务时出错：{1}", this.ClaimerId, e.Message);
                    message = string.Format("为ID为{0}的大机调用Web服务时出错", this.ClaimerId);
                }
                this.DictErrorMessages["WebService"] = string.Format("{0} [{1:yyyy-MM-dd HH:mm:ss}]", message, DateTime.Now);
            }))
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// 将GNSS信息通过OPC写入
        /// </summary>
        public void WriteOpcValue()
        {
            //this.DictErrorMessages["OPC"] = string.Empty;
            new Thread(new ThreadStart(() =>
            {
                if (!BaseConst.OpcHelper.OpcConnected)
                    return;

                string message;
                try
                {
                    //向OPC写入本地坐标，假如设置为不转换为本地坐标，则写入经纬度，写入前乘以OPC数值更新系数
                    BaseConst.OpcWriteLongitude.Value = (this.LocalCoor_Ante.X * BaseConst.OpcUpdateRatio).ToString();
                    BaseConst.OpcWriteLatitude.Value = (this.LocalCoor_Ante.Y * BaseConst.OpcUpdateRatio).ToString();
                    BaseConst.OpcWriteAltitude.Value = (this.LocalCoor_Ante.Z * BaseConst.OpcUpdateRatio).ToString();
                    BaseConst.OpcWriteRandom.Value = (this.random.Next(1000)).ToString();
                    //TODO 假如回转角写入项不为空，修改待写入回转角的值
                    if (BaseConst.OpcWriteYaw != null)
                        BaseConst.OpcWriteYaw.Value = (this.TrackDirection_TrueNorth * BaseConst.OpcUpdateRatio).ToString();
                    if (BaseConst.PitchAngleMode == PitchAngleMode.AntennaHeight)
                        BaseConst.OpcWritePitch.Value = (this.PitchAngle * BaseConst.OpcUpdateRatio).ToString();
                    BaseConst.OpcWrite.WriteValues(out message);
                    //BaseConst.OpcHelper.WriteOpc(this.LongitudeItemId, this.X.ToString(), 1, out message);
                    //BaseConst.OpcHelper.WriteOpc(this.LatitudeItemId, this.Y.ToString(), 2, out message);
                    //BaseConst.OpcHelper.WriteOpc(this.AltitudeItemId, this.Z.ToString(), 3, out message);
                    this.DictErrorMessages["OPC"] = message;
                }
                catch (Exception ex)
                {
                    message = "向OPC组写入经纬度海拔数据时出现异常：" + ex.Message;
                    this.DictErrorMessages["OPC"] = message;
                }
            }))
            { IsBackground = true }.Start();
        }

        /// <summary>
        /// 调用数据库服务
        /// </summary>
        public void CallDataService()
        {
            new Thread(new ThreadStart(() =>
            {
                string message = string.Empty;
                try
                {
                    //TODO 是否将此处经纬度改为XYZ
                    int flag = this.dataService.UpdateClaimerGnssInfo(this.ClaimerId, this.Latitude, this.LatitudeDirection, this.Longitude, this.LongitudeDirection, this.Altitude);
                    if (flag <= 0)
                        message = string.Format("ID为{0}的大机信息更新失败", this.ClaimerId);
                }
                catch (Exception e)
                {
                    //this.DictErrorMessages["DataService"] = string.Format("ID为{0}的大机信息更新出错：{1}", this.ClaimerId, e.Message);
                    message = string.Format("ID为{0}的大机信息更新出错", this.ClaimerId);
                }
                this.DictErrorMessages["DataService"] = string.IsNullOrWhiteSpace(message) ? string.Empty : string.Format("{0} [{1:yyyy-MM-dd HH:mm:ss}]", message, DateTime.Now);
            }))
            { IsBackground = true }.Start();
        }
        #endregion
    }

    #region 枚举
    /// <summary>
    /// GNSS返回信息类型
    /// </summary>
    public enum GnssMessageType
    {
        /// <summary>
        /// 未知类型
        /// </summary>
        Unknown = -1,

        /// <summary>
        /// 最佳可用的GPS位置信息
        /// </summary>
        BESTPOSA = 42,

        /// <summary>
        /// 伪距位置信息
        /// </summary>
        PSRPOSA = 47,

        /// <summary>
        /// 匹配的RTK位置（静态模式使用最佳，准确，高延迟）
        /// </summary>
        MATCHEDPOSA = 96,

        /// <summary>
        /// RTK低延迟定位数据（动态模式使用最佳，低延迟，牺牲一些准确性）
        /// </summary>
        RTKPOSA = 141,

        /// <summary>
        /// 接收机的时间，位置和定位相关数据（联合定位）
        /// </summary>
        GNGGA = 217,

        /// <summary>
        /// 接收机的时间，位置和定位相关数据
        /// </summary>
        GPGGA = 218,

        /// <summary>
        /// 接收机计算的时间、日期、位置、航向和速度信息
        /// </summary>
        GPRMC = 225,

        /// <summary>
        /// 以度为单位相对真北方向的航向信息（联合定位）
        /// </summary>
        GNHDT = 1044,

        /// <summary>
        /// 以度为单位相对真北方向的航向信息
        /// </summary>
        GPHDT = 1045
    }

    /// <summary>
    /// GPS质量指示
    /// </summary>
    public enum GpsQuality
    {
        /// <summary>
        /// 定位不可用或无效
        /// </summary>
        [EnumDescription("定位不可用或无效")]
        Invalid = 0,

        /// <summary>
        /// 单点定位
        /// </summary>
        [EnumDescription("单点定位")]
        SinglePoint = 1,

        /// <summary>
        /// 伪距差分或SBAS定位
        /// </summary>
        [EnumDescription("伪距差分或SBAS定位")]
        PseudoRangeOrSBAS = 2,

        /// <summary>
        /// RTK固定解
        /// </summary>
        [EnumDescription("RTK固定解")]
        RTKFixed = 4,

        /// <summary>
        /// RTK浮点解
        /// </summary>
        [EnumDescription("RTK浮点解")]
        RTKFloat = 5,

        /// <summary>
        /// 惯性导航定位
        /// </summary>
        [EnumDescription("惯性导航定位")]
        InertialNavigation = 6,

        /// <summary>
        /// 用户设定位置
        /// </summary>
        [EnumDescription("用户设定位置")]
        FixedPosition = 7
    }

    /// <summary>
    /// GPS信息解出状态
    /// </summary>
    public enum SolutionStatus
    {
        /// <summary>
        /// 已解出
        /// </summary>
        [EnumDescription("已解出")]
        SOL_COMPUTED = 0,

        /// <summary>
        /// 观测数据不足
        /// </summary>
        [EnumDescription("观测数据不足")]
        INSUFFICIENT_OBS = 1,

        /// <summary>
        /// 无法收敛
        /// </summary>
        [EnumDescription("无法收敛")]
        NO_CONVERGENCE = 2,

        /// <summary>
        /// 参数矩阵具有奇性（Singularity)
        /// </summary>
        [EnumDescription("参数矩阵具有奇性")]
        SINGULARITY = 3,

        /// <summary>
        /// 协方差矩阵的迹超过最大值（迹>1000 米）
        /// </summary>
        [EnumDescription("协方差矩阵的迹超过最大值")]
        COV_TRACE = 4,

        /// <summary>
        /// 超出测试距离
        /// </summary>
        [EnumDescription("超出测试距离")]
        TEST_DIST = 5,

        /// <summary>
        /// 冷启动尚未收敛（Not yet converged from cold start）
        /// </summary>
        [EnumDescription("冷启动尚未收敛")]
        COLD_START = 6,

        /// <summary>
        /// 超出高度或速度限制
        /// </summary>
        [EnumDescription("超出高度或速度限制")]
        V_H_LIMIT = 7,

        /// <summary>
        /// 超出方差限制
        /// </summary>
        [EnumDescription("超出方差限制")]
        VARIANCE = 8,

        /// <summary>
        /// 残差过大
        /// </summary>
        [EnumDescription("残差过大")]
        RESIDUALS = 9,

        /// <summary>
        /// 大量残差导致位置信息不可靠
        /// </summary>
        [EnumDescription("大量残差导致位置信息不可靠")]
        INTEGRITY_WARNING = 13,

        /// <summary>
        /// 挂起
        /// </summary>
        [EnumDescription("挂起")]
        PENDING = 18,

        /// <summary>
        /// 固定位置不可用
        /// </summary>
        [EnumDescription("固定位置不可用")]
        INVALID_FIX = 19,

        /// <summary>
        /// 位置类型未授权
        /// </summary>
        [EnumDescription("位置类型未授权")]
        UNAUTHORIZED = 20,

        /// <summary>
        /// 输出速率不支持当前解出类型
        /// </summary>
        [EnumDescription("输出速率不支持当前解出类型")]
        INVALID_RATE = 22
    }

    /// <summary>
    /// 位置或速度类型
    /// </summary>
    public enum PositionVelocityType
    {
        /// <summary>
        /// 无解
        /// </summary>
        [EnumDescription("无解")]
        NONE = 0,

        /// <summary>
        /// 位置由FIX POSITION命令指定
        /// </summary>
        [EnumDescription("位置由FIX POSITION命令指定")]
        FIXEDPOS = 1,

        /// <summary>
        /// 位置由FIX HEIGHT或FIX AUTO命令指定
        /// </summary>
        [EnumDescription("位置由FIX HEIGHT或FIX AUTO命令指定")]
        FIXEDHEIGHT = 2,

        /// <summary>
        /// 速度由即时多普勒信息导出
        /// </summary>
        [EnumDescription("速度由即时多普勒信息导出")]
        DOPPLER_VELOCITY = 8,

        /// <summary>
        /// 单点定位
        /// </summary>
        [EnumDescription("单点定位")]
        SINGLE = 16,

        /// <summary>
        /// 伪距差分解
        /// </summary>
        [EnumDescription("伪距差分解")]
        PSRDIFF = 17,

        /// <summary>
        /// L1浮点解
        /// </summary>
        [EnumDescription("L1浮点解")]
        L1_FLOAT = 32,

        /// <summary>
        /// 消电离层浮点解
        /// </summary>
        [EnumDescription("消电离层浮点解")]
        IONOFREE_FLOAT = 33,

        /// <summary>
        /// 窄巷浮点解
        /// </summary>
        [EnumDescription("窄巷浮点解")]
        NARROW_FLOAT = 34,

        /// <summary>
        /// L1固定解
        /// </summary>
        [EnumDescription("L1固定解")]
        L1_INT = 48,

        /// <summary>
        /// 宽巷固定解
        /// </summary>
        [EnumDescription("宽巷固定解")]
        WIDE_INT = 49,

        /// <summary>
        /// 窄巷固定解
        /// </summary>
        [EnumDescription("窄巷固定解")]
        NARROW_INT = 50
    }
    #endregion
}
