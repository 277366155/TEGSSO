using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 用户登录接口参数
    /// </summary>
    public class UserLogin : RequestBase
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        [StringLength(32)]
        public string LoginName { get; set; }
        /// <summary>
        /// 加密之后的密码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string EncryptedPassword { get; set; }

    }
}
