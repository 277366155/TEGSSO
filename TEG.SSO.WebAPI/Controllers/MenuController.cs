using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        [CustomAuthorize("分页获取菜单信息列表", "GetMenuPage")]
        public ActionResult<Result<Page<Menu>>> GetMenuPage(GetMenuPage param)
        {
            return _menuService.GetMenuPage(param);
        }

        /// <summary>
        /// 根据id获取指定菜单信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetMenuByIDs")]
        [CustomAuthorize("根据id获取指定菜单信息", "GetMenuByIDs")]
        public async Task<ActionResult<Result<List<Menu>>>> GetMenuByIDsAsync(RequestIDs param)
        {
            var data =await  _menuService.GetListAsync(a=>param.IDs.Contains(a.ID));
            return  new SuccessResult<List<Menu>> {  Data= data };
        }
        /// <summary>
        /// 获取所有菜单树状信息（包含菜单下的数据和功能）
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAllMenusTree")]
        [CustomAuthorize("获取所有菜单树状信息（包含菜单下的数据和功能）", "GetAllMenusTree")]
        public ActionResult GetAllMenusTree()
        {
            return null;
        }
        /// <summary>
        /// 删除指定菜单. todo:用一个参数标识：有下级菜单存在时，是否同时删除下级菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("DeleteMenu")]
        [CustomAuthorize("删除指定菜单", "DeleteMenu")]
        public ActionResult DeleteMenu()
        {
            return null;
        }
        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddMenu")]
        [CustomAuthorize("新增菜单", "AddMenu")]
        public ActionResult AddMenu()
        {
            return null;
        }
        /// <summary>
        /// 更新指定菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateMenu")]
        [CustomAuthorize("更新指定菜单", "UpdateMenu")]
        public ActionResult UpdateMenu()
        {
            return null;
        }


        #endregion 菜单管理

        #region 菜单中数据、功能管理
        /// <summary>
        /// 分页获取数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAuthObjectPage")]
        [CustomAuthorize("分页获取数据功能项", "GetAuthObjectPage")]
        public ActionResult GetAuthObjectPage()
        {
            return null;
        }
        /// <summary>
        /// 获取指定数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAuthObjectByIDs")]
        [CustomAuthorize("获取指定数据功能项", "GetAuthObjectByIDs")]
        public ActionResult GetAuthObjectByIDs()
        {
            return null;
        }
        /// <summary>
        /// 删除指定数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("DeleteAuthObject")]
        [CustomAuthorize("删除指定数据功能项", "DeleteAuthObject")]
        public ActionResult DeleteAuthObject()
        {
            return null;
        }
        /// <summary>
        /// 新增数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddAuthObject")]
        [CustomAuthorize("新增数据功能项", "AddAuthObject")]
        public ActionResult AddAuthObject()
        {
            return null;
        }
        /// <summary>
        /// 更新数据功能项
        /// </summary>
        /// <returns></returns>
        [HttpPost("UpdateAuthObject")]
        [CustomAuthorize("更新数据功能项", "UpdateAuthObject")]
        public ActionResult UpdateAuthObject()
        {
            return null;
        }
        #endregion 菜单中数据、功能管理

    }
}