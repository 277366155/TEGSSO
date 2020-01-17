using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    public class UpdateMenu:RequestBase<List<UpdateMenuParam>>
    {
    }

    public class UpdateMenuParam : NewMenu
    {
        /// <summary>
        /// 菜单id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }
    }
}
