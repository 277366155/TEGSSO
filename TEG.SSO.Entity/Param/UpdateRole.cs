using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    public class UpdateRole:RequestBase<List<UpdateRoleParam>>
    {
    }

    public class UpdateRoleParam : NewRole
    {
        /// <summary>
        /// 角色id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }
        /// <summary>
        /// 是否有父级限制。父级限制是指，角色权限只能是父级角色权限集合的子集
        /// </summary>
        public bool ParentLimit { get; set; }
        /// <summary>
        /// 权限数据
        /// </summary>
        public List<RoleRightInfo> RoleRightInfos { get; set; }
    }

    public class RoleRightInfo
    {
        /// <summary>
        /// 是否是菜单
        /// </summary>
        [Required]
        public bool IsMenu { get; set; }
        /// <summary>
        /// 菜单或功能项id
        /// </summary>
        [Required]
        [Range(1,int.MaxValue)]
        public int RightID { get; set; }
        /// <summary>
        /// 权限值
        /// </summary>
        [Required]
        public PermissionValue PermissionValue { get; set; }
    }
}
