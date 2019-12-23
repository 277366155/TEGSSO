using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 用户登录成功之后返回的数据
    /// </summary>
    public class UserInfoAndRoleRight
    {
        /// <summary>
        /// 身份凭证
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 系统code
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 登录账户基本信息
        /// </summary>
         public UserInfo UserInfo { get; set; }
        /// <summary>
        /// 登录账户权限信息
        /// </summary>
        public List<RoleMenu> RoleMenus { get; set; }
    }
}
