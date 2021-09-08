using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// 航向信息，航向是移动基站（MOVINGBASE）至定向接收机（HEADING）间基线向量顺时针方向与真北的夹角
    /// </summary>
    public class Heading : GnssBaseMessage
    {
        #region 私有变量
        private double _base_length = 0;
        private double _track_true = 0;
        private double _pitch = 0;
        private bool _is_true_north = true;
        #endregion

        #region 属性
        /// <summary>
        /// 基线长度
        /// </summary>
        public double BaselineLength
        {
            get { return _base_length; }
            set
            {
                _base_length = value;
                if (Parent != null)
                    Parent.BaselineLength = _base_length;
            }
        }

        /// <summary>
        /// 真北航迹方向，单位：度
        /// </summary>
        public double TrackDirection_TrueNorth
        {
            get { return _track_true; }
            set
            {
                _track_true = value;
                if (Parent != null)
                    Parent.TrackDirection_TrueNorth = _track_true;
            }
        }

        /// <summary>
        /// 俯仰（±90°）
        /// </summary>
        public double Pitch
        {
            get { return _pitch; }
            set
            {
                _pitch = value;
                if (Parent != null)
                    Parent.AntePitch = _pitch;
            }
        }

        /// <summary>
        /// 真北
        /// </summary>
        public bool IsTrueNorth
        {
            get { return _is_true_north; }
            set
            {
                _is_true_north = value;
                if (Parent != null)
                    Parent.IsTrueNorth = _is_true_north;
            }
        }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="parent">上级类</param>
        public Heading(GnssInfoObject parent) : base(parent) { }

        /// <summary>
        /// 报文处理
        /// </summary>
        /// <param name="message"></param>
        public override void Analyze(ref string message)
        {
            base.Analyze(ref message);
            //假如出现错误信息，则退出
            if (!string.IsNullOrWhiteSpace(ErrorMessage))
                return;
            Parent.Raiser.Click();
            string[] temp = message.Split(',', '*', ';');
            try
            {
                double baseLen, track, pitch = 0;
                if (double.TryParse(temp[12], out baseLen))
                    BaselineLength = baseLen;
                //Parent.TrackDirection_Received = double.TryParse(temp[13], out angle);
                if (Parent.TrackDirection_Received = double.TryParse(temp[13], out track))
                    TrackDirection_TrueNorth = track;
                if (double.TryParse(temp[14], out pitch))
                    Pitch = pitch;
                //IsTrueNorth = temp[2].Equals("T");
                IsTrueNorth = true;
            }
            catch (Exception) { }
        }
    }
}
