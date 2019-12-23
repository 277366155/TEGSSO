using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 用户安全问题记录
    /// </summary>
    [Table("UserSecurityQuestion")]
   public  class UserSecurityQuestion: DBModelBase
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 问题id
        /// </summary>
        public int QuestionID { get; set; }
        /// <summary>
        /// 答案
        /// </summary>
        [StringLength(128)]
        public string Answer { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        [ForeignKey("QuestionID")]
        public virtual SecurityQuestion SecurityQuestion { get; set; }
    }
}
