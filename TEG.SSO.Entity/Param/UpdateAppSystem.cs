using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 更新业务系统信息
    /// </summary>
    public class UpdateAppSystem:RequestBase<List<UpdateAppSystems>>
    {
    }
    /// <summary>
    /// 更新业务系统信息
    /// </summary>
    public class UpdateAppSystems
    {
        /// <summary>
        ///业务系统Id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }

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
    }
}
