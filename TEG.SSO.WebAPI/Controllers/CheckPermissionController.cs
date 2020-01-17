using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Filter;

namespace TEG.SSO.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckPermissionController : BaseController
    {
        MenuService _menuService;
        public CheckPermissionController(MenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// 校验当前用户在指定code中的权限值。todo:反复测试。。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [CustomAuthorize(false, Description = "权限校验",ActionCode = "CheckPermission_Index", CheckPermission=false)]
        public  ActionResult<Result<List<PermissionValue>>> Index(CheckPermission param)
        {
            return  _menuService.CheckPermission(param);
        }
    }
}