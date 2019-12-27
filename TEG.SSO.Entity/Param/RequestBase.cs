using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 请求参数基类
    /// </summary>
    public class RequestBase
    {
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
}
