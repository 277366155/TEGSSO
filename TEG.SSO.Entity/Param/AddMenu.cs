using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 新增菜单
    /// </summary>
    public class AddMenu : RequestBase<List<NewMenu>>
    { }

    public class NewMenu
    {
        /// <summary>
        /// 父级节点，只能是菜单的id
        /// </summary>
        public int? ParentID { get; set; }

        /// <summary>
        /// 菜单code
        /// </summary>
        [Required]
        [StringLength(64)]
        public string MenuCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Required]
        public MultipleLanguage MenuName { get; set; }
        /// <summary>
        /// 所属系统id 
        /// </summary>
        [Required]
        public int SystemID { get; set; }
        /// <summary>
        /// 排序id 
        /// </summary>
        [Required]
        public int SortID { get; set; }
    }
}
