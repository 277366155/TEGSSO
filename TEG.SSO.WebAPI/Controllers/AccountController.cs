using System;
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
        [CustomAuthorize]
        public async Task<ActionResult<Result>> LogoutAsync(RequestBase param)
        {
            return await _userService.LogoutAsync(param.SysCode);
        }

        /// <summary>
        /// 找回密码,发送验证码。todo..
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("RetrievePassword")]
        public async Task<ActionResult<Result>> RetrievePasswordAsync(RetrievePassword param)
        {
            var tem = param.Lang == Language.zh_CN ? Resource.EmailVerificationCode_zh_CN : Resource.EmailVerificationCode_en_US;
            return await _userService.RetrievePasswordAsync(param, tem);
        }
        #endregion 登录注销、找回密码接口

        #region 用户信息接口
        /// <summary>
        ///  获取当前用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetCurrentUserInfo")]
        [CustomAuthorize]
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
        [CustomAuthorize]
        public async Task<ActionResult<Result<List<User>>>> GetUserInfoAsync(UserIDList userIDList)
        {
            return await _userService.GetUserInfoAsync(userIDList);
        }

        /// <summary>
        /// 检查登录名是否已被占用
        /// </summary>
        /// <param name="param"></param>
        /// <returns>是否存在用户名：true-存在，false-不存在</returns>
        [HttpPost("CheckAccountIsExist")]
        public async Task<ActionResult<Result<bool>>> CheckAccountIsExistAsync(AccountNameParam param)
        {
            return await _userService.IsExistAsync(a => a.AccountName == param.AccountName);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UpdateUser")]
        [CustomAuthorize]
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
        [CustomAuthorize]
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
        [CustomAuthorize]
        public async Task<ActionResult<Result>> DisableUserAsync(UserIDList param)
        {
            return await  _userService.DisableOrEnableUserAsync(param,true);
        }

        /// <summary>
        /// 启用指定用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("EnableUser")]
        [CustomAuthorize]
        public async Task<ActionResult<Result>> EnableUserAsync(UserIDList param)
        {
            return await _userService.DisableOrEnableUserAsync(param,false);
        }

        /// <summary>
        /// 删除指定用户
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DeleteUser")]
        [CustomAuthorize]
        public async Task<ActionResult<Result>> DeleteUserAsync(UserIDList param)
        {
            //其外键关联数据会联合删除
             await _userService.DeleteManyAsync(a=> param.UserIDs.Contains(a.ID));
            return new SuccessResult();
        }
        #endregion  用户信息接口

     
    }
}