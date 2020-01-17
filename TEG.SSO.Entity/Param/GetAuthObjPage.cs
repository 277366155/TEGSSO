using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    public class GetAuthObjPage : RequestBase<GetAuthObjPager>
    { }
    /// <summary>
    /// 分页获取功能权限数据
    /// </summary>
    public class GetAuthObjPager : Pager
    {
        /// <summary>
        /// 所属菜单id
        /// </summary>
        public int? MenuID { get; set; }
        /// <summary>
        /// 所属菜单code
        /// </summary>
        public string MenuCode { get; set; }
        /// <summary>
        /// 功能code
        /// </summary>
        public string ObjCode { get; set; }
        /// <summary>
        /// 功能名称
        /// </summary>
        public string ObjName { get; set; }

    }
}
