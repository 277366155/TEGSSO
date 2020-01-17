using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{    /// <summary>
     /// 新增用户参数
     /// </summary>
    public class NewUserList : RequestBase<List<NewUser>>
    { }

    /// <summary>
    /// 用户参数
    /// </summary>
    public class NewUser
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        [Required]
        [StringLength(64, MinimumLength = 2)]
        public string AccountName { get; set; }
        /// <summary>
        /// 用户名字
        /// </summary>
        [Required]
        [StringLength(64, MinimumLength = 2)]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(256, MinimumLength = 2)]
        public string Password { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(32)]
        public string Mobile { get; set; }
        /// <summary>
        /// 座机电话
        /// </summary>
        [StringLength(32)]
        public string Telphone { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [StringLength(64)]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string Email { get; set; }
        /// <summary>
        /// qq号码
        /// </summary>
        [StringLength(16, MinimumLength = 4)]
        public string QQ { get; set; }

        /// <summary>
        /// 首次登陆修改密码
        /// </summary>
        [Required]
        public bool FirstChange { get; set; }
        /// <summary>
        /// 密码更新的周期。单位：天，0表示不用定时修改密码
        /// </summary>
        [Required]
        public int PasswordModifyPeriod { get; set; }

        /// <summary>
        /// 密码是否为Membership创建
        /// </summary>
        [Required]
        public bool IsMemberShipPassword { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [Required]
        public bool IsDisabled { get; set; }
    }
}
