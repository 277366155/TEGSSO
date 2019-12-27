using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 修改密码
    /// </summary>
   public  class ChangePassword:RequestBase
    {
        /// <summary>
        /// 原密码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string NewPassword { get; set; }
        /// <summary>
        /// 新密码确认
        /// </summary>
        [Required]
        [StringLength(256)]
        public string ConfirmPassword { get; set; }
    }
}
