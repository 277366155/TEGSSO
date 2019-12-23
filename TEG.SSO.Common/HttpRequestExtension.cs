using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TEG.SSO.Common
{
 public  static  class HttpRequestExtension
    {

        public  static string GetRequestParam(this HttpRequest request)
        {

            // post 请求方式获取请求参数方式
            if (request.Method.ToLower().Equals("post"))
            {
                try
                {
                    request.EnableBuffering();
                    request.Body.Position = 0;

                    Stream stream = request.Body;
                    byte[] buffer = new byte[request.ContentLength.Value];
                    stream.Read(buffer, 0, buffer.Length);
                    string querystring = Encoding.UTF8.GetString(buffer);
                    return querystring;
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
    }
}
