using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using TEG.SSO.Common;
using TEG.SSO.Entity.DTO;
using TEG.SSO.LogDBContext;
using TEG.SSO.Service;

namespace TEG.SSO.AdminWeb.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private IServiceProvider _svp;
        public GlobalExceptionFilter(IServiceProvider svp)
        {
            _svp = svp;
        }
        public void OnException(ExceptionContext context)
        {
            //自定义错误是可预见的异常， 仅用于返回请求方信息提示
            if (context.Exception is CustomException)
            {
                var ex = (CustomException)context.Exception;
                context.Result = new JsonResult(new FailResult { Code = ex.Info.Code, Msg = ex.Info.Message });
            }
            else
            {
                context.Result = new JsonResult(new FailResult() { Code = "UnkownError", Msg = "服务器内部错误" });

                var logService = (LogService)_svp.GetService(typeof(LogService));
                logService.ErrorLog(context);
            }
           var identityCheck= (IdentityCheckAttribute)context.Filters.ToList().FirstOrDefault(a => a.GetType() == typeof(IdentityCheckAttribute));
            if (identityCheck.Redirect)
            {
                context.HttpContext.Response.Redirect("/");
                return;
            }
        }
    }
}
