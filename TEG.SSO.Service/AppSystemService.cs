using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.Service
{
    public class AppSystemService : ServiceBase<AppSystem>
    {
        public AppSystemService(IServiceProvider svp) : base(svp)
        {
        }

        /// <summary>
        /// 分页获取业务系统信息列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<AppSystem>> GetAppSystemPage(GetAppSystemPage param)
        {
            var iQueryable = GetIQueryable(a => true);
            if (param.Data.SystemCode.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.SystemCode.Contains(param.Data.SystemCode));
            }

            if (param.Data.SystemName.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.SystemName.Contains(param.Data.SystemName));
            }
            if (param.Data.IsDisabled.HasValue)
            {
                iQueryable = iQueryable.Where(a => a.IsDisabled == param.Data.IsDisabled);
            }
            if (param.Data.SystemType.HasValue)
            {
                iQueryable = iQueryable.Where(a => a.SystemType == param.Data.SystemType);
            }
            var data = iQueryable.ToPage(param.Data.PageIndex, param.Data.PageSize);
            return new SuccessResult<Page<AppSystem>>(data);
        }

        /// <summary>
        /// 删除指定系统信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> DeleteAppSystemAsync(DeleteTree param)
        {
            if (param.Data.IDs.Any(a => !masterDbSet.Any(m => m.ID == a)))
            {
                throw new CustomException("SysIDError", "含有错误的系统id");
            }

            if (!param.Data.RemoveChildren)
            {
                var menuExist = masterDbSet.Where(a => param.Data.IDs.Contains(a.ID)).Include(a => a.Menus).Any(a => a.Menus != null && a.Menus.Count() > 0);
                if (menuExist)
                {
                    throw new CustomException("DataExist", "有数据存在，不可删除");
                }
            }
            await DeleteManyAsync(a => param.Data.IDs.Contains(a.ID));
            return new SuccessResult();
        }

        /// <summary>
        /// 禁用、启用指定业务系统
        /// </summary>
        /// <param name="param"></param>
        /// <param name="disable">是否要禁用</param>
        /// <returns></returns>
        public async Task<Result> DisableOrEnableAppSystemAsync(RequestID param, bool disable)
        {
            if (param.Data.IDs.Any(a => !masterDbSet.Any(m => m.ID == a)))
            {
                throw new CustomException("SysIDError", "含有错误的系统id");
            }
            var dataList = masterDbSet.Where(a => param.Data.IDs.Contains(a.ID)).ToList();
            if (dataList == null || dataList.Count <= 0)
            {
                throw new CustomException("NoData", "未查到任何数据");
            }
            var utcNow = DateTime.UtcNow;
            dataList.ForEach(a =>
            {
                a.IsDisabled = disable;
                a.LastUpdateAccountName = currentUser.AccountName;
                a.ModifyTime = utcNow;
            });
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 新增业务系统信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> AddAppSystemAsync(AddAppSystem param)
        {
            if (param.Data.GroupBy(a => a.SystemCode).Any(a => a.Count() > 1))
            {
                throw new CustomException("SystemCodeExist", "系统码重复");
            }
            if (param.Data.GroupBy(a => a.SystemName).Any(a => a.Count() > 1))
            {
                throw new CustomException("SystemNameExist", "系统名称重复");
            }
            if (readOnlyDbSet.Any(a => param.Data.Select(m => m.SystemName).Contains(a.SystemName)))
            {
                throw new CustomException("SystemNameExist", "系统名称已存在");
            }
            if (readOnlyDbSet.Any(a => param.Data.Select(m => m.SystemCode).Contains(a.SystemCode)))
            {
                throw new CustomException("SystemCodeExist", "系统码已存在");
            }
            var insertData = param.Data.MapTo<List<AppSystem>>();
            insertData.ForEach(a => a.LastUpdateAccountName = currentUser.AccountName);
            await InsertManyAsync(insertData.ToArray());
            return new SuccessResult();
        }

        /// <summary>
        /// 更新指定系统信息。systemcode不能更改
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAppSystemsAsync(UpdateAppSystem param)
        {
            if (param.Data.GroupBy(a => a.SystemName).Any(a => a.Count() > 1))
            {
                throw new CustomException("SystemNameExist", "系统名称重复");
            }

            if (param.Data.Any(a => readOnlyDbSet.Any(m => m.ID != a.ID && m.SystemName == a.SystemName)))
            {
                throw new CustomException("SystemNameExist", "系统名称已存在");
            }
            var utcNow = DateTime.UtcNow;
            foreach (var app in param.Data)
            {
                var sys = masterDbSet.FirstOrDefault(a => a.ID == app.ID);
                sys.ModifyTime = utcNow;
                sys.LastUpdateAccountName = currentUser.AccountName;
                sys.SystemName = app.SystemName;
                sys.SystemType = app.SystemType;
                sys.IsDisabled = app.IsDisabled;
            }
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }


        /// <summary>
        /// 业务系统下拉列表数据
        /// </summary>
        /// <param name="isDisabled"></param>
        /// <returns></returns>
        public async Task<Result<List<AppSystemSelectItem>>> GetAppSystemListAsync(bool isDisabled)
        {
            var datalist = await GetListAsync(a => a.IsDisabled == isDisabled);
            var resultData = datalist.Select(a => new AppSystemSelectItem
            {
                ID = a.ID,
                SystemCode = a.SystemCode,
                SystemName = a.SystemName
            }).ToList();
            return new SuccessResult<List<AppSystemSelectItem>>(resultData);
        }

        /// <summary>
        ///检查当系统码是否存在， 系统信息缓存于redis中
        /// </summary>
        /// <param name="sysCode"></param>
        /// <returns></returns>
        public bool CheckExistFromCache(string sysCode)
        {
            var apps = TryToGetFromRedis(ConfigService.AppSystemCodesKey, () =>
            {
                return readOnlyDbSet.Where(a => true).ToList();
            });
            return apps.Exists(a => !a.IsDisabled && a.SystemCode == sysCode);
        }
    }
}
