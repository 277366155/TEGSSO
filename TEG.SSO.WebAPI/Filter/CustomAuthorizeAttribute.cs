using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TEG.Framework.Security.SSO;
using TEG.Framework.Standard.Cache;
using TEG.SSO.Common;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;
using TEG.SSO.LogDBContext;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Controllers;

namespace TEG.SSO.WebAPI.Filter
{
    /// <summary>
    /// 权限过滤器
    /// </summary>
    public class CustomAuthorizeAttribute : Attribute, IActionFilter
    {
        /// <summary>
        /// 功能项码
        /// </summary>
        public string ActionCode=null;
        /// <summary>
        /// 文本说明
        /// </summary>
        public string Description;
        /// <summary>
        /// 是否启用token验证
        /// </summary>
        public bool Verify=true;
        /// <summary>
        /// 是否进行actionCode权限验证
        /// </summary>
        public bool CheckPermission = true;
        /// <summary>
        /// 是否记录访问日志
        /// </summary>
        bool _saveLog;

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="saveLog">是否记录访问日志</param>
        public CustomAuthorizeAttribute( bool saveLog = true)
        {
            //if (!actionCode.IsNullOrWhiteSpace())
            //{
            //    ActionCode = actionCode;
            //}

            //Description = des;
            //Verify = verify;
            //CheckPermission = checkPermission;
            _saveLog = saveLog;
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
            if (ActionCode.IsNullOrWhiteSpace())
            {
                ActionCode = context.RouteData.ToString();
            }

            var actionCode = ActionCode.IsNullOrWhiteSpace() ? context.ActionDescriptor.AttributeRouteInfo.Template.Replace("/", "_") : ActionCode;

            //所有接口都要以post方式请求
            var param = context.HttpContext.Request.GetRequestParam().JsonToObj<RequestBase>();
            var log = new Operation
            {
                AccessHost = context.HttpContext.Connection.RemoteIpAddress.ToString(),
                SystemCode = param.SysCode,
                ActionCode = actionCode,
                Description = param.SysCode + "/" + Description,
                Url = context.ActionDescriptor.AttributeRouteInfo.Template
            };
            var userinfo = ((BaseController)context.Controller).CurrentUser;
            if (userinfo != null)
            {
                log.UserID = userinfo.UserID;
                log.AccountName = userinfo.AccountName;
                log.UserToken = userinfo.Token;
            }
            var svp = context.HttpContext.RequestServices;
            if (_saveLog)
            {
                Task.Run(() =>
                {
                    using (var scop = svp.CreateScope())
                    {
                        var logService = scop.ServiceProvider.GetService<LogService>();
                        //不可await,不能影响主流业务
                        logService.OperationLogAsync(log);
                    }
                });
            }
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
                context.Result = new JsonResult(new FailResult { Code = "PermissionDenied", Msg = "非法请求" });
            }
            var appSystemService = (AppSystemService)context.HttpContext.RequestServices.GetService(typeof(AppSystemService));
            var appCodeIsExist = appSystemService.CheckExistFromCache(param.SysCode);
            if (!appCodeIsExist)
            {
                context.Result = new JsonResult(new FailResult { Code = "PermissionDenied", Msg = "非法请求" });
            }
            //todo:此处后续可以优化为使用RSA校验方式
            #endregion 校验sysCode

            //不进行token验证
            if (!Verify)
            {
                return;
            }
            var author = context.HttpContext.Request.Headers["Authorization"];
            #region token格式校验
            //author为空或不以bearer开头
            if (string.IsNullOrWhiteSpace(author) || !author.FirstOrDefault().Contains("Bearer"))
            {
                context.Result = new JsonResult(new FailResult { Code = "NotLogin", Msg = "未知身份" });
                return;
            }
            //提取token
            var token = author.ToString().Substring("Bearer".Length).Trim();
            List<string> list;

            //token无法解密，不再查询redis。 
            if (!SSOHelper.IsTokenValid(token, out list))
            {
                context.Result = new JsonResult(new FailResult { Code = "NotLogin", Msg = "未知身份" });
                return;
            }
            var tokenOverTime = Convert.ToDouble(BaseCore.AppSetting.GetSection("TokenOverTime").Value);

            #endregion token格式校验

            //解析token获取用户信息
            var tokenUserInfo = new TokenUserInfo { UserID = Convert.ToInt32(list[0]), AccountName = list[1], UserName = list[2], IP = list[3], Token = token };
            ((BaseController)context.Controller).CurrentUser = tokenUserInfo;

            ////token生成时的ip与当前请求ip不一致
            //if (tokenUserInfo.IP != context.HttpContext.Connection.RemoteIpAddress.ToString())
            //{
            //    
            //    context.Result = new JsonResult(new { msg = "非法请求" });
            //    return;
            //}

            #region token有效期校验
            //redis中不存在，则已过期
            var redisCache = (RedisCache)context.HttpContext.RequestServices.GetService(typeof(RedisCache));
            var userInfoKey = ConfigService.GetUserInfoRedisKey(token, param.SysCode);

            var userIsExist = redisCache.Exists(userInfoKey);
            if (!userIsExist)
            {
                context.Result = new JsonResult(new FailResult { Code = "NotLogin", Msg = "请重新登录" });
                return;
            }

            //缓存信息剩余时间小于10分钟，自动续期
            var userInfoTTL = redisCache.redis.KeyTimeToLive(userInfoKey).GetValueOrDefault().TotalMinutes;
            var tokenRedisKey = ConfigService.GetTokenRedisKey(tokenUserInfo.UserID.ToString(), tokenUserInfo.AccountName, param.SysCode);
            var tokenTTL = redisCache.redis.KeyTimeToLive(tokenRedisKey).GetValueOrDefault().TotalMinutes;
            if ((userInfoTTL > 0 && userInfoTTL < 10) || (tokenTTL > 0 && tokenTTL < 10))
            {
                redisCache.Set(tokenRedisKey, token, TimeSpan.FromMinutes(tokenOverTime));
                redisCache.Set(userInfoKey, redisCache.Get<UserInfoAndRoleRight>(userInfoKey), TimeSpan.FromMinutes(tokenOverTime));
            }
            #endregion token有效期校验

            #region actionCode权限验证
            if (!CheckPermission)
            {
                return;
            }
            var menuService = (MenuService)context.HttpContext.RequestServices.GetService(typeof(MenuService));
            var checkPermission = new CheckPermission() { Data = new List<CheckParam>(), SysCode = param.SysCode, Lang = param.Lang };
            checkPermission.Data.Add(new CheckParam { Code = ActionCode, IsMenu = false });
            var checkResult = menuService.CheckPermission(checkPermission);
            //验证失败，或者当前用户在当前action上无执行权限
            if (!checkResult.IsSuccess || ((int)(checkResult.Data.FirstOrDefault() & PermissionValue.Executable) != (int)PermissionValue.Executable))
            {
                context.Result = new JsonResult(new FailResult { Code = "PermissionDenied", Msg = "无权限" });
                return;
            }
            #endregion action权限验证
        }
    }
}
