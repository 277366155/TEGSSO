using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEG.Framework.Security.SSO;
using TEG.Framework.Standard.Cache;
using TEG.SSO.Common;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Param;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Controllers;

namespace TEG.SSO.WebAPI.Filter
{
    public class CustomAuthorizeAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            //throw new NotImplementedException();
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var author = context.HttpContext.Request.Headers["Authorization"];
            var param = context.HttpContext.Request.GetRequestParam().JsonToObj<RequestBase>();
            //author为空或不以bearer开头
            if (string.IsNullOrWhiteSpace(author) || !author.FirstOrDefault().Contains("Bearer"))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "未知身份" });
                return;
            }
            //提取token
            var token = author.ToString().Substring("Bearer".Length).Trim();
            List<string> list;

            //token无法解密，不再查询redis。 
            if (!SSOHelper.IsTokenValid(token, out list))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "非法token" });
                return;
            }
            //生成token 的时间加上token生效的时间
            if (Convert.ToDateTime(list[4]).AddMinutes(Convert.ToDouble(BaseCore.Configuration.GetSection("AppSetting:TokenOverTime").Value))<DateTime.Now)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "token已过期" });
                return;
            }
            //解析token获取用户信息
            var tokenUserInfo = new TokenUserInfo { UserID=Convert.ToInt32(list[0]), AccountName=list[1], UserName=list[2], IP=list[3], Token= token };
            ((BaseController)context.Controller).CurrentUser = tokenUserInfo;
           
            ////token生成时的ip与当前请求ip不一致
            //if (tokenUserInfo.IP != context.HttpContext.Connection.RemoteIpAddress.ToString())
            //{
            //    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    context.Result = new JsonResult(new { msg = "非法请求" });
            //    return;
            //}

            //context.Controller
            var redisCache = (RedisCache)context.HttpContext.RequestServices.GetService(typeof(RedisCache));
           var userInfoKey= ConfigService.GetUserInfoRedisKey(token, param.SysCode);
            //token+sysCode组成的key是否存在
            if (!redisCache.Exists(userInfoKey))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "无权限" });
                return;
            }
        }
    }
}
