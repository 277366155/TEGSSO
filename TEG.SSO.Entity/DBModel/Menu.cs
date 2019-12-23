using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 菜单信息
    /// </summary>
    [Table("Menu")]
    public  class Menu:DBModelBase
    {
        /// <summary>
        /// 菜单code
        /// </summary>
        [StringLength(64)]
        public string MenuCode { get; set; }
        /// <summary>
        /// 菜单名称，json格式支持多语言：{"local_Lang":"系统设置","en_US":"System Setting"}
        /// </summary>
        [StringLength(512)]
        public string MenuName { get; set; }
        /// <summary>
        /// 所属系统id
        /// </summary>
        public int SystemID { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        public int? ParentID { get; set; }
        /// <summary>
        /// 排序id，按升序排，越小越靠前
        /// </summary>
        public int SortID { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; }

        public virtual ICollection<AuthorizationObject> AuthorizationObjects { get; set; }
        [ForeignKey("SystemID")]
        public virtual AppSystem AppSystem { get; set; }
        [ForeignKey("ParentID")]
        public virtual Menu Parent{ get; set; }
        public virtual ICollection<Menu> Children { get; set; }

        public virtual ICollection<RoleRight> RoleRights { get; set; }
    }
}
