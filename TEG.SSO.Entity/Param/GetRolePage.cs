using System.ComponentModel.DataAnnotations;

namespace TEG.SSO.Entity.Param
{
    public class GetRolePage : RequestBase<GetRolePager>
    { }
    public class GetRolePager : Pager
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [StringLength(64)]
        public string RoleName { get; set; }
        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool? IsSuperAdmin { get; set; }

        /// <summary>
        /// 父级ID。 0-表示查询顶级角色。null-表示查询所有节点
        /// </summary>
        public int? ParentID { get; set; }
    }
}
