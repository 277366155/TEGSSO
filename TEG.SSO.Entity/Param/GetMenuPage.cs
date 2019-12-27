using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
   public  class GetMenuPage:PageParam
    {
        /// <summary>
        /// 业务系统id。下拉列表选择。
        /// </summary>
        public int? SystemID { get; set; }
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
        /// 是否禁用
        /// </summary>
        public bool? IsDisabled { get; set; }

    }
}
