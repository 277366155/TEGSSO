using Microsoft.AspNetCore.Mvc;
using System;
using TEG.SSO.AdminService;
using TEG.SSO.Common;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.AdminWeb.Controllers
{
    public class DefaultController : BaseController
    {
        AccountService _accountService;
        SecurityQuestionService _questionService;
        public DefaultController(AccountService accountService, SecurityQuestionService questionService)
        {
            _accountService = accountService;
            _questionService = questionService;
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult ForgetPassword()
        {
            ViewBag.Questions = _questionService.GetQuestionList();
            return View();
        }

        public IActionResult ResetPassowrd()
        {
            var param = Request.Query["id"].ToString();
            ViewBag.AccountName = "";
            if (param.IsNotNullOrWhiteSpace())
            {
                ViewBag.AccountName = DEncrypt.Decrypt(param, key: "");
                string email = _accountService.GetUserEmail(ViewBag.AccountName.ToString());
                var index = email.IndexOf("@");
                var emailAccount = email.Substring(0, index);
                if (emailAccount.Length <= 4)
                {
                    ViewBag.Email = email.Substring(0, 1) + "*" + email.Substring(index);
                }
                else
                {
                    ViewBag.Email = email.Substring(0, 2) + "***" + email.Substring(index - 2);
                }
            }
            return View();
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login([FromForm]UserLoginParam param)
        {
            return Json(_accountService.Login(param));
        }

        /// <summary>
        /// 找回密码接口，发送邮件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RetrievePassword([FromForm]RetrievePasswordParam param)
        {
            return Json(_accountService.RetrievePassword(param));
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ResetPassword([FromForm]ResetPasswordParam param)
        {
            return Json(_accountService.ResetPassword(param));
        }
    }
}