using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 新增密保问题
    /// </summary>
    public class AddNewQuestion:RequestBase
    {
        /// <summary>
        /// 问题内容
        /// </summary>
        [Required]
        public List<QuestionContent> Content { get; set; }
    }
    /// <summary>
    /// 问题内容
    /// </summary>
    public class QuestionContent
    {
        /// <summary>
        /// 本地语言版
        /// </summary>
        [Required]
        [StringLength(128)]
        public string local_Lang { get; set; }
        /// <summary>
        /// 英文版
        /// </summary>
        [Required]
        [StringLength(128)]
        public string en_US { get; set; }
    }
}
