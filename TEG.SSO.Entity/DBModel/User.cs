using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Table("User")]
  public   class User:DBModelBase
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        [StringLength(64)]
        public string AccountName { get; set; }

        /// <summary>
        /// 用户名字
        /// </summary>
        [StringLength(64)]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(256)]
        [JsonIgnore]
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
        public string Email { get; set; }
        /// <summary>
        /// qq号码
        /// </summary>
        [StringLength(16)]
        public string QQ { get; set; }
        /// <summary>
        /// 有效截止期
        /// </summary>
        public DateTime ValidTime { get; set; }
        /// <summary>
        /// 首次登陆修改密码
        /// </summary>
        public bool FirstChange { get; set; }
        /// <summary>
        /// 密码更新的周期。单位：天，0表示不用定时修改密码
        /// </summary>
        public int PasswordModifyPeriod { get; set; }
        /// <summary>
        /// 最近一次的密码修改时间
        /// </summary>
        public DateTime PasswordModifyTime { get; set; }
        /// <summary>
        /// 是否是新用户（从未登录），默认为true
        /// </summary>
        public bool IsNew { get; set; }
        /// <summary>
        /// 密码是否为Membership创建
        /// </summary>
        public bool IsMemberShipPassword { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; }

        public virtual ICollection<UserDeptRel> UserDeptRels { get; set; }

        public virtual ICollection<UserRoleRel> UserRoleRels { get; set; }

        public virtual ICollection<UserSecurityQuestion> UserSecurityQuestions { get; set; }
    }
}
