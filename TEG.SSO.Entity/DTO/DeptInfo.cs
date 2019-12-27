using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 部门信息
    /// </summary>
    public class DeptInfo
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public int DeptID { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

    }

    /// <summary>
    /// 包含下级部门的部门信息
    /// </summary>
    public class DeptAndChildrenInfo : DeptInfo
    {
        /// <summary>
        /// 下级部门
        /// </summary>
        public List<DeptAndChildrenInfo> Children { get; set; }
    }

    /// <summary>
    /// 包含上级部门的部门信息
    /// </summary>
    public class DeptAndParentInfo : DeptInfo
    {
        /// <summary>
        /// 上级部门
        /// </summary>
        public DeptAndParentInfo Parent { get; set; }
    }
}
