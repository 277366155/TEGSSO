using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 找回密码参数
    /// </summary>
   public class RetrievePassword:RequestBase
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        [Required]
        [StringLength(64)]
        public string AccountName { get; set; }
        /// <summary>
        /// 问题id
        /// </summary>
        [Required]
        public int QuestionID { get; set; }
        /// <summary>
        /// 答案
        /// </summary>
        [Required]
        [StringLength(128)]
        public string Answer { get; set; }

        [StringLength(256)]
        public string Url { get; set; }
    }
}
