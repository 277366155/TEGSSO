using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class MenuController : BaseController
    {
        MenuService _menuService;
        AuthorizationObjectService _authObjService;
        public MenuController(MenuService menuService, AuthorizationObjectService authObjService)
        {
            _menuService = menuService;
            _authObjService = authObjService;
        }
        #region 菜单管理
        /// <summary>
        /// 分页获取菜单信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetMenuPage")]
        [CustomAuthorize(Description = "分页获取菜单信息列表", ActionCode ="GetMenuPage")]
        public ActionResult<Result<Page<Menu>>> GetMenuPage(GetMenuPage param)
        {
            return _menuService.GetMenuPage(param);
        }

        /// <summary>
        /// 根据id获取指定菜单信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetMenuByIDs")]
        [CustomAuthorize(Description = "根据id获取指定菜单信息",ActionCode = "GetMenuByIDs")]
        public async Task<ActionResult<Result<List<Menu>>>> GetMenuByIDsAsync(RequestID param)
        {
            var data = await _menuService.GetListAsync(a => param.Data.IDs.Contains(a.ID));
            data.ForEach(a => a.MenuName = a.MenuName.JsonToObj<MultipleLanguage>().GetContent(param.Lang));
            return new SuccessResult<List<Menu>> { Data = data };
        }

        /// <summary>
        /// 获取指定系统下所有菜单树状信息（包含菜单下的数据和功能）//角色赋权限tree数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAllMenuTrees")]
        [CustomAuthorize(Description = "获取所有菜单树状信息（包含菜单下的数据和功能）",ActionCode = "GetAllMenusTree")]
        public ActionResult<Result<List<SystemMenuTrees>>> GetAllMenuTreesBySystemCode(SystemCode param)
        {
            return _menuService.GetAllMenuTrees(param);
        }

        /// <summary>
        /// 获取当前登录系统的所有菜单树信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetCurrentSysMenuTrees")]
        [CustomAuthorize(Description = "获取当前登录系统的所有菜单树信息（包含菜单下的数据和功能）",ActionCode = "GetCurrentSysMenuTrees")]
        public ActionResult<Result<List<SystemMenuTrees>>> GetCurrentSysMenuTrees(RequestBase param)
        {
            var sysParam = param.MapTo<SystemCode>();
            sysParam.Data.SysCodes = new List<string> ();
            sysParam.Data.SysCodes.Add(param.SysCode);
            return _menuService.GetAllMenuTrees(sysParam);
        }

        /// <summary>
        /// 删除指定菜单.
        /// </summary>
        /// <returns></returns>
        [HttpPost("DeleteMenu")]
        [CustomAuthorize(Description = "删除指定菜单",ActionCode = "DeleteMenu")]
        public async Task<ActionResult<Result>> DeleteMenuAsync(DeleteTree param)
        {
            return await _menuService.DeleteMenuAsync(param);
        }
        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddMenu")]
        [CustomAuthorize(Description = "新增菜单", ActionCode ="AddMenu")]
        public async Task<ActionResult<Result>> AddMenuAsync(AddMenu param)
        {
            return await _menuService.AddMenuAsync(param);
        }
        /// <summary>
        /// 更新指定菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateMenu")]
        [CustomAuthorize(Description = "更新指定菜单", ActionCode ="UpdateMenu")]
        public async Task<ActionResult<Result>> UpdateMenuAsync(UpdateMenu param)
        {
            return await _menuService.UpdateMenusAsync(param);
        }
        #endregion 菜单管理

        #region 菜单中数据、功能管理
        /// <summary>
        /// 分页获取数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAuthObjectPage")]
        [CustomAuthorize(Description = "分页获取数据功能项",ActionCode = "GetAuthObjectPage")]
        public ActionResult<Result<Page<AuthorizationObject>>> GetAuthObjectPage(GetAuthObjPage param)
        {
            return _authObjService.GetAuthObjPage(param);
        }
        /// <summary>
        /// 获取指定数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAuthObjectByIDs")]
        [CustomAuthorize(Description = "获取指定数据功能项",ActionCode = "GetAuthObjectByIDs")]
        public async Task<ActionResult<Result<List<AuthorizationObject>>>> GetAuthObjectByIDsAsync(RequestID param)
        {
            var data = await _authObjService.GetListAsync(a => param.Data.IDs.Contains(a.ID));
            data.ForEach(a => a.ObjectName = a.ObjectName.JsonToObj<MultipleLanguage>().GetContent(param.Lang));
            return new SuccessResult<List<AuthorizationObject>> { Data = data };
        }
        /// <summary>
        /// 获取指定菜单下的数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAuthObjectByMenuIDs")]
        [CustomAuthorize(Description = "获取指定菜单下的数据功能项",ActionCode = "GetAuthObjectByMenuIDs")]
        public async Task<ActionResult<Result<List<AuthorizationObject>>>> GetAuthObjectByMenuIDsAsync(RequestID param)
        {
            var data = await _authObjService.GetListAsync(a => a.MenuId.HasValue && param.Data.IDs.Contains(a.MenuId.Value));
            data.ForEach(a => a.ObjectName = a.ObjectName.JsonToObj<MultipleLanguage>().GetContent(param.Lang));
            return new SuccessResult<List<AuthorizationObject>> { Data = data };
        }

        /// <summary>
        /// 删除指定数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("DeleteAuthObject")]
        [CustomAuthorize(Description = "删除指定数据功能项",ActionCode = "DeleteAuthObject")]
        public async Task<ActionResult<Result>> DeleteAuthObjectAsync(RequestID param)
        {
            return await  _authObjService.DeleteAuthObjectsAsync(param);
        }
        /// <summary>
        /// 新增数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddAuthObject")]
        [CustomAuthorize(Description = "新增数据功能项", ActionCode ="AddAuthObject")]
        public async Task<ActionResult<Result>> AddAuthObjectAsync(AddAuthObj param)
        {
            return await _authObjService.AddAuthObjAsync(param);
        }
        /// <summary>
        /// 更新数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateAuthObjects")]
        [CustomAuthorize(Description = "更新数据功能项", ActionCode ="UpdateAuthObjects")]
        public async Task<ActionResult<Result>> UpdateAuthObjectsAsync(UpdateAuthObj param)
        {
            return await _authObjService.UpdateAuthObjectsAsync(param);
        }
        #endregion 菜单中数据、功能管理

    }
}