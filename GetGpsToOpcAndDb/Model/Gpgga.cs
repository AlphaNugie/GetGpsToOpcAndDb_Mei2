using CommonLib.Enums;
using GetGpsToOpcAndDb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// 接收机的时间，位置和定位相关数据的实体类
    /// </summary>
    public class Gpgga : GnssBaseMessage
    {
        #region 私有变量
        private string _utc = string.Empty; //UTC时间
        private double _latitude = 0; //纬度
        private string _lat_dir = string.Empty; //维度方向，N S
        private double _longitude = 0; //经度
        private string _lon_dir = string.Empty; //经度方向，E W
        private GpsQuality _quality = GpsQuality.Invalid; //GPS质量
        private int _sat_num = 0; //卫星数量
        private double _hdop = 0; //水平经度衰减因子
        private double _altitude = 0; //天线高度
        private double _undulation = 0; //大地水准面差距
        private int _age = 0; //基站数据龄期
        private string _station_id = string.Empty; //基站ID
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
        /// GPS质量指示符
        /// </summary>
        public GpsQuality Quality
        {
            get { return this._quality; }
            set
            {
                this._quality = value;
                if (this.Parent != null)
                {
                    this.Parent.Quality = this._quality;
                    this.Parent.PositionQuality = this._quality.GetDescription();
                }
            }
        }

        /// <summary>
        /// 使用中的卫星数。可能与所见数不一致
        /// </summary>
        public int SatelliteNumber
        {
            get { return this._sat_num; }
            set
            {
                this._sat_num = value;
                if (this.Parent != null)
                    this.Parent.SatelliteNumber = this._sat_num;
            }
        }

        /// <summary>
        /// 水平精度因子
        /// </summary>
        public double HorizontalDOP
        {
            get { return this._hdop; }
            set
            {
                this._hdop = value;
                if (this.Parent != null)
                    this.Parent.HorizontalDOP = this._hdop;
            }
        }

        /// <summary>
        /// 天线高度，高于/低于平均海平面，单位：米
        /// </summary>
        public double Altitude
        {
            get { return this._altitude; }
            set
            {
                this._altitude = value;
                if (this.Parent != null)
                    this.Parent.Altitude = this._altitude;
            }
        }

        /// <summary>
        /// 大地水准面差距（米）–大地水准面和WGS84椭球面之间的距离。大地水准面高于椭球面为正值，否则，为负值
        /// </summary>
        public double Undulation
        {
            get { return this._undulation; }
            set
            {
                this._undulation = value;
                if (this.Parent != null)
                    this.Parent.Undulation = this._undulation;
            }
        }

        /// <summary>
        /// 差分数据龄期，没有差分数据时为0，单位：秒
        /// </summary>
        public int Age
        {
            get { return this._age; }
            set
            {
                this._age = value;
                if (this.Parent != null)
                    this.Parent.Age = this._age;
            }
        }

        /// <summary>
        /// 差分基站ID，0000-1023，没有差分数据时为00
        /// </summary>
        public string StationId
        {
            get { return this._station_id; }
            set
            {
                this._station_id = value;
                if (this.Parent != null)
                    this.Parent.StationId = this._station_id;
            }
        }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="parent">上级类</param>
        public Gpgga(GnssInfoObject parent) : base(parent) { }

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
                this.Latitude = BaseFunc.GetDegree(temp[2]);
                this.LatitudeDirection = temp[3];
                this.Longitude = BaseFunc.GetDegree(temp[4]);
                this.LongitudeDirection = temp[5];
                this.Quality = (GpsQuality)int.Parse(temp[6]);
                this.SatelliteNumber = int.Parse(temp[7]);
                this.HorizontalDOP = double.Parse(temp[8]);
                this.Altitude = double.Parse(temp[9]);
                this.Undulation = double.Parse(temp[11]);
                this.Age = int.Parse(temp[13]);
                this.StationId = temp[14];
            }
            catch (Exception) { }
        }
    }
}
