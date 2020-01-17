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
    public class RoleService : ServiceBase<Role>
    {
        public RoleService(IServiceProvider svp) : base(svp)
        {
        }

        /// <summary>
        /// 分页获取角色信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<Role>> GetRolePage(GetRolePage param)
        {
            var iQueryable = GetIQueryable(a => true);
            if (param.Data.RoleName.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.RoleName.Contains(param.Data.RoleName));
            }
            if (param.Data.IsSuperAdmin.HasValue)
            {
                iQueryable = iQueryable.Where(a => a.IsSuperAdmin == param.Data.IsSuperAdmin);
            }
            if (param.Data.ParentID.HasValue)
            {
                if (param.Data.ParentID == 0)
                {
                    iQueryable = iQueryable.Where(a => a.ParentID == null);
                }
                else
                {
                    iQueryable = iQueryable.Where(a => a.ParentID == param.Data.ParentID);
                }
            }
            var data = iQueryable.ToPage(param.Data.PageIndex, param.Data.PageSize);
            return new SuccessResult<Page<Role>>(data);
        }

        /// <summary>
        /// 根据id获取角色信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result<List<RoleAndRightInfo>>> GetRoleByIDsAsync(RequestID param)
        {
            if (param.Data.IDs.Any(a => !masterDbSet.Any(m => m.ID == a)))
            {
                throw new CustomException("RoleIDError", "含有错误的角色ID");
            }
            var data = await masterDbSet.Where(a => param.Data.IDs.Contains(a.ID)).Include(a => a.RoleRights).ToListAsync();
            return new SuccessResult<List<RoleAndRightInfo>>(data.MapTo<List<RoleAndRightInfo>>());
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> AddRolesAsync(AddRole param)
        {
            if (param.Data.GroupBy(a => a.RoleName).Any(a => a.Count() > 1))
            {
                throw new CustomException("RoleNameError", "角色名称重复");
            }
            var parentNotExist = param.Data.Any(a => a.ParentID.HasValue && !readOnlyDbSet.Any(m => m.ID == a.ParentID));
            if (parentNotExist)
            {
                throw new CustomException("ParentIDError", "含有错误的上级角色ID");
            }
            var newRoleNameList = param.Data.Select(a => a.RoleName);
            var nameIsExist = readOnlyContext.Roles.Any(a => newRoleNameList.Contains(a.RoleName));
            if (nameIsExist)
            {
                throw new CustomException("RoleNameError", "角色名称已存在");
            }
            var insertData = param.Data.MapTo<List<Role>>();
            insertData.ForEach(a => a.LastUpdateAccountName = currentUser.AccountName);
            await InsertManyAsync(insertData.ToArray());
            return new SuccessResult();
        }

        /// <summary>
        /// 更新角色基本信息以及权限信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> UpdateRolesAsync(UpdateRole param)
        {
            if (param.Data.GroupBy(a => a.RoleName).Any(a => a.Count() > 1))
            {
                throw new CustomException("RoleNameError", "角色名称重复");
            }
            if (param.Data.Any(a => !masterDbSet.Any(m => m.ID == a.ID)))
            {
                throw new CustomException("RoleIDError", "角色ID不存在");
            }
            var parentNotExist = param.Data.Any(a => a.ParentID.HasValue && !masterDbSet.Any(m => m.ID == a.ParentID));
            if (parentNotExist)
            {
                throw new CustomException("ParentIDError", "含有错误的上级角色ID");
            }
            var nameIsExist = param.Data.Any(a => masterDbSet.Any(m => m.ID != a.ID && m.RoleName == a.RoleName));//readOnlyContext.Roles.Any(a => newRoleNameList.Contains(a.RoleName));
            if (nameIsExist)
            {
                throw new CustomException("RoleNameError", "角色名称已存在");
            }
            //非超级管理员用户不能添加超级管理员类角色
            if (param.Data.Any(a => a.IsSuperAdmin) && !GetCurrentUserInfoFromRedis().IsSuperAdmin)
            {
                throw new CustomException("OverstepPermission", "越权操作");
            }
            var utcNow = DateTime.UtcNow;
            foreach (var r in param.Data)
            {
                //更新role信息
                var role = masterDbSet.FirstOrDefault(a => a.ID == r.ID);
                role.IsSuperAdmin = r.IsSuperAdmin;
                role.LastUpdateAccountName = currentUser.AccountName;
                role.ModifyTime = utcNow;
                role.ParentID = r.ParentID;
                role.RoleName = r.RoleName;
                //有父级角色，且要受父级角色权限限制
                if (r.ParentID.HasValue && r.ParentLimit)
                {
                    //当前角色的父级角色权限信息
                    var parentRole = TryToGetFromRedis(ConfigService.GetRoleAndRightInfoRedisKey(r.ParentID.Value),
                        () =>
                        {
                            return masterDbSet.Where(a => a.ID == r.ParentID).Include(a => a.RoleRights).FirstOrDefault().MapTo<RoleAndRightInfo>();
                        });
                    //父级角色不是超级管理员时，才进行权限削减
                    if (!parentRole.IsSuperAdmin)
                    {
                        r.IsSuperAdmin = false;

                        var currentRoleRight = new List<RoleRightInfo>();
                        r.RoleRightInfos.ForEach(a => currentRoleRight.Add(a));

                        foreach (var right in r.RoleRightInfos)
                        {
                            //父级权限中不含有对应的权限对象，直接remove
                            if (!parentRole.RoleRightInfos.Any(a => a.IsMenu == right.IsMenu
                                                                        && ((a.MenuID == right.RightID && right.IsMenu) || (a.AuthorizationObjectID == right.RightID && !right.IsMenu))))
                            {
                                currentRoleRight.Remove(currentRoleRight.FirstOrDefault(a => a.IsMenu == right.IsMenu && a.RightID == right.RightID));
                            }
                            //如果含有权限对象，取权限值的“且”运算结果
                            else
                            {
                                var currentParentRight = parentRole.RoleRightInfos.FirstOrDefault(a => a.IsMenu == right.IsMenu
                                                                            && ((a.MenuID == right.RightID && right.IsMenu) || (a.AuthorizationObjectID == right.RightID && !right.IsMenu)));
                                currentRoleRight.FirstOrDefault(a => a.IsMenu == right.IsMenu && a.RightID == right.RightID).PermissionValue = (currentParentRight.PermissionValue & right.PermissionValue);
                            }
                        }
                        r.RoleRightInfos = currentRoleRight;
                    }
                }

                //删除原有roleRight信息
                var roleRights = masterContext.RoleRights.Where(a => a.RoleID == role.ID).ToList();
                masterContext.RoleRights.RemoveRange(roleRights);
                //插入现有roleRight信息.
                //superAdmin可以直接越过权限校验
                if (!role.IsSuperAdmin)
                {
                    var insertRoleRights = r.RoleRightInfos.MapTo<List<RoleRight>>();
                    insertRoleRights.ForEach(a =>
                    {
                        a.LastUpdateAccountName = currentUser.AccountName;
                        a.RoleID = r.ID;
                    });
                    await masterContext.RoleRights.AddRangeAsync(insertRoleRights);
                }
            }
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 删除指定角色
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> DeleteRolesAsync(DeleteTree param)
        {
            if (param.Data.IDs.Any(a => !masterDbSet.Any(m => m.ID == a)))
            {
                throw new CustomException("RoleIDError", "角色ID不存在");
            }
            //外键关联的会自动删除。
            if (!param.Data.RemoveChildren && masterDbSet.Any(a => a.ParentID.HasValue && param.Data.IDs.Contains(a.ParentID.Value)))
            {
                throw new CustomException("ChildrenExist", "存在下级角色不可删除");
            }
            await DeleteManyAsync(a => param.Data.IDs.Contains(a.ID));
            return new SuccessResult();
        }


    }
}
