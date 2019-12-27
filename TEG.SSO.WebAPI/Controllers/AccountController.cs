﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Filter;
using TEG.SSO.Common;

namespace TEG.SSO.WebAPI.Controllers
{
    /// <summary>
    /// 账号相关模块
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        UserService _userService;
        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        #region  登录注销、找回密码接口
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [CustomAuthorize("登录", "Login", false)]
        public async Task<ActionResult<Result<UserInfoAndRoleRight>>> LoginAsync(UserLogin userLogin)
        {
            return await _userService.LoginAsync(userLogin);
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("Logout")]
        [CustomAuthorize("注销", "Logout")]
        public async Task<ActionResult<Result>> LogoutAsync(RequestBase param)
        {
            return await _userService.LogoutAsync(param.SysCode);
        }

        /// <summary>
        /// 当前用户通过旧密码修改自己密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        [CustomAuthorize( "修改自己密码", "ChangePassword")]
        public async Task<ActionResult<Result>> ChangePasswordAsync(ChangePassword param)
        {
            return await _userService.ChangPasswordAsync(param);
        }

        /// <summary>
        /// 管理员修改指定用户的密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("AdminChangePassword")]
        [CustomAuthorize("管理员修改用户密码","AdminChangePassword")]
        public async Task<ActionResult<Result>> ChangePasswordByUserIdAsync(AdminChangePassword param)
        {
            return await _userService.ChangePasswordByUserIDAsync(param);
        }
        /// <summary>
        /// 找回密码,发送验证码。
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("RetrievePassword")]
        [CustomAuthorize( "邮件找回密码", "RetrievePassword", false)]
        public async Task<ActionResult<Result>> RetrievePasswordAsync(RetrievePassword param)
        {
            var template = param.Lang == Language.local_Lang ? Resource.EmailVerificationCode_zh_CN : Resource.EmailVerificationCode_en_US;
            return await _userService.RetrievePasswordAsync(param, template);
        }
        /// <summary>
        /// 通过邮件重置密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        [CustomAuthorize("邮件验证码重置密码", "ResetPassword", false)]
        public async Task<ActionResult<Result>> ResetPasswordAsync(ResetPassword param)
        {
            return await _userService.ResetPasswordAsync(param);
        }
        #endregion 登录注销、找回密码接口

        #region 用户信息接口
        /// <summary>
        ///  获取当前用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetCurrentUserInfo")]
        [CustomAuthorize("获取当前用户信息", "GetCurrentUserInfo")]
        public ActionResult<Result<UserInfoAndRoleRight>> GetCurrentUserInfo(RequestBase param)
        {
            return _userService.GetCurrentUserInfo(param.SysCode);
        }

        /// <summary>
        /// 获取指定用户的信息
        /// </summary>
        /// <param name="userIDList"></param>
        /// <returns></returns>
        [HttpPost("GetUserInfos")]
        [CustomAuthorize("获取指定UserId的用户信息", "GetUserInfos")]
        public async Task<ActionResult<Result<List<User>>>> GetUserInfoAsync(RequestIDs userIDList)
        {
            return await _userService.GetUserInfoAsync(userIDList);
        }

        /// <summary>
        /// 分页获取用户信息列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetUserByPage")]
        [CustomAuthorize("分页获取用户信息列表", "GetUserByPage")]
        public ActionResult<Result<Page<User>>> GetUserByPage(GetUserPage param)
        {
            var data = _userService.GetPage(param);
            return data;
        }

        /// <summary>
        /// 检查登录名是否已被占用
        /// </summary>
        /// <param name="param"></param>
        /// <returns>是否存在用户名：true-存在，false-不存在</returns>
        [HttpPost("CheckAccountIsExist")]
        [CustomAuthorize("检测用户名是否被占用", "CheckAccountIsExist", false)]
        public  ActionResult<Result<bool>> CheckAccountIsExistAsync(AccountNameParam param)
        {
            return  _userService.IsExist(a => a.AccountName == param.AccountName);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UpdateUser")]
        [CustomAuthorize("更新用户信息", "UpdateUser")]
        public async Task<ActionResult<Result>> UpdateUserAsync(UpdateUsers param)
        {
           return  await  _userService.UpdateUsersAsync(param);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("AddUser")]
        [CustomAuthorize("新增用户", "AddUser")]
        public async Task<ActionResult<Result>> AddUserAsync(NewUserList param)
        {
            return await _userService.AddUsersAsync(param);
        }

        /// <summary>
        /// 禁用指定用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DisableUser")]
        [CustomAuthorize("禁用用户", "DisableUser")]
        public async Task<ActionResult<Result>> DisableUserAsync(RequestIDs param)
        {
            return await  _userService.DisableOrEnableUserAsync(param,true);
        }

        /// <summary>
        /// 启用指定用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("EnableUser")]
        [CustomAuthorize("启用用户", "EnableUser")]
        public async Task<ActionResult<Result>> EnableUserAsync(RequestIDs param)
        {
            return await _userService.DisableOrEnableUserAsync(param,false);
        }

        /// <summary>
        /// 删除指定用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DeleteUser")]
        [CustomAuthorize("删除用户", "DeleteUser")]
        public async Task<ActionResult<Result>> DeleteUserAsync(RequestIDs param)
        {
            //其外键关联数据会联合删除
             await _userService.DeleteManyAsync(a=> param.IDs.Contains(a.ID));
            return new SuccessResult();
        }
        #endregion  用户信息接口
    }
}