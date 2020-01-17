using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 请求参数基类
    /// </summary>
    public class RequestBase
    {
        public RequestBase() { }
        public RequestBase(string sysCode, string ip = null, Language lang = Language.local_Lang)
        {
            this.SysCode = sysCode;
            this.RequestIP = ip;
            this.Lang = lang;
        }
        /// <summary>
        /// 系统code
        /// </summary>
        [Required]
        [StringLength(64)]
        public string SysCode { get; set; }

        /// <summary>
        /// 客户端发起请求的ip
        /// </summary>
        [StringLength(64)]
        public string RequestIP { get; set; }

        /// <summary>
        /// 语言类型
        /// </summary>
        [Required]
        public Language Lang { get; set; }
    }

    /// <summary>
    /// 请求参数基类
    /// </summary>
    public class RequestBase<T>: RequestBase
    {
        public RequestBase() { }
        public RequestBase(T data , string sysCode):base(sysCode)
        {
            this.Data = data;
            this.SysCode = sysCode;
        }
        /// <summary>
        /// 业务请求参数
        /// </summary>
        [Required]
        public T Data { get; set;}
    }
}
