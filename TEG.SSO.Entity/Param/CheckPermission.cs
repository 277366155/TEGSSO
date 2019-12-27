using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 权限校验参数
    /// </summary>
    public class CheckPermission:RequestBase
    {
        /// <summary>
        /// 待权限校验列表
        /// </summary>
        public List<CheckParam> Checks { get; set; }
    }

    public class CheckParam
    {
        /// <summary>
        /// 待校验的code
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Code { get; set; }

        /// <summary>
        /// code类型
        /// </summary>
        [Required]
        public RightType Type { get; set; }
    }
}
