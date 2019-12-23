using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Common
{
    /// <summary>
    /// 自定义异常类
    /// </summary>
   public  class CustomException:Exception
    {
        public ExceptionInfo Info { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="errorMsg">错误信息</param>
        public CustomException(string errorCode, string errorMsg)
        {
            this.Info = new ExceptionInfo { Code= errorCode, Message=errorMsg };
        }
        public CustomException(ExceptionInfo info)
        {
            this.Info = info;
        }
    }

    public class ExceptionInfo
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
    }
}
