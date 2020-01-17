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
    public class AuthorizationObjectService : ServiceBase<AuthorizationObject>
    {
        public AuthorizationObjectService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 分页获取菜单下的数据项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<AuthorizationObject>> GetAuthObjPage(GetAuthObjPage param)
        {
            var iQueryable = GetIQueryable(a => true);
            //menuid=0表示查询未分配上级菜单的功能项
            if (param.Data.MenuID.HasValue)
            {
                if (param.Data.MenuID == 0)
                {
                    iQueryable = iQueryable.Where(a => a.MenuId == null);
                }
                else
                {
                    iQueryable = iQueryable.Where(a => a.MenuId == param.Data.MenuID);
                }
            }
            if (param.Data.MenuCode.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Include(a => a.Menu).Where(a => a.Menu.MenuCode.Contains(param.Data.MenuCode));
            }
            if (param.Data.ObjCode.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.ObjectCode.Contains(param.Data.ObjCode));
            }
            if (param.Data.ObjName.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.ObjectName.JsonToObj<MultipleLanguage>().local_Lang.Contains(param.Data.ObjName) || a.ObjectName.JsonToObj<MultipleLanguage>().en_US.Contains(param.Data.ObjName));
            }
            var data = iQueryable.ToPage(param.Data.PageIndex, param.Data.PageSize);
            data.List.ForEach(a =>
            {
                a.ObjectName = a.ObjectName.JsonToObj<MultipleLanguage>().GetContent(param.Lang);
                if (a.Menu != null)
                {
                    a.Menu.MenuName = a.Menu.MenuName.JsonToObj<MultipleLanguage>().GetContent(param.Lang);
                }
            });
            return new SuccessResult<Page<AuthorizationObject>> { Data = data };
        }
        
        /// <summary>
        /// 新增菜单下级权限对象
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> AddAuthObjAsync(AddAuthObj param)
        {
            var parentNotExist = param.Data.Any(a => a.MenuID.HasValue && !masterContext.Menus.Any(m => m.ID == a.MenuID));
            if (parentNotExist)
            {
                throw new CustomException("ParentIDError", "含有错误的父级菜单ID");
            }
            var objCode = param.Data.Select(m => m.ObjCode);
            var codeExist = masterContext.AuthorizationObjects.Any(a => objCode.Contains(a.ObjectCode));
            if (codeExist)
            {
                throw new CustomException("ObjCodeError", "含有重复的编码");
            }
            var insertData = param.Data.MapTo<List<AuthorizationObject>>();
            insertData.ForEach(a=>a.LastUpdateAccountName=currentUser.AccountName);
            await masterContext.AuthorizationObjects.AddRangeAsync(insertData);
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 更新指定功能项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> UpdateAuthObjectsAsync(UpdateAuthObj param)
        {
            //参数中同级重名判断
            var nameParamExist = param.Data.Any(a => param.Data.Any(m => m.ID != a.ID && m.MenuID == a.MenuID && m.ObjName == a.ObjName));
            if (nameParamExist)
            {
                throw new CustomException("NameIsExist", "同级下名称重复");
            }
            var idNotExist = param.Data.Any(a=>!masterContext.AuthorizationObjects.Any(m=>m.ID==a.ID));
            if (idNotExist)
            {
                throw new CustomException("IDError", "ID不存在");
            }
            var newAuthObjCodeList = param.Data.Select(a => a.ObjCode);
            var codeExist = newAuthObjCodeList.GroupBy(a => a).Any(a => a.Count() > 1) || masterContext.AuthorizationObjects.Any(m => param.Data.Any(a=>a.ID!=m.ID&&a.ObjCode==m.ObjectCode));
            if (codeExist)
            {
                throw new CustomException("CodeError", "编码重复");
            }

            //db中同级重名判断
            var nameExist = param.Data.Any(a => masterContext.AuthorizationObjects.Any(m => m.ID != a.ID && m.MenuId == a.MenuID && m.ObjectName == a.ObjName.ToJson()));
            if (nameExist)
            {
                throw new CustomException("NameIsExist", "同级下名称已存在");
            }
            //参数中有任何一个非空不存在的parentid，就不往下执行
            var parentNotExist = param.Data.Any(a => a.MenuID != null && !masterContext.Menus.Any(m => m.ID == a.MenuID));
            if (parentNotExist)
            {
                throw new CustomException("MenuIDError", "含有错误的上级菜单ID");
            }
            var utcNow = DateTime.UtcNow;
            foreach (var e in param.Data)
            {
                var m = await masterDbSet.FirstOrDefaultAsync(a => a.ID == e.ID);
                m.ObjectCode = e.ObjCode;
                m.ModifyTime = utcNow;
                m.ObjectName = e.ObjName.ToJson();
                m.MenuId = e.MenuID;
                m.LastUpdateAccountName = currentUser.AccountName;
            }
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 删除指定的功能项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> DeleteAuthObjectsAsync(RequestID param)
        {
            if (param.Data.IDs.Any(a => !masterDbSet.Any(m => m.ID == a)))
            {
                throw new CustomException("IDError","操作id错误");
            }
            await DeleteManyAsync(a => param.Data.IDs.Contains(a.ID));
            return new SuccessResult();
        }
    }
}
