using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    public class UserLogin : RequestBase<UserLoginParam>
    { }
    /// <summary>
    /// 用户登录接口参数
    /// </summary>
    public class UserLoginParam
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        [StringLength(32)]
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Password { get; set; }

    }
}
