using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 角色权限配置
    /// </summary>
    [Table("RoleRight")]
    public class RoleRight : DBModelBase
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public int RoleID { get; set; }
        /// <summary>
        /// 权限设置类型：Menu时，取MenuID；Function和Data时，取AuthorizationObjectID
        /// </summary>
        public RightType RightType { get; set; }

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

        [ForeignKey("RoleID")]
        public virtual Role Role { get; set; }

        [ForeignKey("MenuID")]
        public virtual Menu Menu { get; set; }

        [ForeignKey("AuthorizationObjectID")]
        public virtual AuthorizationObject AuthorizationObject { get; set; }
    }
}
