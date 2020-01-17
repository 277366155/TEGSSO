using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TEG.SSO.AdminService;
using TEG.SSO.AdminWeb.Filter;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.AdminWeb.Controllers
{
    [IdentityCheck]
    public class HomeController : BaseController
    {
        AccountService _accountService;
        public HomeController(AccountService accountService, IServiceProvider svp) : base(svp)
        {
            _accountService = accountService;
        }

        public IActionResult Index()
        {
          
            return View();
        }
        public IActionResult Main()
        {
            var userInfoAndMenus = _accountService.GetCurrentUserMenus();
            ViewBag.Menus = userInfoAndMenus.RoleMenus;
            ViewBag.UserInfo = userInfoAndMenus.UserInfo.AccountName;
            if (userInfoAndMenus.Dept != null && userInfoAndMenus.Dept.Count > 0)
            {
                ViewBag.UserInfo = userInfoAndMenus.UserInfo.AccountName + $"({userInfoAndMenus.Dept.FirstOrDefault().DeptName})";
            }
          
            return View();
        }


        [HttpPost]
        public ActionResult<Result> Logout()
        {
            return Json(_accountService.Logout());
        }
    }
}