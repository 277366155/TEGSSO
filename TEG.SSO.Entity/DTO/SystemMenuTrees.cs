using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 系统菜单树
    /// </summary>
    public class SystemMenuTrees
    {
        /// <summary>
        /// 系统code
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        public string SysName { get; set; }
        /// <summary>
        /// 当前系统下的菜单集合
        /// </summary>
        public List<MenuTree> MenuTrees { get; set; }
    }

    public class MenuTree
    {
        /// <summary>
        /// 菜单id
        /// </summary>
        public int MenuID { get; set; }
        /// <summary>
        /// 菜单编号
        /// </summary>
        public string MenuCode { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; }

        ///// <summary>
        ///// 权限值
        ///// </summary>
        //public PermissionValue PermissionValue { get; set; }
    
        /// <summary>
        /// 排列序号
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        /// 父级菜单id
        /// </summary>
        public int? ParentID { get; set; }

        ///// <summary>
        ///// 子菜单
        ///// </summary>
        //public List<MenuTree> ChildrenMenus { get; set; }

        /// <summary>
        /// 子模块
        /// </summary>
        public List<AuthObj> ChildrenAuthObj { get; set; }

    }
    /// <summary>
    /// 菜单下级权限对象data/function
    /// </summary>
    public class AuthObj
    {
        /// <summary>
        /// 权限对象id
        /// </summary>
        public int ObjID { get; set; }
        /// <summary>
        /// 权限对象名称
        /// </summary>
        public string ObjName { get; set; }
        /// <summary>
        /// 权限对象code
        /// </summary>
        public string ObjCode { get; set; }
        /// <summary>
        /// 权限对象类型
        /// </summary>
        public ObjectType ObjType { get; set; }
        ///// <summary>
        ///// 权限值
        ///// </summary>
        //public PermissionValue PermissionValue { get; set; }
    }
}
