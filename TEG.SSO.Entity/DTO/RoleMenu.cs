using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 只包含请求特定某一系统的权限信息
    /// </summary>
    public class RoleMenu
    {
        /// <summary>
        /// 菜单code
        /// </summary>
        public string MenuCode { get; set; }
        /// <summary>
        /// 菜单名称，json格式支持多语言：{"local_Lang":"系统设置","en_US":"System Setting"}
        /// </summary>
        public string MenuName { get; set; }
        ///// <summary>
        ///// 所属系统id
        ///// </summary>
        //public int SystemID { get; set; }
        /// <summary>
        /// 权限值
        /// </summary>
        public PermissionValue PermissionValue { get; set; }
        /// <summary>
        /// 下级菜单
        /// </summary>
        public List<RoleMenu> ChildrenMenus { get; set; }
        /// <summary>
        /// 菜单页面中的权限对象
        /// </summary>
        public List<MenuChildrenObject> MenuChildrenObjects { get; set; }
        /// <summary>
        /// 排序id，按升序排，越小越靠前
        /// </summary>
        public int SortID { get; set; }
    }
}
