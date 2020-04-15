﻿using GetGpsToOpcAndDb.Core;
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
    public class Coordinate
    {
        private double x, y, z, xp, yp, xc, yc;

        #region 原始坐标
        /// <summary>
        /// X坐标
        /// </summary>
        public double X
        {
            get { return this.x; }
            set
            {
                this.x = value;
                this.xp = this.x * BaseConst.AxisRatios[0] + BaseConst.AxisRatios[1];
                this.xc = this.xp - BaseConst.TrackCoordinate.xp;
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
                this.y = value;
                this.yp = this.y * BaseConst.AxisRatios[2] + BaseConst.AxisRatios[3];
                this.yc = this.yp - BaseConst.TrackCoordinate.yp;
            }
        }

        /// <summary>
        /// Z坐标
        /// </summary>
        public double Z
        {
            get { return this.z; }
            set
            {
                this.z = value;
                this.ZClaimer = this.z - BaseConst.TrackCoordinate.z;
            }
        }
        #endregion

        #region 经过变换处理后的坐标
        /// <summary>
        /// 处理后X坐标：X'
        /// </summary>
        public double XPrime
        {
            get { return !BaseConst.AxisSwapped ? this.xp : this.yp; }
            //set
            //{
            //    if (!BaseConst.AxisSwapped)
            //        this.xp = value;
            //    else
            //        this.yp = value;
            //}
        }

        /// <summary>
        /// 处理后Y坐标：Y'
        /// </summary>
        public double YPrime
        {
            get { return !BaseConst.AxisSwapped ? this.yp : this.xp; }
            //set
            //{
            //    if (!BaseConst.AxisSwapped)
            //        this.yp = value;
            //    else
            //        this.xp = value;
            //}
        }
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
        public double ZClaimer { get; set; }
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
    }
}