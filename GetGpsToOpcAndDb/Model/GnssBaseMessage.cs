using CommonLib.Function;
using GetGpsToOpcAndDb.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GetGpsToOpcAndDb.Model
{
    /// <summary>
    /// GNSS消息基础类(NMEA 0183)
    /// </summary>
    public class GnssBaseMessage
    {
        private readonly Regex regex_message = new Regex(BaseConst.RegexPattern_FullMessage, RegexOptions.Compiled); //提取消息类型的正则
        private string message = string.Empty;

        #region 属性
        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage
        {
            get { return message; }
            set
            {
                message = value;
                if (Parent != null)
                    Parent.DictErrorMessages["GNSS"] = message;
            }
        }

        /// <summary>
        /// 原始报文
        /// </summary>
        public string OriginalMessage { get; set; }

        /// <summary>
        /// 父类
        /// </summary>
        public GnssInfoObject Parent { get; set; }
        #endregion

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="parent">上级类</param>
        public GnssBaseMessage(GnssInfoObject parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// 解析报文内容
        /// </summary>
        /// <param name="message"></param>
        public virtual void Analyze(ref string message)
        {
            ErrorMessage = string.Empty;
            message = regex_message.Match(message).Value;
            if (string.IsNullOrEmpty(message))
                ErrorMessage = "未找到符合格式的GNSS消息";
            //假如校验和与CRC32均未校验通过
            //else if (!BaseFunc.IsChecksumVerified(message) && !BaseFunc.IsCrc32Verified(message))
            else if (!HexHelper.IsGnssChecksumVerified(message) && !HexHelper.IsGnssCRC32Verified(message))
                ErrorMessage = "GNSS消息未通过校验";

            if (string.IsNullOrWhiteSpace(ErrorMessage))
                OriginalMessage = message;
        }
    }
}
