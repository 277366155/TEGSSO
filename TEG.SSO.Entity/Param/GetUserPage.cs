using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
   public  class GetUserPage:PageParam
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int? UserID { get; set; }
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
        /// 性别
        /// </summary>
        public Gender? Gender { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [StringLength(64)]
        public string Email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [StringLength(32)]
        public string Mobile { get; set; }
        /// <summary>
        /// 是否是新用户（从未登录），默认为true
        /// </summary>
        public bool? IsNew { get; set; }
        /// <summary>
        /// 密码是否为Membership创建
        /// </summary>
        public bool? IsMemberShipPassword { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool? IsDisabled { get; set; }
    }
}
