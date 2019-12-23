using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 找回密码安全问题
    /// </summary>
    [Table("SecurityQuestion")]
    public  class SecurityQuestion:DBModelBase
    {
        /// <summary>
        /// 问题内容
        /// json格式支持多语言：{"zh_CN":"你妈妈的名字？","en_US":"What's your mother's name?"}
        /// </summary>
        [StringLength(512)]
        public string QuestionContent { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; }

        public virtual ICollection<UserSecurityQuestion> UserSecurityQuestions { get; set; }
    }
}
