using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TEG.Framework.Security.SSO;
using TEG.Framework.Standard.Cache;
using TEG.SSO.Common;
using TEG.SSO.EFCoreContext;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.Service
{
    public class ServiceBase<T> where T : DBModelBase
    {
        protected IServiceProvider _serviceProvider;
        protected BizReadOnlyContext readOnlyContext;
        protected BizMasterContext masterContext;
        protected LogContext logContext;
        protected HttpContext currentHttpContext;
        protected RedisCache redisCache;
        protected TokenUserInfo currentUser;
        protected DbSet<T> masterDbSet;
        protected DbSet<T> readOnlyDbSet;

       
        public ServiceBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            readOnlyContext = (BizReadOnlyContext)serviceProvider.GetService(typeof(BizReadOnlyContext));
            masterContext = (BizMasterContext)serviceProvider.GetService(typeof(BizMasterContext));
            logContext = (LogContext)serviceProvider.GetService(typeof(LogContext));
            currentHttpContext=((IHttpContextAccessor)serviceProvider.GetService(typeof(IHttpContextAccessor))).HttpContext;
            redisCache = (RedisCache)serviceProvider.GetService(typeof(RedisCache));
            currentUser = GetCurrentUserFromToken();
            masterDbSet = masterContext.Set<T>();
            readOnlyDbSet = readOnlyContext.Set<T>();
        }

        /// <summary>
        /// token解密获取用户身份
        /// </summary>
        /// <returns></returns>
        private TokenUserInfo GetCurrentUserFromToken()
        {
            if (currentHttpContext == null)
            {
                return null;
            }
            var author = currentHttpContext.Request.Headers["Authorization"];
            //var sysCode = _httpContext.Request.Query["SysCode"];
            if (string.IsNullOrWhiteSpace(author) || !author.FirstOrDefault().Contains("Bearer") )//|| sysCode.ToString().IsNullOrWhiteSpace())
            {
                return null;
            }
            var token = author.ToString().Substring("Bearer".Length).Trim();
            List<string> list;
            
            //token校验不通过或者token过期
            if (!SSOHelper.IsTokenValid(token, out list)||
                Convert.ToDateTime(list[4]).AddMinutes(ConfigService.TokenOverTime) < DateTime.Now)
            {
                return null;
            }
            //解析token获取用户信息
            var tokenUserInfo = new TokenUserInfo { UserID = Convert.ToInt32(list[0]), AccountName = list[1], UserName = list[2], IP = list[3],Token= token };

            return tokenUserInfo;
        }

        #region 主库中新增、删除、更新
        /// <summary>
        /// 插入单个数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> InsertAsync(T model)
        {
            await masterDbSet.AddAsync(model);
            var result = await masterContext.SaveChangesAsync();
            return result > 0;
        }

        /// <summary>
        /// 批量插入model
        /// </summary>
        /// <param name="models"></param>
        public async Task<bool> InsertManyAsync(T[] models)
        {
            await masterDbSet.AddRangeAsync(models);
            return await  masterContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 删除指定model
        /// </summary>
        /// <param name="model"></param>
        public async Task<bool> DeleteAsync(T model)
        {
            masterDbSet.Remove(model);
            return await masterContext.SaveChangesAsync() > 0;
        }
        /// <summary>
        /// 删除符合指定条件的数据
        /// </summary>
        /// <param name="expression">lambda表达式</param>
        public async Task<bool> DeleteManyAsync(Expression<Func<T, bool>> expression)
        {
            var datas = await masterDbSet.Where(expression).ToArrayAsync();
            masterDbSet.RemoveRange(datas);
            return await masterContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        ///根据id更新model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(T model)
        {
            var data = await masterDbSet.FirstOrDefaultAsync(a => a.ID == model.ID);
            //获取属性，并遍历赋值
            var properties = typeof(T).GetProperties();
            foreach (var p in properties)
            {
                if (p.Name.ToLower() == "id")
                {
                    continue;
                }
                var type = p.PropertyType;
                p.SetValue(data, p.GetValue(model));
            }
            return await masterContext.SaveChangesAsync()>0;
        }

        /// <summary>
        ///讲符合条件的数据 批量更新为目标数据
        /// </summary>
        /// <param name="expression">筛选条件</param>
        /// <param name="model">目标数据</param>
        /// <returns></returns>
        public async Task<bool> UpdateManyAsync(Expression<Func<T, bool>> expression, T model)
        {
            var datas = await masterDbSet.Where(expression).ToArrayAsync();

            var properties = typeof(T).GetProperties();

            foreach (var data in datas)
            {
                foreach (var p in properties)
                {
                    if (p.Name.ToLower() == "id")
                    {
                        continue;
                    }
                    var type = p.PropertyType;
                    p.SetValue(data, p.GetValue(model));
                }
            }
            return await masterContext.SaveChangesAsync()>0;
        }
        #endregion 主库中新增、删除、更新

        #region 查询

        /// <summary>
        /// 检查是否存在指定的数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="fromMasterDb"></param>
        /// <returns></returns>
        public async Task<Result<bool>> IsExistAsync(Expression<Func<T, bool>> filter, bool fromMasterDb = false)
        {
            var result = await (fromMasterDb ? masterDbSet : readOnlyDbSet).AnyAsync(filter);
            return new SuccessResult<bool>() { Data=result  };
        }

        /// <summary>
        /// 按照指定条件和排序规则，取出第一行数据。默认查询从库，按照id正序排列
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="fromMasterDb">是否从主库查询</param>
        /// <returns></returns>
        public async Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter,  bool fromMasterDb = false)
        {
            return await  GetFirstOrDefaultAsync<object>(filter, null, SortOption.ASC, fromMasterDb);
        }

        /// <summary>
        /// 按照指定条件和排序规则，取出第一行数据。默认查询从库，按照id正序排列
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="sortOption">正序asc，倒序desc</param>
        /// <param name="fromMasterDb">是否从主库查询</param>
        /// <returns></returns>
        public async Task<T> GetFirstOrDefaultAsync<TKey>(Expression<Func<T, bool>> filter, Expression<Func<T, TKey>> sort = null, SortOption sortOption = SortOption.ASC, bool fromMasterDb = false)
        {
            var data = GetIQueryable(filter, sort, sortOption, fromMasterDb);
            return await data.FirstOrDefaultAsync();
        }

        /// <summary>
        /// 按照指定条件查出列表数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="fromMasterDb">是否从主库查询</param>
        /// <returns></returns>
        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> filter, bool fromMasterDb = false)
        {
            return await GetListAsync<object>(filter, null, SortOption.ASC, fromMasterDb);
        }

        /// <summary>
        ///  按照指定条件和排序规则，查出列表数据。默认查询从库，按照id正序排列
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="sortOption">正序asc，倒序desc</param>
        /// <param name="fromMasterDb">是否从主库查询</param>
        /// <returns>List集合</returns>
        public async Task<List<T>> GetListAsync<TKey>(Expression<Func<T, bool>> filter = null, Expression<Func<T, TKey>> sort = null, SortOption sortOption = SortOption.ASC, bool fromMasterDb = false)
        {
            return await GetIQueryable(filter, sort, sortOption, fromMasterDb).ToListAsync();
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="sortOption"></param>
        /// <param name="pageIndex">页码，从0开始</param>
        /// <param name="pageSize">每页行数</param>
        /// <param name="fromMasterDb">是否从主库查询</param>
        /// <returns></returns>
        public async Task<Page<T>> GetPageAsync<TKey>(Expression<Func<T, bool>> filter = null, Expression<Func<T, TKey>> sort = null, SortOption sortOption = SortOption.ASC, int pageIndex = 0, int pageSize = 10, bool fromMasterDb = false)
        {
            var dataIQueryable = GetIQueryable(filter, sort, sortOption, fromMasterDb);

            return new Page<T>()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Total = dataIQueryable.Count(),
                Data = await dataIQueryable.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync()
            };
        }      

        /// <summary>
        ///  按照指定条件和排序规则，查出IQueryable集合。默认查询从库，按照id正序排列
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="sortOption">正序asc，倒序desc</param>
        /// <param name="fromMasterDb">是否从主库查询</param>
        /// <returns>IQueryable</returns>
        protected IQueryable<T> GetIQueryable<TKey>(Expression<Func<T, bool>> filter = null, Expression<Func<T, TKey>> sort = null, SortOption sortOption = SortOption.ASC, bool fromMasterDb = false)
        {
            IQueryable<T> iQueryable;
            if (filter == null)
            {
                iQueryable = (fromMasterDb ? masterDbSet : readOnlyDbSet).Where(a => true);
            }
            else
            {
                iQueryable = (fromMasterDb ? masterDbSet : readOnlyDbSet).Where(filter);
            }
            switch (sortOption)
            {
                default:
                case SortOption.ASC:
                    if (sort == null)
                    {
                        return iQueryable.OrderBy(a => a.ID);
                    }
                    else
                    {
                        return iQueryable.OrderBy(sort);
                    }
                case SortOption.DESC:
                    if (sort == null)
                    {
                        return iQueryable.OrderByDescending(a => a.ID);
                    }
                    else
                    {
                        return iQueryable.OrderByDescending(sort);
                    }
            }
        }

        /// <summary>
        /// 查询排序规则
        /// </summary>
        public enum SortOption
        {
            /// <summary>
            /// 正序
            /// </summary>
            ASC = 0,
            /// <summary>
            /// 倒序
            /// </summary>
            DESC
        }
        #endregion 查询



    }
}
