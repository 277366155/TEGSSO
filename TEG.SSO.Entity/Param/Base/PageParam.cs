using System.ComponentModel.DataAnnotations;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 分页查询参数
    /// </summary>
    public class PageParam : RequestBase<Pager>
    { }

    /// <summary>
    /// 分页查询参数
    /// </summary>
    public class Pager
    {
        /// <summary>
        /// 页码数，从0开始
        /// </summary>
        [Range(0,int.MaxValue)]
        public int PageIndex { get; set; } = 0;
        /// <summary>
        /// 每页数据行数
        /// </summary>
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;
    }

}
