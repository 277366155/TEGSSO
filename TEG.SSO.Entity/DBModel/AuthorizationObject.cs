using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 权限设置对象（除菜单以外的功能、数据）
    /// </summary>
    [Table("AuthorizationObject")]
    public class AuthorizationObject : DBModelBase
    {
        /// <summary>
        ///view元素code
        /// </summary>
        [StringLength(64)]
        public string ObjectCode { get; set; }
        /// <summary>
        /// 名称描述
        /// </summary>
        [StringLength(512)]
        public string ObjectName { get; set; }
        /// <summary>
        /// 权限设置对象的类型
        /// </summary>
        public ObjectType ObjectType { get; set; }
        /// <summary>
        /// 所属菜单id
        /// </summary>
        public int? MenuId { get; set; }

        [ForeignKey("MenuId")]
        public virtual Menu Menu { get; set; }

        public virtual ICollection<RoleRight> RoleRights { get; set; }
    }
}
