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
    public class CheckPermission : RequestBase<List<CheckParam>>
    { }

    public class CheckParam
    {
        /// <summary>
        /// 待校验的code
        /// </summary>
        [Required]
        [StringLength(64)]
        public string Code { get; set; }

        /// <summary>
        /// code是否是菜单
        /// </summary>
        [Required]
        public bool IsMenu { get; set; }
    }
}
