using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 新增部门信息
    /// </summary>
    public class AddDept : RequestBase<List<NewDept>>
    { }

    public class NewDept
    {
        /// <summary>
        /// 上级部门id
        /// </summary>
        public int? ParentID { get; set; }
        /// <summary>
        /// 新增部门名称
        /// </summary>
        [Required]
        [StringLength(64)]
        public string DeptName { get; set; }
    }
}
