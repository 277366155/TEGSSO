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
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Controllers;

namespace TEG.SSO.WebAPI.Filter
{
    public class CustomAuthorizeAttribute : Attribute, IActionFilter
    {
        string _actionCode;
        string _description;
        bool _verify;
        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="des">文本说明</param>
        /// <param name="actionCode">当前action的编码</param>
        /// <param name="verify">是否启用验证，默认启用</param>
        public CustomAuthorizeAttribute(string des, string actionCode=null,bool verify=true)
        {
            if (!actionCode.IsNullOrWhiteSpace())
            {
                _actionCode = actionCode;
            }

            _description = des;
            _verify = verify;
        }
        /// <summary>
        /// action执行后，记录日志
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (_actionCode.IsNullOrWhiteSpace())
            {
                _actionCode = context.RouteData.ToString();
            }

            var actionCode = _actionCode.IsNullOrWhiteSpace() ? context.ActionDescriptor.AttributeRouteInfo.Template.Replace("/", "_") : _actionCode;
            var logService = (LogService)context.HttpContext.RequestServices.GetService(typeof(LogService));
            //所有接口都要以post方式请求
            var param = context.HttpContext.Request.GetRequestParam().JsonToObj<RequestBase>();          
            var log = new Operation {
                AccessHost = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                SystemCode = param.SysCode,
                ActionCode = actionCode,
                Description = param.SysCode + "/" + _description,
                 Url= context.ActionDescriptor.AttributeRouteInfo.Template
            };
            var userinfo = ((BaseController)context.Controller).CurrentUser;
            if (userinfo != null)
            {
                log.UserID = userinfo.UserID;
                log.AccountName = userinfo.AccountName;
                log.UserToken = userinfo.Token;
            }
            //不可await,不能影响主流业务
            logService.OperationLogAsync(log);
        }

        /// <summary>
        /// action执行前
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            #region 校验sysCode
            var param = context.HttpContext.Request.GetRequestParam().JsonToObj<RequestBase>();
            if (param == null)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "非法请求" });
            }
            var   appSystemService = (AppSystemService)context.HttpContext.RequestServices.GetService(typeof(AppSystemService));
           var appCodeIsExist=   appSystemService.CheckExist(a=>a.SystemCode==param.SysCode);
            if (!appCodeIsExist)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "非法请求" });
            }
            //todo:此处后续可以优化为使用RSA校验方式
            #endregion 校验sysCode
            
            //不进行权限验证
            if (!_verify)
            {
                return;
            }
            var author = context.HttpContext.Request.Headers["Authorization"];
            #region token格式校验
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
            if (Convert.ToDateTime(list[4]).AddMinutes(Convert.ToDouble(BaseCore.AppSetting.GetSection("TokenOverTime").Value))<DateTime.Now)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "token已过期" });
                return;
            }
            #endregion token格式校验

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

            #region token有效期校验
            //redis中不存在，则已过期
            var redisCache = (RedisCache)context.HttpContext.RequestServices.GetService(typeof(RedisCache));
           var userInfoKey= ConfigService.GetUserInfoRedisKey(token, param.SysCode);
            var userIsExist = redisCache.Exists(userInfoKey);
            
            if (!userIsExist)
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "无权限" });
                return;
            }
            #endregion token有效期校验

            #region actionCode权限验证

            var menuService = (MenuService)context.HttpContext.RequestServices.GetService(typeof(MenuService));
            var checkPermission = new CheckPermission() { Checks = new List<CheckParam>(), SysCode = param.SysCode, Lang = param.Lang };
            checkPermission.Checks.Add(new CheckParam { Code = _actionCode, Type = RightType.Function });
            var checkResult = menuService.CheckPermission(checkPermission);
            //验证失败，或者当前用户在当前action上无执行权限
            if (!checkResult.IsSuccess || ((int)(checkResult.Data.FirstOrDefault() & PermissionValue.Executable) != (int)PermissionValue.Executable))
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Result = new JsonResult(new { msg = "无权限" });
                return;
            }
            #endregion action权限验证
        }
    }
}
