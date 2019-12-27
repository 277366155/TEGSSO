using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TEG.SSO.Common
{
    public static class IQueryableException
    {
        /// <summary>
        /// 转为分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="pageSize">每页行数</param>
        /// <returns></returns>
        public static Page<T> ToPage<T>(this IQueryable<T> queryable,int pageIndex,int pageSize)
        {
            return new Page<T> {
                PageIndex=pageIndex,
                PageSize=pageSize,
                Total=queryable.Count(),
                List= queryable?.Skip(pageIndex * pageSize).Take(pageSize).ToList()
            };
        }
    }
    /// <summary>
    /// 分页查询对象
    /// </summary>
    public class Page
    {
        /// <summary>
        /// 查询页码数，从0开始
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每一页行数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 数据总行数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                if (Total <= 0 || PageSize <= 0)
                {
                    return 0;
                }
                else
                {
                    return Total / PageSize + Total % PageSize == 0 ? 0 : 1;
                }
            }
        }
    }

    /// <summary>
    /// 分页数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T> : Page
    {
        /// <summary>
        /// 当前页数据列表
        /// </summary>
        public List<T> List { get; set; }
    }
}
