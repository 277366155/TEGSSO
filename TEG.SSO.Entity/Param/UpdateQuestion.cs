using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 更新密保问题
    /// </summary>
   public  class UpdateQuestion:RequestBase
    {
        public List<UpdateQuestionContent> Question { get; set; }
    }

    public class UpdateQuestionContent: QuestionContent
    {
        /// <summary>
        /// id
        /// </summary>
        [Required]
        public int ID { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        [Required]
        public bool IsDisabled { get; set; }
    }
}
