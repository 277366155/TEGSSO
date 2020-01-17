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
    public class RoleController : BaseController
    {
        RoleService _roleService;
        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        #region 角色信息单表数据管理
        /// <summary>
        /// 分页获取角色信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetRolePage")]
        [CustomAuthorize(Description = "分页获取角色信息",ActionCode = "GetRolePage")]
        public ActionResult<Result<Page<Role>>> GetRolePage(GetRolePage param)
        {
            return _roleService.GetRolePage(param);
        }
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetRoleByIDs")]
        [CustomAuthorize(Description = "根据id获取指定角色信息",ActionCode = "GetRoleByIDs")]
        public async Task<ActionResult<Result<List<RoleAndRightInfo>>>> GetRoleByIDsAsync(RequestID param)
        {
            return await _roleService.GetRoleByIDsAsync(param);
        }

        /// <summary>
        /// 新增角色。角色名称不可重复
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddRoles")]
        [CustomAuthorize(Description = "新增角色",ActionCode = "AddRoles")]
        public async Task<ActionResult<Result>> AddRolesAsync(AddRole param)
        {
            return  await  _roleService.AddRolesAsync(param);
        }
        /// <summary>
        /// 更新角色信息。
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateRoles")]
        [CustomAuthorize(Description = "更新角色信息",ActionCode = "UpdateRoles")]
        public async Task<ActionResult<Result>> UpdateRolesAsync(UpdateRole param)
        {
            return await  _roleService.UpdateRolesAsync(param);
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DeleteRoles")]
        [CustomAuthorize(Description = "删除指定角色",ActionCode = "DeleteRoles")]
        public async Task<ActionResult<Result>> DeleteRolesAsync(DeleteTree param)
        {
            return await  _roleService.DeleteRolesAsync(param);
        }
        #endregion 角色信息单表数据管理

    }
}