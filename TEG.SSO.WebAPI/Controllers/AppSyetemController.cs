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
    public class AppSyetemController : BaseController
    {
        AppSystemService _appService;
        public AppSyetemController(AppSystemService appService)
        {
            _appService = appService;
        }

        /// <summary>
        /// 分页获取系统信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetAppSystemPage")]
        [CustomAuthorize(Description = "分页获取业务系统列表",ActionCode = "GetAppSystemPage")]
        public ActionResult<Result<Page<AppSystem>>> GetAllAppSystem(GetAppSystemPage param)
        {
            return _appService.GetAppSystemPage(param);
        }

        /// <summary>
        /// 获取未禁用的业务系统下拉列表数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetAppSelectItems")]
        [CustomAuthorize(Description = "获取未禁用的业务系统下拉列表数据",ActionCode = "GetAppSelectItems")]
        public async Task<ActionResult<Result<List<AppSystemSelectItem>>>> GetEnableAppSystemListAsync(RequestBase param)
        {
            return await  _appService.GetAppSystemListAsync(false);
        }

        /// <summary>
        /// 获取指定的业务系统信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetAppSystemByIDs")]
        [CustomAuthorize(Description = "获取指定的业务系统信息",ActionCode = "GetAppSystemByIDs")]
        public async Task<Result<List<AppSystem>>> GetAppSystemByIDsAsync(RequestID param)
        {
           var data =await  _appService.GetListAsync(a => param.Data.IDs.Contains(a.ID));
            return new SuccessResult<List<AppSystem>>(data);
        }

        /// <summary>
        /// 新增业务系统信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddAppSystem")]
        [CustomAuthorize(Description = "新增业务系统信息", ActionCode ="AddAppSystem")]
        public async Task<ActionResult<Result>> AddAppSystemAsync(AddAppSystem param)
        {
            return  await _appService.AddAppSystemAsync(param);
        }

        /// <summary>
        /// 删除指定业务系统信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DeleteAppSystem")]
        [CustomAuthorize(Description = "删除指定业务系统信息",ActionCode = "DeleteAppSystem")]
        public async Task<ActionResult<Result>> DeleteAppSystemAsync(DeleteTree param)
        {
            return await  _appService.DeleteAppSystemAsync(param);
        }

        /// <summary>
        /// 禁用指定业务系统信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DisableAppSystem")]
        [CustomAuthorize(Description = "禁用指定业务系统信息", ActionCode ="DisableAppSystem")]
        public async Task<ActionResult<Result>> DisableAppSystemAsync(RequestID param)
        {
            return await  _appService.DisableOrEnableAppSystemAsync(param,true);
        }

        /// <summary>
        /// 启用指定业务系统信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("EnableAppSystem")]
        [CustomAuthorize(Description = "启用指定业务系统信息",ActionCode = "EnableAppSystem")]
        public async Task<ActionResult<Result>> EnableAppSystemAsync(RequestID param)
        {
            return await _appService.DisableOrEnableAppSystemAsync(param, false);
        }
        /// <summary>
        /// 更新业务系统信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateAppSystem")]
        [CustomAuthorize(Description = "更新业务系统信息", ActionCode ="UpdateAppSystem")]
        public async Task<ActionResult<Result>> UpdateAppSystemAsync(UpdateAppSystem param)
        {
            return await _appService.UpdateAppSystemsAsync(param);
        }
    }
}