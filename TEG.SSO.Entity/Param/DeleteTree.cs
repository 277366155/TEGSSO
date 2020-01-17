using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    public class DeleteTree : RequestBase<DeleteTrees>
    { }
    /// <summary>
    /// 删除节点信息 部门、菜单、角色等。。
    /// </summary>
    public class DeleteTrees
    {
        /// <summary>
        /// 是否一起删除下级节点。若为false，则当前节点有下级节点存在时，不可删除
        /// </summary>
        public bool RemoveChildren { get; set; }

        /// <summary>
        ///节点id列表
        /// </summary>
        [Required]
        public List<int> IDs { get; set; }
    }
}
