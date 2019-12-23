using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Common;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Service
{
    public static class ConfigService
    {
        /// <summary>
        /// token的过期时间，单位：分钟
        /// </summary>
        public static int TokenOverTime
        {
            get
            {
                return Convert.ToInt32(BaseCore.Configuration.GetSection("AppSetting:TokenOverTime").Value);
            }
        }
        /// <summary>
        /// 获取存储token信息的key
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="accountName"></param>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public static string GetTokenRedisKey(string userId, string accountName, string systemCode)
        {
            return $"{systemCode}:Token:{userId}_{accountName}";
        }

        /// <summary>
        /// 获取存储user信息的key
        /// </summary>
        /// <param name="token"></param>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public static string GetUserInfoRedisKey(string token,string systemCode)
        {
            return $"{systemCode}:UserInfo:{token}";
        }

        /// <summary>
        /// 获取验证码缓存key
        /// </summary>
        /// <param name="accountName">用户登录名</param>
        /// <param name="type">类型</param>
        /// <param name="systemCode">系统code</param>
        /// <returns></returns>
        public static string GetVerificationCodeRedisKey(string accountName, VerificationCodeType type,string systemCode)
        {
            return $"{systemCode}:VerificationCode:{type.ToString()}:{accountName}";
        }

    }
}
