using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    public class AdminChangePassword : RequestBase<AdminChangePasswordParam>
    { }
    /// <summary>
    /// 管理员修改其他人密码
    /// </summary>
    public class AdminChangePasswordParam
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        public int UserID { get; set; }

        /// <summary>
        ///密码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Password { get; set; }
    }
}
