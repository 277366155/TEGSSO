using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.Service
{
    public class MenuService : ServiceBase<Menu>
    {
        public MenuService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public List<RoleMenu> GetRoleMenuByUserID(int userId)
        {

            return null;
        }

        #region 权限校验
        /// <summary>
        /// 检查对应code 的权限值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<List<PermissionValue>> CheckPermission(CheckPermission param)
        {
            //防止频繁读取数据库
            var roleInfo =  TryToGetFromRedis(ConfigService.GetDBUserRoleRedisKey(currentUser.AccountName,currentUser.UserID), Convert.ToInt32(BaseCore.AppSetting["CacheTime"]), 
                 () => {
                var user=   masterContext.Users.Where(a=>a.AccountName==currentUser.AccountName)
                                                                               .Include(a => a.UserRoleRels).ThenInclude(a=>a.Role).FirstOrDefault();
                return user.UserRoleRels.AsQueryable().Select(a => a.Role).ToList();
            });
            var dataList = new List<PermissionValue>();
            //当前用户角色中如果有超级管理员角色。则全部返回最高权限。。
            if (roleInfo.Any(a => a.IsSuperAdmin))
            {
                param.Checks.ForEach(a=>dataList.Add( PermissionValue.VE));
                return  new SuccessResult<List<PermissionValue>>() { Data = dataList };
            }
            var userInfoKey = ConfigService.GetUserInfoRedisKey(currentUser.Token, param.SysCode);
            var userInfo = redisCache.Get<UserInfoAndRoleRight>(userInfoKey);
            if (userInfo.RoleMenus == null || userInfo.RoleMenus.Count <= 0)
            {
                throw new CustomException("NoPermission", "无权限");
            }
 
            var cacheTime = Convert.ToInt32(BaseCore.AppSetting["CacheTime"]);
            //获取到user的所有RoleRight信息.防止频繁读取数据库
            var userRoleRight = TryToGetFromRedis(ConfigService.GetDBUserRightRedisKey(param.SysCode,currentUser.AccountName,currentUser.UserID), cacheTime,
                () =>
                {
                    var userRoleRel = readOnlyContext.UserRoleRels.Where(a => a.UserID == currentUser.UserID)
                                  .Include(a => a.Role).ThenInclude(a => a.RoleRights).ToList();
                    var roles = userRoleRel.Select(a => a.Role).ToList();

                    var userRight = new List<RoleRight>();
                    roles.ForEach(a =>
                    {
                        a.RoleRights.ToList().ForEach(m => m.Role = null);
                        userRight.AddRange(a.RoleRights.ToList());
                    });
                    return userRight;
                });

            foreach (var p in param.Checks)
            {
                //code为空/roleRight无数据返回0权限
                if (p.Code.IsNullOrWhiteSpace()|| userRoleRight == null || userRoleRight.Count <= 0)
                {
                    dataList.Add(PermissionValue.None);
                    continue;
                }

                if (p.Type == RightType.Menu)
                {
                    //查询出所有有权限的菜单id
                    var menuIds = userRoleRight.Where(a => a.RightType == p.Type).Select(a => new { a.MenuID, a.PermissionValue });
                    //防止频繁读取数据库
                    var menuInfo = TryToGetFromRedis(ConfigService.GetDBMenuRedisKey(param.SysCode), cacheTime,
                         () =>
                      {
                          var data = readOnlyContext.Menus.Where(a => !a.IsDisabled).Include(a => a.AppSystem)
                          .Where(a => !a.AppSystem.IsDisabled && a.AppSystem.SystemCode == param.SysCode).ToList();
                          data.ForEach(a =>
                          {
                              a.AppSystem = null;
                              a.AuthorizationObjects = null;
                              a.Parent = null;
                              a.RoleRights = null;
                          });
                          return data;
                      });
                    //菜单表中查询有权限的id中本次的code是否存在
                    var menu = menuInfo.FirstOrDefault(a => menuIds.Select(m => m.MenuID).Contains(a.ID) && !a.IsDisabled && a.MenuCode == p.Code);
                    if (menu != null)
                    {
                        dataList.Add(menuIds.FirstOrDefault(a => a.MenuID == menu.ID).PermissionValue);
                    }
                    else
                    {
                        dataList.Add(PermissionValue.None);
                    }
                }
                else
                {                   
                    //查询出所有有权限的数据功能id
                    var objIds = userRoleRight.Where(a => a.RightType == p.Type)?.Select(a => new { a.AuthorizationObjectID, a.PermissionValue });
                    //获取当前系统中所有authorizationObject数据
                    var obj = TryToGetFromRedis(ConfigService.GetDBAuthObjRedisKey(param.SysCode), cacheTime,
                        () =>
                        {
                            var data = readOnlyContext.AuthorizationObjects.Include(a => a.Menu).ThenInclude(a => a.AppSystem)
                                        .Where(a => a.Menu.AppSystem.SystemCode == param.SysCode).ToList();
                            data.ForEach(a =>
                            {
                                a.RoleRights = null;
                                a.Menu = null;
                            });
                            return data;
                        });
                    if (obj != null&& objIds!=null&&objIds.Count()>0)
                    {
                        //从列表中查出当前code对应的obj
                        var currentObj = obj.FirstOrDefault(a => (int)a.ObjectType == (int)p.Type &&  a.ObjectCode==p.Code);
                        dataList.Add(objIds.FirstOrDefault(a => a.AuthorizationObjectID == currentObj.ID).PermissionValue);
                    }
                    else
                    {
                        dataList.Add(PermissionValue.None);
                    }
                }
            }
            return new SuccessResult<List<PermissionValue>>() { Data =dataList};
    }
        #endregion 权限校验

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<Menu>> GetMenuPage(GetMenuPage param)
        {
            var iQueryable = GetIQueryable(a => true);
            if (param.SystemID.HasValue)
            {
                iQueryable = iQueryable.Where(a => a.SystemID == param.SystemID);
            }
            if (param.MenuCode.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.MenuCode.Contains(param.MenuCode));
            }
            if (param.MenuName.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.MenuName.JsonToObj<MultipleLanguage>().local_Lang.Contains(param.MenuName)
                || a.MenuName.JsonToObj<MultipleLanguage>().en_US.Contains(param.MenuName));
            }
            if (param.IsDisabled.HasValue)
            {
                iQueryable = iQueryable.Where(a => a.IsDisabled == param.IsDisabled);
            }
            var data = iQueryable.ToPage(param.PageIndex, param.PageSize);
            return new SuccessResult<Page<Menu>> { Data = data };
        }

    }
}
