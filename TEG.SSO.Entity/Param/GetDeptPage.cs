using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.Entity.DTO
{
    public class GetDeptPage : RequestBase<DeptPage>
    { }
    /// <summary>
    /// 分页查询部门信息
    /// </summary>
    public class DeptPage:Pager
    {
        public int? ID { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [StringLength(64)]
        public string OrgName { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>        
        public int? ParentID { get; set; }
    }
}
