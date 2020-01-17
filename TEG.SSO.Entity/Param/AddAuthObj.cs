using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 新增菜单、数据、功能参数
    /// </summary>
    public class AddAuthObj : RequestBase<List<NewObj>>
    { }

    public class NewObj
    {
        /// <summary>
        /// 父级节点，只能是菜单的id
        /// </summary>
        public int? MenuID { get; set; }
        /// <summary>
        /// 类型  1-功能，2-数据
        /// </summary>
        [Required]
        public ObjectType ObjType { get; set; }
        /// <summary>
        /// 权限对象code
        /// </summary>
        [Required]
        [StringLength(64)]
        public string ObjCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public MultipleLanguage ObjName { get; set; }

    }
}
