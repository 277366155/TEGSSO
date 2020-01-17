using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 下拉列表选项
    /// </summary>
   public  class AppSystemSelectItem
    {
        /// <summary>
        /// 业务系统id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 业务系统码
        /// </summary>
        public string SystemCode { get; set; }
        /// <summary>
        /// 业务系统名称
        /// </summary>
        public string SystemName { get; set; }
    }
}
