using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 用户-部门关系表
    /// </summary>
    [Table("UserDeptRel")]
    public class UserDeptRel:DBModelBase
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public int DeptID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("DeptID")]
        public virtual Organization Dept { get; set; }
    }
}
