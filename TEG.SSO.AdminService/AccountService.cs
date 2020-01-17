using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using TEG.Framework.Security.SSO;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.AdminService
{
    public class AccountService : BaseService
    {
        /// <summary>
        /// 登录接口
        /// </summary>
        static string LoginUrl = ApiHost + "/api/Account/Login";
        /// <summary>
        /// 获取当前用户的基本信息和菜单信息
        /// </summary>
        static string GetCurrentUserInfoAndRoleRightUrl = ApiHost + "/api/Account/GetCurrentUserInfo";
        /// <summary>
        /// 注销接口
        /// </summary>
        static string LogoutUrl = ApiHost + "/api/Account/Logout";
        /// <summary>
        /// 找回密码接口
        /// </summary>
        static string RetrievePasswordUrl = ApiHost + "/api/Account/RetrievePassword";
        /// <summary>
        /// 重置密码
        /// </summary>
        static string ResetPasswordUrl = ApiHost + "/api/Account/ResetPassword";
        /// <summary>
        /// 获取用户邮箱地址
        /// </summary>
        static string GetUserEmailUrl = ApiHost + "/api/Account/GetUserEmail";

        public AccountService(IHttpContextAccessor accessor) : base(accessor)
        {

        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result Login(UserLoginParam param)
        {
            var pwd = DEncrypt.Decrypt(param.Password, key: "");
            param.Password = SSOHelper.EncryptPassword(pwd, param.LoginName);
            var result = Post<string>(LoginUrl, param);
            if (result.IsSuccess && result.Data.IsNotNullOrWhiteSpace())
            {
                CurrentConetxt.SetCookie(result.Data);
            }
            return result;
        }
        
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public Result Logout()
        {
            var data = Post(LogoutUrl,null,true);
            if (data.IsSuccess)
            {
                   CurrentConetxt.DeleteCookie();
            }
            return data;
        }

        /// <summary>
        /// 获取当前用户的基本信息和菜单信息
        /// </summary>
        /// <returns></returns>
        public UserInfoAndRoleRight GetCurrentUserMenus()
        {
            var data = Post<UserInfoAndRoleRight>( GetCurrentUserInfoAndRoleRightUrl,null,true);
            if (data.IsSuccess)
            {
                return data.Data;
            }
            else
            {
                throw new CustomException(data.Code,data.Msg);
            }
        }

        public Result RetrievePassword(RetrievePasswordParam param)
        {
            return Post(RetrievePasswordUrl,param);
        }

        public Result ResetPassword(ResetPasswordParam param)
        {
            param.NewPassword = DEncrypt.Decrypt(param.NewPassword, key: "");
            param.ConfirmPassword = DEncrypt.Decrypt(param.ConfirmPassword, key: "");

            param.NewPassword = SSOHelper.EncryptPassword(param.NewPassword,param.AccountName);
            param.ConfirmPassword = SSOHelper.EncryptPassword(param.ConfirmPassword, param.AccountName);
            return Post(ResetPasswordUrl, param);
        }

        public string GetUserEmail(string accountName)
        {
            var data = Post<string>(GetUserEmailUrl,accountName);
            if (data.IsSuccess)
            {
                return data.Data;
            }
            else
            {
                throw new CustomException(data.Code,data.Msg);
            }
        }
        #region 用户信息管理
        static string GetUserListUrl =ApiHost+ "/api/Account/GetUserByPage";
       /// <summary>
       /// 获取用户信息列表
       /// </summary>
       /// <param name="param"></param>
       /// <returns></returns>
        public Result<Page<User>> GetUserList(GetUserPager param)
        {
            return  Post<Page<User>>(GetUserListUrl,param,true);
        }

        #endregion
    }
}
