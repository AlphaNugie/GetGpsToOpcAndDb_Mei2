using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Extensions;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// RTK低延迟定位数据（该log推荐在动态模式下使用，静态模式下要获取更好的精度可用MATCHEDPOS）
    /// </summary>
    public class Rtkpos : GnssBaseMessage
    {
        #region 私有变量
        private SolutionStatus _sol_status = SolutionStatus.PENDING; //解出状态
        private PositionVelocityType _pos_type = PositionVelocityType.NONE; //位置速度类型
        private double _latitude = 0; //纬度
        private double _longitude = 0; //经度
        private double _height = 0; //海拔高度
        private double _undulation = 0; //大地水准面差距
        private string _datum_id = string.Empty; //坐标系ID
        private double _lat_variance = 0; //纬度标准差
        private double _lon_variance = 0; //经度标准差
        private double _height_variance = 0; //高度标准差
        private string _station_id = string.Empty; //基站ID
        private double _diff_age = 0; //差分龄期
        private double _sol_age = 0; //解的龄期
        private int _sate_num_tracked = 0; //跟踪中的卫星数
        private int _sate_num_used = 0; //在解中使用的卫星数
        //private int signal_mask = 0; //信号掩码
        private string signal_mask = string.Empty; //信号掩码
        #endregion

        #region 属性
        /// <summary>
        /// 位置信息解出类型
        /// </summary>
        public SolutionStatus SolutionStatus
        {
            get { return this._sol_status; }
            set
            {
                this._sol_status = value;
                if (this.Parent != null)
                    this.Parent.SolutionStatus = this._sol_status;
            }
        }

        /// <summary>
        /// 位置或速度类型
        /// </summary>
        public PositionVelocityType PositionType
        {
            get { return this._pos_type; }
            set
            {
                this._pos_type = value;
                if (this.Parent != null)
                {
                    this.Parent.PositionType = this._pos_type;
                    this.Parent.PositionQuality = this._pos_type.GetDescription();
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
        /// 海拔高度
        /// </summary>
        public double HeightAboveSea
        {
            get { return this._height; }
            set
            {
                this._height = value;
                if (this.Parent != null)
                    //this.Parent.HeightAboveSea = this._height;
                    this.Parent.Altitude = this._height;
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
        /// 坐标系ID
        /// </summary>
        public string DatumId
        {
            get { return this._datum_id; }
            set { this._datum_id = value; }
        }

        /// <summary>
        /// 纬度标准差
        /// </summary>
        public double LatitudeVariance
        {
            get { return this._lat_variance; }
            set { this._lat_variance = value; }
        }

        /// <summary>
        /// 经度标准差
        /// </summary>
        public double LongitudeVariance
        {
            get { return this._lon_variance; }
            set { this._lon_variance = value; }
        }

        /// <summary>
        /// 海拔高度标准差
        /// </summary>
        public double HeightVariance
        {
            get { return this._height_variance; }
            set { this._height_variance = value; }
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

        /// <summary>
        /// 差分龄期（秒），若为0，则未使用差分校正
        /// </summary>
        public double DifferentialAge
        {
            get { return this._diff_age; }
            set { this._diff_age = value; }
        }

        /// <summary>
        /// 解的龄期（秒）
        /// </summary>
        public double SolutionAge
        {
            get { return this._sol_age; }
            set { this._sol_age = value; }
        }

        /// <summary>
        /// 跟踪中的卫星数
        /// </summary>
        public int SatelliteNumberTracked
        {
            get { return this._sate_num_tracked; }
            set { this._sate_num_tracked = value; }
        }

        /// <summary>
        /// 在解中使用的卫星数
        /// </summary>
        public int SatelliteNumberUsed
        {
            get { return this._sate_num_used; }
            set { this._sate_num_used = value; }
        }

        /// <summary>
        /// 信号掩码
        /// </summary>
        public string SignalMask
        //public int SignalMask
        {
            get { return this.signal_mask; }
            set { this.signal_mask = value; }
        }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="parent">上级类</param>
        public Rtkpos(GnssInfoObject parent) : base(parent) { }

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
            string[] temp = message.Split(',', '*', ';');
            try
            {
                SolutionStatus status;
                PositionVelocityType type;
                Enum.TryParse(temp[10], out status);
                Enum.TryParse(temp[11], out type);
                if (!Enum.IsDefined(typeof(SolutionStatus), status))
                    status = SolutionStatus.PENDING;
                if (!Enum.IsDefined(typeof(PositionVelocityType), type))
                    type = PositionVelocityType.NONE;
                this.SolutionStatus = status;
                this.PositionType = type;
                this.Latitude = double.Parse(temp[12]);
                this.Longitude = double.Parse(temp[13]);
                this.HeightAboveSea = double.Parse(temp[14]);
                this.Undulation = double.Parse(temp[15]);
                this.DatumId = temp[16];
                this.LatitudeVariance = double.Parse(temp[17]);
                this.LongitudeVariance = double.Parse(temp[18]);
                this.HeightVariance = double.Parse(temp[19]);
                this.StationId = temp[20];
                this.DifferentialAge = double.Parse(temp[21]);
                this.SolutionAge = double.Parse(temp[22]);
                this.SatelliteNumberTracked = int.Parse(temp[23]);
                this.SatelliteNumberUsed = int.Parse(temp[24]);
                //this.SignalMask = Convert.ToInt32(temp[28], 16);
                this.SignalMask = temp[28];
            }
            catch (Exception) { }
        }
    }
}
