using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using TEG.Framework.Security.SSO;
using TEG.SSO.Common;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.AdminWeb.Filter
{
    public class IdentityCheckAttribute :Attribute, IActionFilter
    {
       public bool Redirect { get; private set; }
        public IdentityCheckAttribute(bool redirect = true)
        {
            Redirect = redirect;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.GetCookie();        
            if (token.IsNullOrWhiteSpace())
            {
                if (Redirect)
                {
                    context.HttpContext.Response.Redirect("/");
                    return;
                }
                else
                {
                    context.Result = new JsonResult(new FailResult { Code = "NotLogin", Msg = "未知身份" });
                    return;
                }
            }
        }
    }
}
