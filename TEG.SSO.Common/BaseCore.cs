using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TEG.SSO.Common
{
    public static class BaseCore
    {

        private static IConfigurationBuilder _builder;
        private static object lockObj = new object();
        /// <summary>
        /// 默认读取appsetting.json文件
        /// </summary>
        /// <param name="act">可添加其他json配置文件</param>
        /// <returns></returns>
        public static IConfigurationBuilder InitConfigurationBuilder(Action<IConfigurationBuilder> act = null)
        {

            if (_builder == null)
            {
                lock (lockObj)
                {
                    if (_builder == null)
                    {
                        var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        _builder = builder;
                    }
                }
            }
            act?.Invoke(_builder);

            return _builder;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        public static IConfigurationRoot Configuration
        {
            get
            {
                return InitConfigurationBuilder().Build();
            }
        }

        /// <summary>
        /// 读取根节点下的appsetting节点
        /// </summary>
        public static IConfigurationSection AppSetting
        {
            get
            {
                return Configuration.GetSection("AppSetting");
            }
        }

        public static void EnableCookie(this HttpContext httpContext)
        {
            if (httpContext == null)
            {
                return;
            }
            var consentFeature = httpContext.Features.Get<ITrackingConsentFeature>();
            consentFeature.GrantConsent();
        }
    }
}
