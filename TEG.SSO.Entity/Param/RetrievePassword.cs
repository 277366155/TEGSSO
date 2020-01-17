﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    public class RetrievePassword : RequestBase<RetrievePasswordParam>
    { }
    /// <summary>
    /// 找回密码参数
    /// </summary>
    public class RetrievePasswordParam
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

        /// <summary>
        /// 邮件中可点击跳转的url
        /// </summary>
        [StringLength(256)]
        public string Url { get; set; }
    }
}
