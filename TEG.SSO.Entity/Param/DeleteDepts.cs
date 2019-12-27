using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 删除部门信息
    /// </summary>
   public class DeleteDepts:RequestBase
    {
        /// <summary>
        /// 是否一起删除下级部门。若为false，则当前部门有下级部门存在时，不可删除
        /// </summary>
        public bool RemoveChildren { get; set; }

        /// <summary>
        /// 部门id列表
        /// </summary>
        [Required]
        public List<int> DeptIDs { get; set; }
    }
}
