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
    public class OrganizationService : ServiceBase<Organization>
    {
        public OrganizationService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        #region 部门信息查询
        /// <summary>
        /// 根据id获取部门和子tree信息。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result<List<DeptAndChildrenInfo>>> GetDeptInfoListByIDAsync(RequestIDs param)
        {
            var data = await GetDeptsAndChildrenByIDsAsync(param.IDs);
            return new SuccessResult<List<DeptAndChildrenInfo>> { Data = data };
        }

        /// <summary>
        /// 根据id 获取部门（包含下级部门）信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        private async Task<List<DeptAndChildrenInfo>> GetDeptsAndChildrenByIDsAsync(List<int> ids)
        {
            if (ids == null || ids.Count <= 0)
            {
                return null;
            }
            var dataList = new List<DeptAndChildrenInfo>();


            //遍历所有部门id
            foreach (var id in ids)
            {
                var dept = await readOnlyDbSet.Where(m => m.ID == id).Include(m => m.Children).FirstOrDefaultAsync();
                if (dept == null)
                {
                    dataList.Add(null);
                    continue;
                }
                var currentDept = dept.MapTo<DeptAndChildrenInfo>();
                if (currentDept.Children != null && currentDept.Children.Count > 0)
                {
                    //递归处理当前部门下级部门的子部门
                    Action<DeptAndChildrenInfo> act = null;
                    act = s =>
                    {
                        var entity = readOnlyDbSet.Where(m => m.ID == s.DeptID).Include(m => m.Children).FirstOrDefault();
                        if (entity.Children != null && entity.Children.Count > 0)
                        {
                            s.Children.AddRange(entity.MapTo<DeptAndChildrenInfo>().Children);
                            s.Children.ForEach(m => act(m));
                        }
                    };

                    currentDept.Children.ForEach(m => act(m));
                }
                dataList.Add(currentDept);
            }
            return dataList;
        }

        /// <summary>
        /// 获取当前用户的部门信息（包含上级部门信息）
        /// </summary>
        /// <returns></returns>
        public async Task<Result<List<DeptAndParentInfo>>> GetCurrentDeptsAsync()
        {
            return new SuccessResult<List<DeptAndParentInfo>> { Data = await GetCurrentUserDeptsAndParentsAsync() };
        }

        /// <summary>
        /// 获取当前用户的部门信息（包含上级部门信息）
        /// </summary>
        /// <returns></returns>
        public async Task<List<DeptAndParentInfo>> GetCurrentUserDeptsAndParentsAsync()
        {
            var depts = readOnlyContext.UserDeptRels.Where(a => a.UserID == currentUser.UserID).Include(a => a.Dept)?.Select(a => a.Dept).Distinct();
            if (depts == null || depts.Count() <= 0)
            {
                //throw new CustomException("CurrentUserNoDept", "当前用户无任何部门");
                //无部门，不应返回错误。
                return null;
            }

            var list = new List<DeptAndParentInfo>();
            foreach (var id in depts.Select(a => a.ID).ToList())
            {
                list.Add(await GetDeptAndParentByIdAsync(id));
            }
            return list;
        }

        /// <summary>
        /// 根据id 获取部门（包含上级级部门）信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<DeptAndParentInfo> GetDeptAndParentByIdAsync(int id)
        {
            var dept = await readOnlyDbSet.Where(m => m.ID == id).Include(m => m.Parent).FirstOrDefaultAsync();
            if (dept == null)
            {
                return null;
            }
            var currentDept = dept.MapTo<DeptAndParentInfo>();
            //递归处理当前部门上级部门的上级部门
            Action<DeptAndParentInfo> act = null;
            act = async s =>
            {
                if (s.Parent == null)
                {
                    return;
                }

                var parent = await readOnlyDbSet.Where(m => m.ID == s.Parent.DeptID).Include(m => m.Parent).FirstOrDefaultAsync();

                if (parent.Parent != null)
                {
                    s.Parent = parent.MapTo<DeptAndParentInfo>();
                    act(s.Parent);
                }
            };
            act(currentDept);
            return currentDept;
        }

        /// <summary>
        /// 分页查询，包含上级部门信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<Organization>> GetPage(DeptPage param)
        {
            var data = GetIQueryable(a => true);
            if (param.ID.HasValue)
            {
                data = data.Where(a => a.ID == param.ID);
            }
            //parentID=0表示查询根节点
            if (param.ParentID.HasValue)
            {
                data = data.Where(a => a.ParentID == (param.ParentID == 0 ? null : param.ParentID));
            }
            if (param.OrgName.IsNotNullOrWhiteSpace())
            {
                data = data.Where(a => a.OrgName.Contains(param.OrgName));
            }
            data = data.Include(a => a.Parent);
            return new SuccessResult<Page<Organization>> { Data = data.ToPage(param.PageIndex, param.PageSize) };
        }
        #endregion 部门信息查询

        /// <summary>
        /// 新增部门信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> AddDeptAsync(AddDepts param)
        {
            //参数中有任何一个非空不存在的parentid，就不往下执行
            var parentNotExist = param.Depts.Any(a => a.ParentID != null && !masterContext.Organizations.Any(m => m.ID == a.ParentID));
            if (parentNotExist)
            {
                throw new CustomException("ParentIDError", "含有错误的上级部门ID");
            }
            //同级重名判断
            var nameExist = param.Depts.Any(a => masterContext.Organizations.Any(m => m.ParentID == a.ParentID && m.OrgName == a.DeptName));
            if (nameExist)
            {
                throw new CustomException("DeptNameIsExist", "同级部门名称已存在");
            }

            var entities = param.Depts.MapTo<List<Organization>>();
            entities.ForEach(a => a.LastUpdateAccountName = currentUser.AccountName);
            await masterContext.Organizations.AddRangeAsync(entities);
            await masterContext.SaveChangesAsync();
            return new SuccessResult { Msg = "添加成功" };
        }

        /// <summary>
        /// 删除指定部门
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> DeleteDeptAsync(DeleteDepts param)
        {
            var idNotExist = param.DeptIDs.Any(a => !masterContext.Organizations.Any(m => m.ID == a));
            if (idNotExist)
            {
                throw new CustomException("DeptIDError", "含有错误的部门ID");
            }
            //如果不一起删除下级部门，存在下级部门时不可删除
            if (!param.RemoveChildren)
            {
                var childrenExist = masterContext.Organizations.Any(a => param.DeptIDs.Any(m => a.ParentID == m));
                if (childrenExist)
                {
                    throw new CustomException("ChildrenExist", "有下级部门，不可删除");
                }
            }
            var deleteEntities = masterDbSet.Where(a => param.DeptIDs.Contains(a.ID)).Include(a => a.Children).ToList();
            masterDbSet.RemoveRange(deleteEntities);
            await masterContext.SaveChangesAsync();
            return new SuccessResult { Msg = "删除成功" };
        }

        /// <summary>
        /// 更新部门信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> UpdateDeptsAsync(UpdateDepts param)
        {

            //参数中同级重名判断
            var nameParamExist = param.Depts.Any(a => param.Depts.Any(m => m.ID != a.ID && m.ParentID == a.ParentID && m.DeptName == a.DeptName));
            if (nameParamExist)
            {
                throw new CustomException("DeptNameIsExist", "同级部门名称重复");
            }
            //自己是自己的上级部门或者自己和下级部门互为上下级
            var parentError = param.Depts.Any(a => a.ID == a.ParentID) || masterContext.Organizations.Any(a => param.Depts.Any(m => m.ParentID == a.ID && m.ID == a.ParentID));
            if (parentError)
            {
                throw new CustomException("ParenError", "上级部门错误");
            }
            //db中同级重名判断
            var nameExist = param.Depts.Any(a => masterContext.Organizations.Any(m => m.ID != a.ID && m.ParentID == a.ParentID && m.OrgName == a.DeptName));
            if (nameExist)
            {
                throw new CustomException("DeptNameIsExist", "同级部门名称已存在");
            }
            //参数中有任何一个非空不存在的parentid，就不往下执行
            var parentNotExist = param.Depts.Any(a => a.ParentID != null && !masterContext.Organizations.Any(m => m.ID == a.ParentID));
            if (parentNotExist)
            {
                throw new CustomException("ParentIDError", "含有错误的上级部门ID");
            }
            var utcNow = DateTime.UtcNow;
            foreach (var e in param.Depts)
            {
                var m = await masterDbSet.FirstOrDefaultAsync(a => a.ID == e.ID);
                m.ModifyTime = utcNow;
                m.OrgName = e.DeptName;
                m.ParentID = e.ParentID;
                m.LastUpdateAccountName = currentUser.AccountName;
            }
            await masterContext.SaveChangesAsync();

            return new SuccessResult { Msg = "更新成功" };
        }

        /// <summary>
        /// 更新用户部门信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> UpdateUserDeptRelAsync(UpdateUserDeptRel param)
        {
            /*
             * 1，删除用户原有部门信息
             * 2，新增用户部门信息。注意检查是否有重复
             * **/
            var userNotExist = param.Rels.Any(a => !masterContext.Users.Any(m => m.ID == a.UserID));
            if (userNotExist)
            {
                throw new CustomException("UserIDError", "用户不存在");
            }

            var deptNotExit = param.Rels.Any(a => !masterContext.Organizations.Any(m => m.ID == a.DeptID));
            if (deptNotExit)
            {
                throw new CustomException("DeptIDError", "部门不存在");
            }

            //删除原用户-部门信息
            var oldRels = masterContext.UserDeptRels.Where(a => param.Rels.Select(m => m.UserID).Contains(a.UserID)).ToList();
            masterContext.UserDeptRels.RemoveRange(oldRels);

            var entities = param.Rels.MapTo<List<Entity.DBModel.UserDeptRel>>();
            entities.ForEach(a => a.LastUpdateAccountName = currentUser.AccountName);
            await masterContext.UserDeptRels.AddRangeAsync(entities);
            await masterContext.SaveChangesAsync();

            return new SuccessResult { Msg = "更新成功" };
        }
    }
}
