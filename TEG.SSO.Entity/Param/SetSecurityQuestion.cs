using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 设置安全问题参数
    /// </summary>
    public class SetSecurityQuestion:RequestBase
    {
        /// <summary>
        /// 密码密文
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Password { get; set; }

        /// <summary>
        /// 安全问题id
        /// </summary>
        [Required]        
        public int SecurityQuestionID { get; set; }

        /// <summary>
        /// 问题答案
        /// </summary>
        [Required]
        [StringLength(128,MinimumLength =2)]
        public string Answer { get; set; }
    }
}
