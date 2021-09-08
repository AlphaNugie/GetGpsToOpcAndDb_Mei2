using CommonLib.Extensions.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    public class OpcDataSource
    {
        /// <summary>
        /// 经度
        /// </summary>
        [PropertyMapperFrom("LocalCoor_Ante.X")]
        public double Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [PropertyMapperFrom("LocalCoor_Ante.Y")]
        public double Latitude { get; set; }

        /// <summary>
        /// 海拔
        /// </summary>
        [PropertyMapperFrom("LocalCoor_Ante.Z")]
        public double Altitude { get; set; }

        /// <summary>
        /// 行走位置（北斗）
        /// </summary>
        [PropertyMapperFrom("WalkingPosition")]
        public double WalkingPosition { get; set; }

        /// <summary>
        /// 俯仰角度（北斗）
        /// </summary>
        [PropertyMapperFrom("PitchAngle")]
        public double PitchAngle { get; set; }

        /// <summary>
        /// 回转角度（北斗）
        /// </summary>
        [PropertyMapperFrom("YawAngle")]
        public double YawAngle { get; set; }

        /// <summary>
        /// 行走位置（PLC）
        /// </summary>
        [PropertyMapperTo("WalkingPosition")]
        public double WalkingPosition_Plc { get; set; }

        /// <summary>
        /// 俯仰角度（PLC）
        /// </summary>
        [PropertyMapperTo("PitchAngle")]
        public double PitchAngle_Plc { get; set; }

        /// <summary>
        /// 回转角度（PLC）
        /// </summary>
        [PropertyMapperTo("YawAngle")]
        public double YawAngle_Plc { get; set; }
    }
}
