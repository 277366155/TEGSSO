using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 接入平台信息
    /// </summary>
    [Table("AppSystem")]
    public class AppSystem:DBModelBase
    {
        /// <summary>
        /// 系统code，不可重复
        /// </summary>
        [StringLength(64)]
        public string SystemCode { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        [StringLength(64)]
        public string SystemName { get; set; }
        /// <summary>
        /// 系统类型
        /// </summary>
        public SystemType SystemType { get; set; }
        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; }
        [JsonIgnore]
        public virtual ICollection<Menu> Menus { get; set; }
    }
}
