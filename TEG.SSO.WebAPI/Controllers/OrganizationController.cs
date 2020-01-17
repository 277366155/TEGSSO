using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Param;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Filter;

namespace TEG.SSO.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : BaseController
    {
        OrganizationService _organizationService;
        public OrganizationController(OrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        /// 根据id获取部门和下级部门信息。 
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetDeptsAndChildren")]
        [CustomAuthorize(Description = "获取部门信息",ActionCode ="GetDepts")]
        public async  Task<ActionResult<Result<List<DeptAndChildrenInfo>>>> GetDeptsAndChildrenByIDsAsync(RequestID param)
        {
            return await  _organizationService.GetDeptInfoListByIDAsync(param);
        }
        /// <summary>
        /// 根据id获取部门信息 
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetDeptsByIDs")]
        [CustomAuthorize(Description = "获取指定部门信息",ActionCode = "GetDeptsByIDs")]
        public async Task<ActionResult<Result<List<Organization>>>> GetDeptsByIDsAsync(RequestID param)
        {
            return await _organizationService.GetDeptByIDsAsync(param);
        }

        /// <summary>
        /// 获取当前用户部门信息，查询至顶级节点
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetCurrentDepts")]
        [CustomAuthorize(Description = "获取当前用户的部门信息",ActionCode = "GetCurrentDepts")]
        public async Task<ActionResult<Result<List<DeptAndParentInfo>>>> GetCurrentUserDeptsAsync(RequestBase param)
        {
            return  await _organizationService.GetCurrentDeptsAsync();
        }

        /// <summary>
        /// 分页获取部门信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetOrganizationByPage")]
        [CustomAuthorize(Description = "分页获取部门信息", ActionCode ="GetOrganizationByPage")]
        public ActionResult<Result<Page<Organization>>> GetOrganizationByPage(GetDeptPage param)
        {
            return _organizationService.GetPage(param);
        }
        /// <summary>
        /// 更新部门信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UpdateDepts")]
        [CustomAuthorize(Description = "更新部门信息", ActionCode ="UpdateDepts")]
        public async Task<ActionResult<Result>> UpdateOrganizationsAsync(UpdateDept param)
        {
            return await _organizationService.UpdateDeptsAsync(param);
        }
        /// <summary>
        /// 新增部门信息list
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddDeptInfos")]
        [CustomAuthorize(Description = "新增部门信息",ActionCode = "AddDeptInfos")]
        public async Task<ActionResult<Result>> AddDeptInfosAsync(AddDept param)
        {
            return await _organizationService.AddDeptAsync(param);
        }

        /// <summary>
        /// 删除指定部门信息，用参数区分：有下级部门的是否需要先删除下级部门
        /// </summary>
        /// <returns></returns>
        [HttpPost("DeleteDepts")]
        [CustomAuthorize(Description = "删除部门信息", ActionCode ="DeleteDepts")]
        public async Task<ActionResult<Result>> DeleteDeptsAsync(DeleteTree param)
        {
            return await  _organizationService.DeleteDeptAsync(param);
        }

        /// <summary>
        ///更新用户部门信息，删除原有的信息之后新增
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateUserDeptRel")]
        [CustomAuthorize(Description = "更新用户部门信息",ActionCode = "UpdateUserDeptRel")]
        public async Task<ActionResult<Result>> UpdateUserDeptRelAsync(UpdateUserDeptRel param)
        {
            return await  _organizationService.UpdateUserDeptRelAsync(param);
        }
    }
}