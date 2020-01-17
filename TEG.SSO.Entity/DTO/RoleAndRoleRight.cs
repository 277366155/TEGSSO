using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DTO
{
    public class RoleAndRightInfo
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public int ID { get; set; }
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
        /// <summary>
        /// 角色选项信息
        /// </summary>
        public List<RightInfo> RoleRightInfos { get; set; }
    }

    /// <summary>
    /// 角色信息
    /// </summary>
    public class RightInfo
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public int RoleID { get; set; }
        /// <summary>
        /// 是否是菜单类型的权限
        /// </summary>
        public bool IsMenu { get; set; }
        /// <summary>
        /// 权限菜单id
        /// </summary>
        public int? MenuID { get; set; }
        /// <summary>
        /// 权限模块id
        /// </summary>
        public int? AuthorizationObjectID { get; set; }
        /// <summary>
        /// 权限值
        /// </summary>
        public PermissionValue PermissionValue { get; set; }
    }
}
