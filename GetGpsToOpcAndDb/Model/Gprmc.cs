using GetGpsToOpcAndDb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// 接收机计算的时间、日期、位置、航向和速度信息的实体类
    /// </summary>
    public class Gprmc : GnssBaseMessage
    {
        #region 私有变量
        private string _utc = string.Empty; //UTC时间
        private bool _pos_valid = false; //
        private double _latitude = 0; //纬度
        private string _lat_dir = string.Empty; //维度方向，N S
        private double _longitude = 0; //经度
        private string _lon_dir = string.Empty; //经度方向，E W
        private double _ground_speed = 0;
        private double _track_true = 0;
        private string _date = string.Empty;
        private double _mag_dec = 0;
        private string _mag_dec_dir = string.Empty;
        private string _mode_indicator = string.Empty;
        #endregion

        #region 属性
        /// <summary>
        /// 位置对应的UTC事件，格式：hhmmss.ss
        /// </summary>
        public string Utc
        {
            get { return this._utc; }
            set
            {
                this._utc = value;
                if (this.Parent != null)
                    this.Parent.Utc = this._utc;
            }
        }

        /// <summary>
        /// 位置状态是否有效
        /// </summary>
        public bool IsPositionValid
        {
            get { return this._pos_valid; }
            set
            {
                this._pos_valid = value;
                if (this.Parent != null)
                {
                    this.Parent.IsPositionValid = this._pos_valid;
                    this.Parent.PositionQuality = this._pos_valid ? "定位信息有效" : "定位信息无效";
                }
            }
        }

        /// <summary>
        /// 纬度（DDmm.mm）
        /// </summary>
        public double Latitude
        {
            get { return this._latitude; }
            set
            {
                this._latitude = value;
                if (this.Parent != null)
                    this.Parent.Latitude = this._latitude;
            }
        }

        /// <summary>
        /// 纬度方向（N = 北，S =南）
        /// </summary>
        public string LatitudeDirection
        {
            get { return this._lat_dir; }
            set
            {
                this._lat_dir = value;
                if (this.Parent != null)
                    this.Parent.LatitudeDirection = this._lat_dir;
            }
        }

        /// <summary>
        /// 经度（DDDmm.mm）
        /// </summary>
        public double Longitude
        {
            get { return this._longitude; }
            set
            {
                this._longitude = value;
                if (this.Parent != null)
                    this.Parent.Longitude = this._longitude;
            }
        }

        /// <summary>
        /// 经度方向（E = 东，W = 西）
        /// </summary>
        public string LongitudeDirection
        {
            get { return this._lon_dir; }
            set
            {
                this._lon_dir = value;
                if (this.Parent != null)
                    this.Parent.LongitudeDirection = this._lon_dir;
            }
        }

        /// <summary>
        /// 地速，相对于地面的速度，单位：节
        /// </summary>
        public double GroundSpeed
        {
            get { return this._ground_speed; }
            set
            {
                this._ground_speed = value;
                if (this.Parent != null)
                    this.Parent.GroundSpeed = this._ground_speed;
            }
        }

        /// <summary>
        /// 真北航迹方向，单位：度
        /// </summary>
        public double TrackDirection_TrueNorth
        {
            get { return this._track_true; }
            set
            {
                this._track_true = value;
                if (this.Parent != null)
                    this.Parent.TrackDirection_TrueNorth = this._track_true;
            }
        }

        /// <summary>
        /// 日期，格式：ddmmyy
        /// </summary>
        public string Date
        {
            get { return this._date; }
            set
            {
                this._date = value;
                if (this.Parent != null)
                    this.Parent.Date = this._date;
            }
        }

        /// <summary>
        /// 磁偏角，单位：度
        /// </summary>
        public double MagneticDeclination
        {
            get { return this._mag_dec; }
            set
            {
                this._mag_dec = value;
                if (this.Parent != null)
                    this.Parent.MagneticDeclination = this._mag_dec;
            }
        }

        /// <summary>
        /// 磁偏角方向，E 东，W 西
        /// </summary>
        public string MagneticDeclinationDir
        {
            get { return this._mag_dec_dir; }
            set
            {
                this._mag_dec_dir = value;
                if (this.Parent != null)
                    this.Parent.MagneticDeclinationDir = this._mag_dec_dir;
            }
        }

        /// <summary>
        /// 定位模式指示符：A 单点定位, D 差分定位, E 推算定位, M 用户输入, N 数据无效
        /// </summary>
        public string ModeIndicator
        {
            get { return this._mode_indicator; }
            set
            {
                this._mode_indicator = value;
                if (this.Parent != null)
                    this.Parent.ModeIndicator = this._mode_indicator;
            }
        }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="parent">上级类</param>
        public Gprmc(GnssInfoObject parent) : base(parent) { }

        /// <summary>
        /// 报文处理
        /// </summary>
        /// <param name="message"></param>
        public override void Analyze(ref string message)
        {
            base.Analyze(ref message);
            //假如出现错误信息，则退出
            if (!string.IsNullOrWhiteSpace(this.ErrorMessage))
                return;
            string[] temp = message.Split(',', '*');
            try
            {
                this.Utc = temp[1];
                this.IsPositionValid = temp[2].Equals("A");
                this.Latitude = BaseFunc.GetDegree(temp[3]);
                this.LatitudeDirection = temp[4];
                this.Longitude = BaseFunc.GetDegree(temp[5]);
                this.LongitudeDirection = temp[6];
                this.GroundSpeed = double.Parse(temp[7]);
                this.TrackDirection_TrueNorth = double.Parse(temp[8]);
                this.Date = temp[9];
                this.MagneticDeclination = double.Parse(temp[10]);
                this.MagneticDeclinationDir = temp[11];
                this.ModeIndicator = temp[12];
            }
            catch (Exception) { }
        }
    }
}
