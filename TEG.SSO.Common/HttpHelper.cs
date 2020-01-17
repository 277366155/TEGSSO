using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TEG.SSO.Common
{
    public class HttpHelper
    {
        #region 模拟请求       
        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Post(RequestDataParam param, string certPath = "", string certPwd = "")
        {
            var resultStr = "";
            //创建一个http请求
            var request = (HttpWebRequest)WebRequest.Create(param.Url);
            //request.Proxy = new WebProxy("http://127.0.0.1:8888", false);

            //设置请求类型
            request.Method = "POST";
            if (param.Authorization.IsNotNullOrWhiteSpace())
            {
                request.Headers.Add("Authorization", "Bearer " + param.Authorization.Trim());
            }
            //请求内容格式
            request.ContentType = param.ContentType;
            //request.Accept = "*/*";
            //request.AllowAutoRedirect = false;
            if (param.Url.Trim().ToLower().IndexOf("https") == 0)
            {
                //当请求为https时，验证服务器证书
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
                if (!string.IsNullOrEmpty(certPath) && !string.IsNullOrEmpty(certPwd))
                {
                    X509Certificate2 cer = new X509Certificate2(certPath, certPwd,
                        X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                    request.ClientCertificates.Add(cer);
                }
            }
            //获取http请求的流/管道
            var requestStream = request.GetRequestStream();
            var requestData = param.RequestData.ToJson();

            //创建一个写流数据的对象
            using (var tw = new StreamWriter(requestStream))
            {
                //向流中写数据
                tw.Write(requestData);
            }
            //发起请求获取响应结果
            var rs = (HttpWebResponse)request.GetResponse();

            //获取响应结果的流/管道
            using (var getStream = rs.GetResponseStream())
            {
                using (var rd = new StreamReader(getStream, Encoding.GetEncoding(param.Encode)))
                {
                    // 读取返回结果流信息
                    resultStr = rd.ReadToEnd();
                }
            }
            return resultStr;
        }

        public static TR Post<TR>(RequestDataParam param, string certPath = "", string certPwd = "")
        {
            var resultStr =Post(param, certPath, certPwd);

            return resultStr.JsonToObj<TR>();
        }

        /// <summary>
        /// get请求
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string Get(RequestDataParam param)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(param.Url + (string.IsNullOrWhiteSpace(param.RequestData.ToString()) ? "" : "?" + param.RequestData));
            request.Method = "GET";
            request.ContentType = param.ContentType;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(param.Encode));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        /// <summary>
        /// 文件post上传功能
        /// </summary>
        /// <param name="param"></param>
        /// <param name="certpath"></param>
        /// <param name="certpwd"></param>
        /// <returns></returns>
        public static string PostUpload(UploadRequestParam param, string certpath = "", string certpwd = "")
        {
            var resultStr = "";
            //创建一个http请求
            var request = (HttpWebRequest)WebRequest.Create(param.Url);
            //request.Proxy = new WebProxy("http://127.0.0.1:8888", false);

            //设置请求类型
            request.Method = "POST";

            //request.Accept = "*/*";
            //request.AllowAutoRedirect = false;
            if (param.Url.Trim().ToLower().IndexOf("https") == 0)
            {
                //当请求为https时，验证服务器证书
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback((a, b, c, d) => true);
                if (!string.IsNullOrEmpty(certpath) && !string.IsNullOrEmpty(certpwd))
                {
                    X509Certificate2 cer = new X509Certificate2(certpath, certpwd,
                        X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                    request.ClientCertificates.Add(cer);
                }
            }

            //var requestData = param.RequestData;
            var boundary = $"----{DateTime.Now.Ticks.ToString("x")}";
            var formData = $"\r\n--{boundary}\r\nContent-Disposition:form-data;name=\"{param.TypeName}\";filename=\"{param.FileName}\"\r\nContent-Type:{param.ContentType}\r\n\r\n";
            var formDataBytes = Encoding.ASCII.GetBytes(param.InputStream.Length == 0 ? formData.Substring(2, formData.Length - 2) : formData);
            //创建一个写流数据的对象
            var tw = new MemoryStream();

            //向流中写入文件头
            tw.Write(formDataBytes, 0, formDataBytes.Length);

            //向流中写数据
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            while ((bytesRead = param.InputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                tw.Write(buffer, 0, bytesRead);
            }
            var footer = Encoding.ASCII.GetBytes($"\r\n--{boundary}--\r\n");
            tw.Write(footer, 0, footer.Length);
            //请求内容格式
            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.ContentLength = tw != null ? tw.Length : 0;

            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.KeepAlive = true;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.57 Safari/537.36";

            if (tw != null)
            {
                tw.Position = 0;

                var requestStream = request.GetRequestStream();
                buffer = new byte[1024];
                bytesRead = 0;
                while ((bytesRead = tw.Read(buffer, 0, buffer.Length)) != 0)
                {
                    requestStream.Write(buffer, 0, bytesRead);
                }
                tw.Close();
            }

            //发起请求获取响应结果
            var rs = (HttpWebResponse)request.GetResponse();

            //获取响应结果的流/管道
            using (var getStream = rs.GetResponseStream())
            {
                using (var rd = new StreamReader(getStream, Encoding.GetEncoding(param.Encode)))
                {
                    // 读取返回结果流信息
                    resultStr = rd.ReadToEnd();
                }
            }
            return resultStr;
        }
        #endregion
    }
}
