using GetGpsToOpcAndDb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// 包含以度为单位相对真北方向的航向信息的实体类
    /// </summary>
    public class Gphdt : GnssBaseMessage
    {
        #region 私有变量
        private double _track_true = 0;
        private bool _is_true_north = true;
        #endregion

        #region 属性
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
        /// 真北
        /// </summary>
        public bool IsTrueNorth
        {
            get { return this._is_true_north; }
            set
            {
                this._is_true_north = value;
                if (this.Parent != null)
                    this.Parent.IsTrueNorth = this._is_true_north;
            }
        }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="parent">上级类</param>
        public Gphdt(GnssInfoObject parent) : base(parent) { }

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
                this.TrackDirection_TrueNorth = double.Parse(temp[1]);
                this.IsTrueNorth = temp[2].Equals("T");
            }
            catch (Exception) { }
        }
    }
}
