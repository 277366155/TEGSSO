using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Common;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.AdminService
{
    public class BaseService
    {
        /// <summary>
        /// 当前系统码
        /// </summary>
        protected static string SysCode = BaseCore.AppSetting["SysCode"];
        /// <summary>
        /// api服务地址
        /// </summary>
        protected static string ApiHost = BaseCore.AppSetting["ApiHost"];

        protected HttpContext CurrentConetxt;
        public BaseService(IHttpContextAccessor accelyearr)
        {
            CurrentConetxt= accelyearr.HttpContext;
        }

        /// <summary>
        ///  post请求api服务
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param">业务参数，不包括syscode、language等</param>
        /// <param name="needToken">是否需要header中加入token凭据</param>
        /// <returns></returns>
        public Result<TR> Post<TR>(string url, object param =null,bool needToken=false)
        {
            object request = null;
            if (param == null)
            {
                request = new RequestBase(SysCode);
            }
            else
            {
                request = new RequestBase<object>(param, SysCode);
            }

            return HttpHelper.Post<Result<TR>>(new RequestDataParam(request, url, ContentTypes.Json, needToken ? CurrentConetxt.GetCookie() : null)) ;
        }

        /// <summary>
        /// post请求api服务
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param">业务参数，不包括syscode、language等</param>
        /// <param name="needToken">是否需要header中加入token凭据</param>
        /// <returns></returns>
        public Result Post(string url, object param = null, bool needToken = false)
        {
            object request = null;
            if (param == null)
            {
                request = new RequestBase(SysCode);
            }
            else
            {
                request = new RequestBase<object>(param, SysCode);
            }

            return HttpHelper.Post<Result>(new RequestDataParam(request, url, ContentTypes.Json, needToken ? CurrentConetxt.GetCookie() : null));
        }

    }
}
