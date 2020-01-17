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

        //public List<Entity.DTO.AuthRight> GetRoleMenuByUserID(int userId)
        //{
        //    return null;
        //}

        #region 权限校验
        /// <summary>
        /// 检查对应code 的权限值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<List<PermissionValue>> CheckPermission(CheckPermission param)
        {
            //防止频繁读取数据库
            var roleInfo = TryToGetFromRedis(ConfigService.GetDBUserRoleRedisKey(currentUser.AccountName, currentUser.UserID),
                 () =>
                 {
                     var user = masterContext.Users.Where(a => a.AccountName == currentUser.AccountName)
                                                                                    .Include(a => a.UserRoleRels).ThenInclude(a => a.Role).FirstOrDefault();
                     return user.UserRoleRels.AsQueryable().Select(a => a.Role).ToList();
                 });
            var dataList = new List<PermissionValue>();
            //当前用户角色中如果有超级管理员角色。则全部返回最高权限。。
            if (roleInfo.Any(a => a.IsSuperAdmin))
            {
                param.Data.ForEach(a => dataList.Add(PermissionValue.VE));
                return new SuccessResult<List<PermissionValue>>() { Data = dataList };
            }
            var userInfoKey = ConfigService.GetUserInfoRedisKey(currentUser.Token, param.SysCode);
            var userInfo = redisCache.Get<UserInfoAndRoleRight>(userInfoKey);
            if (userInfo.RoleMenus == null || userInfo.RoleMenus.Count <= 0)
            {
                //throw new CustomException("NoPermission", "无权限");
                return new SuccessResult<List<PermissionValue>>();
            }

            //获取到user的所有RoleRight信息.防止频繁读取数据库
            var userRoleRight = TryToGetFromRedis(ConfigService.GetDBUserRightRedisKey(param.SysCode, currentUser.AccountName, currentUser.UserID),
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

            foreach (var p in param.Data)
            {
                //code为空/roleRight无数据返回0权限
                if (p.Code.IsNullOrWhiteSpace() || userRoleRight == null || userRoleRight.Count <= 0)
                {
                    dataList.Add(PermissionValue.None);
                    continue;
                }

                if (p.IsMenu)
                {
                    //查询出所有有权限的菜单id
                    var menuIds = userRoleRight.Where(a => a.IsMenu).Select(a => new { a.MenuID, a.PermissionValue });
                    //防止频繁读取数据库
                    var menuInfo = TryToGetFromRedis(ConfigService.GetDBMenuRedisKey(param.SysCode),
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
                    var objPermissions = userRoleRight.Where(a => !a.IsMenu)?.Select(a => new { a.AuthorizationObjectID, a.PermissionValue }).ToList();
                    //获取当前系统中所有authorizationObject数据
                    var obj = TryToGetFromRedis(ConfigService.GetDBAuthObjRedisKey(param.SysCode),
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
                    if (obj != null && objPermissions != null && objPermissions.Count() > 0)
                    {
                        //从列表中查出当前code对应的obj
                        var currentObj = obj.FirstOrDefault(a =>a.ObjectCode == p.Code);
                        if (currentObj == null)
                        {
                            dataList.Add(PermissionValue.None);
                        }
                        else
                        {
                            var per = objPermissions.FirstOrDefault(a => a.AuthorizationObjectID == currentObj.ID);
                            dataList.Add(per == null? PermissionValue.None: per.PermissionValue);
                        }
                    }
                    else
                    {
                        dataList.Add(PermissionValue.None);
                    }
                }
            }
            return new SuccessResult<List<PermissionValue>>(dataList);
        }
        #endregion 权限校验

        #region menu数据管理
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<Menu>> GetMenuPage(GetMenuPage param)
        {
            var iQueryable = GetIQueryable(a => true);
            if (param.Data.SystemID.HasValue)
            {
                iQueryable = iQueryable.Where(a => a.SystemID == param.Data.SystemID);
            }
            if (param.Data.MenuCode.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.MenuCode.Contains(param.Data.MenuCode));
            }
            if (param.Data.MenuName.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.MenuName.JsonToObj<MultipleLanguage>().local_Lang.Contains(param.Data.MenuName)
                || a.MenuName.JsonToObj<MultipleLanguage>().en_US.Contains(param.Data.MenuName));
            }
            if (param.Data.IsDisabled.HasValue)
            {
                iQueryable = iQueryable.Where(a => a.IsDisabled == param.Data.IsDisabled);
            }
            var data = iQueryable.ToPage(param.Data.PageIndex, param.Data.PageSize);
 
            data.List.ForEach(a =>
            {
                a.MenuName = a.MenuName.JsonToObj<MultipleLanguage>().GetContent(param.Lang);
            });
            return new SuccessResult<Page<Menu>> { Data = data };
        }

        /// <summary>
        ///获取指定系统的所有菜单以及树状图
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<List<SystemMenuTrees>> GetAllMenuTrees(SystemCode param)
        {
            if (param.Data.SysCodes == null || param.Data.SysCodes.Count <= 0)
            {
                throw new CustomException("InvalidArguments", "参数错误");
            }
            var app = readOnlyContext.AppSystems.Where(a => param.Data.SysCodes.Contains(a.SystemCode)).Include(a => a.Menus).ThenInclude(a => a.Children).Include(a => a.Menus).ThenInclude(a => a.AuthorizationObjects).ToList();
            if (app == null || app.Count() <= 0)
            {
                throw new CustomException("InvalidArguments", "系统信息不存在");
            }
            //获取指定系统以及包含的menu和menu的子节点
            var data = app.Select(a => new SystemMenuTrees
            {
                SysCode = a.SystemCode,
                SysName = a.SystemName,
                MenuTrees = a.Menus.Where(m => !m.IsDisabled).OrderBy(m => m.SortID).ToList()
                                            .MapTo<List<MenuTree>>().OrderBy(m => m.SortID).ToList(),
            }).ToList();

            #region  递归处理菜单多语言以及排序问题
            Action<List<MenuTree>> act = null;
            act = s =>
                {
                    if (s == null || s.Count <= 0)
                    {
                        return;
                    }
                    s = s.OrderBy(a => a.SortID).ToList();
                    s.ForEach(a =>
                    {
                        a.MenuName = a.MenuName.JsonToObj<MultipleLanguage>().GetContent(param.Lang);
                        if (a.ChildrenAuthObj != null && a.ChildrenAuthObj.Count() > 0)
                        {
                            a.ChildrenAuthObj.ForEach(m => m.ObjName = m.ObjName.JsonToObj<MultipleLanguage>().GetContent(param.Lang));
                        }
                        //if (a.ChildrenMenus != null && a.ChildrenMenus.Count > 0)
                        //{
                        //    a.ChildrenMenus = a.ChildrenMenus.OrderBy(m => m.SortID).ToList();
                        //    act(a.ChildrenMenus);
                        //}
                    });
                };
            #endregion  递归处理菜单多语言以及排序问题

            data.ForEach(a => act(a.MenuTrees));
            return new SuccessResult<List<SystemMenuTrees>> { Data = data };
        }

        /// <summary>
        /// 删除指定菜单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> DeleteMenuAsync(DeleteTree param)
        {
            var idNotExist = param.Data.IDs.Any(a => !masterContext.Menus.Any(m => m.ID == a));
            if (idNotExist)
            {
                throw new CustomException("MenuIDError", "含有错误的菜单ID");
            }
            //如果不一起删除children，存在下级菜单或下级数据时不可删除
            if (!param.Data.RemoveChildren)
            {
                var childrenMenuExist = masterContext.Menus.Any(a => param.Data.IDs.Any(m => a.ParentID == m));
                if (childrenMenuExist)
                {
                    throw new CustomException("ChildrenMenuExist", "有下级菜单存在，不可删除");
                }
                var childrenAuthObjExist = masterContext.AuthorizationObjects.Any(a => param.Data.IDs.Any(m => m == a.MenuId));
                if (childrenAuthObjExist)
                {
                    throw new CustomException("ChildrenAuthObjExist", "有下级功能数据存在，不可删除");
                }
            }
            var deleteEntities = masterDbSet.Where(a => param.Data.IDs.Contains(a.ID)).ToList();
            masterDbSet.RemoveRange(deleteEntities);
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> AddMenuAsync(AddMenu param)
        {
            var parentNotExist = param.Data.Any(a => a.ParentID.HasValue && !readOnlyDbSet.Any(m => m.ID == a.ParentID));
            if (parentNotExist)
            {
                throw new CustomException("ParentIDError", "含有错误的父级菜单ID");
            }

            var menus = new List<Menu>();
            var sysIDNotExist = param.Data.Any(a => !readOnlyContext.AppSystems.Any(m => m.ID == a.SystemID));
            if (sysIDNotExist)
            {
                throw new CustomException("SystemIDError", "含有错误的系统ID");
            }
            var paramMenuCode = param.Data.Select(m => m.MenuCode);
            var codeExist = readOnlyDbSet.Any(a => paramMenuCode.Contains(a.MenuCode));
            if (codeExist)
            {
                throw new CustomException("MenuCodeError", "含有重复的菜单编码");
            }
            var insertData = param.Data.MapTo<List<Menu>>();
            insertData.ForEach(a=>a.LastUpdateAccountName=currentUser.AccountName);
            await masterDbSet.AddRangeAsync(insertData);
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> UpdateMenusAsync(UpdateMenu param)
        {
            //参数中同级重名判断
            var nameParamExist = param.Data.Any(a => param.Data.Any(m => m.ID != a.ID && m.ParentID == a.ParentID && m.MenuName == a.MenuName));
            if (nameParamExist)
            {
                throw new CustomException("NameIsExist", "同级名称重复");
            }
            var idNotExist = param.Data.Any(a => !masterContext.Menus.Any(m => m.ID == a.ID));
            if (idNotExist)
            {
                throw new CustomException("IDError", "ID不存在");
            }

            var newMenuCodeList = param.Data.Select(a => a.MenuCode);
            var codeExist = newMenuCodeList.GroupBy(a => a).Any(a => a.Count() > 1) || param.Data.Any(a => masterDbSet.Any(m =>m.ID!=a.ID&&m.MenuCode==a.MenuCode));// masterContext.Menus.Any(m=> newMenuCodeList.Contains(m.MenuCode)) ;
            if (codeExist)
            {
                throw new CustomException("CodeError", "编码重复");
            }
            //自己是自己的上级部门或者自己和下级部门互为上下级
            var parentError = param.Data.Any(a => a.ID == a.ParentID) || masterContext.Menus.Any(a => param.Data.Any(m => m.ParentID == a.ID && m.ID == a.ParentID));
            if (parentError)
            {
                throw new CustomException("ParentError", "上级菜单错误");
            }
            //db中同级重名判断
            var nameExist = param.Data.Any(a => masterContext.Menus.Any(m => m.ID != a.ID && m.ParentID == a.ParentID && m.MenuName == a.MenuName.ToJson()));
            if (nameExist)
            {
                throw new CustomException("MenuNameIsExist", "同级菜单名称已存在");
            }
            //参数中有任何一个非空不存在的parentid，就不往下执行
            var parentNotExist = param.Data.Any(a => a.ParentID != null && !masterContext.Menus.Any(m => m.ID == a.ParentID));
            if (parentNotExist)
            {
                throw new CustomException("ParentIDError", "含有错误的上级菜单ID");
            }
            var utcNow = DateTime.UtcNow;
            foreach (var e in param.Data)
            {
                var m = await masterDbSet.FirstOrDefaultAsync(a => a.ID == e.ID);
                m.MenuCode = e.MenuCode;
                m.ModifyTime = utcNow;
                m.MenuName = e.MenuName.ToJson();
                m.ParentID = e.ParentID;
                m.LastUpdateAccountName = currentUser.AccountName;
            }
            await masterContext.SaveChangesAsync();

            return new SuccessResult { Msg = "更新成功" };
        }

        #endregion menu数据管理

    }
}
