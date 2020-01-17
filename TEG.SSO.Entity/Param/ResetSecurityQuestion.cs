using System.ComponentModel.DataAnnotations;

namespace TEG.SSO.Entity.Param
{
    public class ResetSecurityQuestion : RequestBase<ResetSecurityQuestionParam>
    { }
    /// <summary>
    /// 重设安全问题参数
    /// </summary>
    public class ResetSecurityQuestionParam: SetSecurityQuestionParam
    {
        /// <summary>
        /// 原问题id
        /// </summary>
        [Required]
        public int OldSecurityQuestionID { get; set; }

        /// <summary>
        /// 原问题答案
        /// </summary>
        [Required]
        [StringLength(128, MinimumLength = 2)]
        public string OldSecurityQuestionAnswer { get; set; }
    }
}
