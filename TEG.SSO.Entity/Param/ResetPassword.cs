using System.ComponentModel.DataAnnotations;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 通过邮件重置密码
    /// </summary>
    public class ResetPassword:RequestBase
    {
        /// <summary>
        /// 账户名
        /// </summary>
        [Required]
        [StringLength(64)]
        public string AccountName { get; set; }
        /// <summary>
        /// 邮箱验证码
        /// </summary>
        [Required]
        [StringLength(16)]
        public string VerificationCode { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string NewPassword { get; set; }
        /// <summary>
        /// 确认新密码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string ConfirmPassword { get; set; }
    }
}
