using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 新增角色
    /// </summary>
    public class AddRole : RequestBase<List<NewRole>>
    { }

    public class NewRole
    {
        /// <summary>
        /// 上级角色
        /// </summary>
        public int? ParentID { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [StringLength(64)]
        public string RoleName { get; set; }

        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSuperAdmin { get; set; }
    }
}
