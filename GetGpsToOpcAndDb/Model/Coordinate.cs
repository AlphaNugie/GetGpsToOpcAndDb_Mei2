using GetGpsToOpcAndDb.Core;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// 3维笛卡尔坐标系XYZ坐标基础类
    /// </summary>
    [ProtoContract]
    public class Coordinate
    {
        private double x, y, z, xp, yp, zp, xc, yc, zc;

        #region 原始坐标
        /// <summary>
        /// X坐标
        /// </summary>
        public double X
        {
            get { return this.x; }
            set
            {
                this.x = Math.Round(value, 3);
                this.xp = Math.Round(this.x * BaseConst.AxisRatios[0] + BaseConst.AxisRatios[1], 3);
                this.xc = Math.Round(this.xp - BaseConst.TrackCoordinate.xp, 3);
            }
        }

        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y
        {
            get { return this.y; }
            set
            {
                this.y = Math.Round(value, 3);
                this.yp = Math.Round(this.y * BaseConst.AxisRatios[2] + BaseConst.AxisRatios[3], 3);
                this.yc = Math.Round(this.yp - BaseConst.TrackCoordinate.yp, 3);
            }
        }

        /// <summary>
        /// Z坐标
        /// </summary>
        [ProtoMember(5)]
        public double Z
        {
            get { return this.z; }
            set
            {
                this.z = Math.Round(value, 3);
                this.zp = this.z;
                this.zc = Math.Round(this.zp - BaseConst.TrackCoordinate.z, 3);
                //this.ZClaimer = this.z - BaseConst.TrackCoordinate.z;
            }
        }
        #endregion

        #region 经过变换处理后的坐标
        /// <summary>
        /// 处理后X坐标：X'
        /// </summary>
        public double XPrime { get { return !BaseConst.AxisSwapped ? this.xp : this.yp; } }

        /// <summary>
        /// 处理后Y坐标：Y'
        /// </summary>
        public double YPrime { get { return !BaseConst.AxisSwapped ? this.yp : this.xp; } }

        /// <summary>
        /// 处理后Z坐标：Z'
        /// </summary>
        public double ZPrime { get { return this.zp; } }
        #endregion

        #region 单机坐标
        /// <summary>
        /// 相对于单机轨道起点的X坐标（基于处理后的X坐标）
        /// </summary>
        public double XClaimer { get { return !BaseConst.AxisSwapped ? this.xc : this.yc; } }

        /// <summary>
        /// 相对于单机轨道起点的Y坐标（基于处理后的Y坐标）
        /// </summary>
        public double YClaimer { get { return !BaseConst.AxisSwapped ? this.yc : this.xc; } }

        /// <summary>
        /// 相对于单机轨道起点的Z坐标
        /// </summary>
        public double ZClaimer { get { return this.zc; } }
        #endregion

        /// <summary>
        /// 默认构造器
        /// </summary>
        public Coordinate() { }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="x">X坐标</param>
        /// <param name="y">Y坐标</param>
        /// <param name="z">Z坐标</param>
        public Coordinate(double x, double y, double z)
        {
            this.Update(x, y, z);
        }

        /// <summary>
        /// 更新坐标
        /// </summary>
        /// <param name="x">更新后X坐标</param>
        /// <param name="y">更新后Y坐标</param>
        /// <param name="z">更新后Z坐标</param>
        public void Update(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        /// <summary>
        /// 将坐标对象根据变换后坐标转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString("prime");
        }

        /// <summary>
        /// 将坐标对象根据指定的坐标类型转换为字符串
        /// </summary>
        /// <param name="param">3种坐标类型：default（原始坐标），prime（变换后坐标）, claimer（单机）</param>
        /// <returns></returns>
        public string ToString(string param)
        {
            string output;
            switch (param)
            {
                case "prime":
                    output = string.Format("{0}, {1}, {2}", this.XPrime, this.YPrime, this.ZPrime);
                    break;
                case "claimer":
                    output = string.Format("{0}, {1}, {2}", this.XClaimer, this.YClaimer, this.ZClaimer);
                    break;
                default:
                    output = string.Format("{0}, {1}, {2}", this.X, this.Y, this.Z);
                    break;
            }
            return output;
        }
    }
}
