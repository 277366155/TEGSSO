using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    public class GetAppSystemPage : RequestBase<GetAppSystemPager>
    { }
    public class GetAppSystemPager:Pager
    {
        /// <summary>
        /// 系统code
        /// </summary>
        [StringLength(64)]
        public string SystemCode { get; set; }
        /// <summary>
        /// 系统名称
        /// </summary>
        [StringLength(64)]
        public string SystemName { get; set; }
        /// <summary>
        /// 是否已禁用
        /// </summary>
        public bool? IsDisabled { get; set; }

        /// <summary>
        /// 系统类型
        /// </summary>
        public SystemType? SystemType { get; set; }
    }
}
