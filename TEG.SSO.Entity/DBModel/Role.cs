using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 角色信息
    /// </summary>
    [Table("Role")]
    public class Role:DBModelBase
    {
        /// <summary>
        /// 角色名称
        /// </summary>
        [StringLength(64)]
        public string RoleName { get; set; }

        /// <summary>
        /// 父级id
        /// </summary>
        public int? ParentID { get; set; }
        /// <summary>
        /// 是否是超级管理员。（true-不验证权限，直接返回所有权限数据）
        /// </summary>
        public bool IsSuperAdmin { get; set; }

        [ForeignKey("ParentID")]
        public virtual Role Parent { get; set; }
        
        public virtual ICollection<Role> Children { get; set; }

        public virtual ICollection<RoleRight> RoleRights { get; set; }
    }
}
