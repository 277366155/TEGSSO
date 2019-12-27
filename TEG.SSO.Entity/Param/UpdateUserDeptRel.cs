using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 用户-部门关系
    /// </summary>
    public class UpdateUserDeptRel:RequestBase
    {
        /// <summary>
        /// 用户-部门关系
        /// </summary>
        public List<UDRel> Rels { get; set; }
    }
    /// <summary>
    /// 用户-部门关系
    /// </summary>
    public class UDRel
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int UserID { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int DeptID { get; set; }
    }

}
