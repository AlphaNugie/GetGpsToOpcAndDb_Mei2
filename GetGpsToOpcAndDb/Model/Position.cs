using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// 现实世界位置（WGS84坐标系坐标）
    /// </summary>
    public class Position
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// 海拔
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public Position() { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="lat">纬度</param>
        /// <param name="lon">经度</param>
        /// <param name="alt">海拔</param>
        public Position(double lat, double lon, double alt)
        {
            this.Update(lat, lon, alt);
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="lat">更新后纬度</param>
        /// <param name="lon">更新后经度</param>
        /// <param name="alt">更新后海拔</param>
        public void Update(double lat, double lon, double alt)
        {
            this.Latitude = lat;
            this.Longitude = lon;
            this.Altitude = alt;
        }
    }
}
