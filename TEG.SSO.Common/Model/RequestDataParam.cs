using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TEG.SSO.Common
{

    public class RequestParam
    {
        private string encode = "UTF-8";
        private string contentType = ContentTypes.Json;

        public RequestParam(string url, string contentType = null, string token = null)
        {
            this.Url = url;
            if (contentType.IsNotNullOrWhiteSpace())
            {
                ContentType = contentType;
            }
            if (token.IsNotNullOrWhiteSpace())
            {
                Authorization = token;
            }
        }

        public string Url { get; set; }
        public string Authorization { get; set; }
        /// <summary>
        /// 取ContentType类中只读字段
        /// </summary>
        public string ContentType
        {
            get
            {
                return contentType;
            }
            set
            {
                contentType = value;
            }
        }

        public string Encode
        {
            get
            {
                return encode;
            }
            set
            {
                encode = value;
            }
        }
    }
    // <summary>
    /// 模拟post/get请求参数
    /// </summary>
    public class RequestDataParam : RequestParam
    {
        public RequestDataParam(string url, string contentType) : base(url, contentType)
        { }
        public RequestDataParam(string url, string contentType,string token) : base(url, contentType,token)
        { }
        public RequestDataParam(object data, string url, string contentType, string token) : base(url, contentType,token)
        {
            RequestData = data;
        }
        public object RequestData { get; set; }
    }


    /// <summary>
    /// post上传文件请求参数
    /// </summary>
    public class UploadRequestParam : RequestParam
    {
        public UploadRequestParam(string url) : base(url, ContentTypes.OctetStream)
        { }
        public string TypeName => "media";
        public string FileName { get; set; }
        public Stream InputStream { get; set; }
    }


    public static class ContentTypes
    {
        /// <summary>
        /// post提交application/x-www-form-urlencoded
        /// </summary>
        public readonly static string FormUrlEncoded = "application/x-www-form-urlencoded";
        /// <summary>
        /// post提交application/json
        /// </summary>
        public readonly static string Json = "application/json; charset=utf-8";

        ///// <summary>
        /// post上传文件流
        /// </summary>
        public readonly static string OctetStream = "application/octet-stream";

        /// <summary>
        /// get提交text/html;charset=UTF-8
        /// </summary>
        public readonly static string TextHtml = "text/html;charset=UTF-8";
    }
}
