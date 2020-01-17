using Microsoft.AspNetCore.Mvc;
using TEG.SSO.AdminService;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.AdminWeb.Controllers
{
    public class UserController :BaseController
    {
        AccountService _accountService;
        public UserController(AccountService accountService)
        {
            _accountService = accountService;
        }
        public IActionResult UserList()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetUserList(GetUserPager param)
        {
            return Json(_accountService.GetUserList(param));
        }
    }
}