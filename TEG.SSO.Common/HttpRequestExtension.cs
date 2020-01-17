using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
//using TEG.Framework.Security;

namespace TEG.SSO.Common
{
    public  static  class HttpRequestaxtension
    {
        private static readonly string Secret = "9nsDgVzz";
        /// <summary>
        /// 获取请求参数
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public  static string GetRequestParam(this HttpRequest request)
        {
            // post 请求方式获取请求参数方式
            if (request.Method.ToLower().Equals("post"))
            {
                try
                {
                    if (request.Body.Length > 0)
                    {
                        request.EnableBuffering();
                        request.Body.Position = 0;
                        Stream stream = request.Body;
                        byte[] buffer = new byte[request.ContentLength.Value];
                        stream.Read(buffer, 0, buffer.Length);
                        string querystring = Encoding.UTF8.GetString(buffer);
                        return querystring;
                    }
                    else if (request.Form.Count > 0)
                    {
                        return request.Form.ToJson();
                    }
                    return  string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
            else
            {
                return request.QueryString.Value;
            }
        }

        #region cookie操作
        private const string CookieKey = "token";


        /// <summary>
        /// 从cookie中拿到信息并解密
        /// </summary>
        /// <param name="accessor"></param>
        /// <returns></returns>
        public static string GetCookie(this HttpContext httpContext, string cookieKey = CookieKey)
        {
            if (string.IsNullOrWhiteSpace(cookieKey))
            {
                return "";
            }
            return DEncrypt.Decrypt(httpContext.Request.Cookies[cookieKey], key: Secret);
        }

        /// <summary>
        /// 将信息加密保存到cookie
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        public static bool SetCookie(this HttpContext httpContext, string cookieValue, string cookieKey = CookieKey)
        {
            if (string.IsNullOrWhiteSpace(cookieValue) || string.IsNullOrWhiteSpace(cookieKey))
            {
                return false;
            }
            httpContext.Response.Cookies.Append(CookieKey, DEncrypt.Encrypt(cookieValue,key: Secret), new CookieOptions()
            {
                Expires = DateTime.Now.AddHours(1),
                HttpOnly = true,
                Secure = false
            });

            return true;
        }

        public static void DeleteCookie(this HttpContext httpContext, string cookieKey = CookieKey)
        {
            httpContext.Response.Cookies.Delete(cookieKey);
        }
        #endregion


    }
}
