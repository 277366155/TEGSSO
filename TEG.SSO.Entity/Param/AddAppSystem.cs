using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    public class AddAppSystem : RequestBase<List<NewAppSystem>>
    { }

    public class NewAppSystem
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
    }
}
