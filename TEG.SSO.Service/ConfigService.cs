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
                return Convert.ToInt32(BaseCore.AppSetting["TokenOverTime"]);
            }
        }
        /// <summary>
        /// 临时缓存时间。TryToGetFromRedis使用
        /// </summary>
        public static int CacheTime
        {
            get
            {
                var cacheTime = Convert.ToInt32(BaseCore.AppSetting["CacheTime"]);
                return cacheTime;
            }
        }

        /// <summary>
        /// 所有业务系统码缓存
        /// </summary>
        public static string AppSystemCodesKey
        {
            get { return "AllAppSystems:AppSystemCodes"; }
        }

        /// <summary>
        /// 获取存储token信息的key:string
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
        /// 获取存储user信息的key:UserInfoAndRoleRight
        /// </summary>
        /// <param name="token"></param>
        /// <param name="systemCode"></param>
        /// <returns></returns>
        public static string GetUserInfoRedisKey(string token,string systemCode)
        {
            return $"{systemCode}:UserInfo:{token}";
        }

        /// <summary>
        /// 获取验证码缓存key :VerificationCode
        /// </summary>
        /// <param name="accountName">用户登录名</param>
        /// <param name="type">类型</param>
        /// <param name="systemCode">系统code</param>
        /// <returns></returns>
        public static string GetVerificationCodeRedisKey(string accountName, VerificationCodeType type,string systemCode)
        {
            return $"{systemCode}:VerificationCode:{type.ToString()}:{accountName}";
        }

        /// <summary>
        /// 缓存指定用户的角色列表信息:List<Role>
        /// </summary>
        /// <param name="accountName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetDBUserRoleRedisKey(string accountName,int userId)
        {
            return $"DBUserRoleCache:{userId}_{accountName}";
        }

        /// <summary>
        /// 缓存指定角色的包含权限的信息：RoleAndRightInfo
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public static string GetRoleAndRightInfoRedisKey(int roleId)
        {
            return $"DBRoleRightCache:RoleID_{roleId}";
        }

        /// <summary>
        /// 缓存用户在某系统中包括权限值的权限信息:List<RoleRight>
        /// </summary>
        /// <param name="sysCode"></param>
        /// <param name="accountName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetDBUserRightRedisKey(string sysCode, string accountName, int userId)
        {
            return $"{sysCode}:UserRight:{userId}_{accountName}";
        }

        /// <summary>
        /// 缓存指定系统所有菜单信息的redisKey：List<Menu>
        /// </summary>
        /// <param name="sysCode"></param>
        /// <returns></returns>
        public static string GetDBMenuRedisKey(string sysCode)
        {
            return $"{sysCode}:AllMenuList";
        }
        /// <summary>
        ///  缓存指定系统所有功能项信息的redisKey：List<AuthorizationObject>
        /// </summary>
        /// <param name="sysCode"></param>
        /// <returns></returns>
        public static string GetDBAuthObjRedisKey(string sysCode)
        {
            return $"{sysCode}:AllAuthObjList";
        }
    }
}
