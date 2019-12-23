using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TEG.Framework.Security.SSO;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;
using TEG.Framework.Utility;

namespace TEG.SSO.Service
{
    public class UserService : ServiceBase<Entity.DBModel.User>
    {
        public UserService(IServiceProvider svp) : base(svp)
        {
        }
        /// <summary>
        /// 判断密码更新时间是否超出更新周期
        /// </summary>
        /// <param name="lastUpdataTime"></param>
        /// <param name="changeCycle"></param>
        /// <returns></returns>
        private bool IsPswOverdue(DateTime lastUpdataTime, int changeCycle)
        {
            if (changeCycle == 0)
                return false;
            var utcNow = DateTime.UtcNow;
            if (Math.Abs(lastUpdataTime.Subtract(utcNow).Days) > changeCycle)
                return true;
            else
                return false;
        }

        /// <summary>
        ///  登录接口，登录成功返回用户基本信息(不返回权限数据)。
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        public async Task<Result<UserInfoAndRoleRight>> LoginAsync(UserLogin userLogin)
        {
            var result = new Result<UserInfoAndRoleRight>();
            if (userLogin.LoginName.IsNullOrWhiteSpace() || userLogin.EncryptedPassword.IsNullOrWhiteSpace())
            {
                throw new CustomException("IDOrPasswordIsEmpty", "登录账号或密码为空");
            }
            var user = await masterContext.Users.Where(a => a.AccountName == userLogin.LoginName && a.Password == userLogin.EncryptedPassword)
                                                                                .Include(a => a.UserDeptRels)
                                                                                .Include(a => a.UserRoleRels).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new CustomException("IDOrPasswordError", "登录账号或密码错误");
            }
            if (user.IsDisabled)
            {
                throw new CustomException("IDIsDisabled", "账号被禁用");
            }
            var utcNow = DateTime.UtcNow;
            if (user.ValidTime < utcNow)
            {
                throw new CustomException("AccountExpired", "账号超出有效期");
            }
            //密码过期限制登录。通过找回密码修改密码
            if (IsPswOverdue(user.PasswordModifyTime, user.PasswordModifyPeriod))
            {
                throw new CustomException("PasswordExpired", "密码过期");
            }

            //登录的系统信息
            var appSystem = masterContext.AppSystems.FirstOrDefault(a => !a.IsDisabled && a.SystemCode == userLogin.SysCode);
            if (appSystem == null)
            {
                throw new CustomException("SystemError", "系统信息有误");
            }
            //当前用户的角色列表
            var roleList = user.UserRoleRels?.Select(a => masterContext.Roles.FirstOrDefault(r => r.ID == a.RoleID))?.ToList();
            if (roleList == null || roleList.Count() == 0)
            {
                throw new CustomException("NoPermission", "账号无权限");
            }

            result.Data = SetUserInfo(user, roleList, appSystem, userLogin.Lang);
            if (user.IsNew)
            {
                user.IsNew = false;
            }
            //如果时间不足30天，每次登录延长90天
            if (user.ValidTime < utcNow.AddDays(30))
            {
                user.ValidTime = utcNow.AddDays(90);
            }
            await masterContext.SaveChangesAsync();

            result.Msg = "登录成功。";
            result.IsSuccess = true;
            if (user.FirstChange)
            {
                result.Code = "ChangeYourPassword";
                result.Msg += "首次登陆请修改密码！";
            }

            if (await SetTokenAsync(result.Data, appSystem, userLogin.RequestIP))
            {
                return result;
            }
            else
            {
                throw new CustomException("SetTokenError", "保存token出错");
            }
        }

        /// <summary>
        /// 构造返回数据结构
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roleList"></param>
        /// <param name="appSystem"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        private UserInfoAndRoleRight SetUserInfo(Entity.DBModel.User user, List<Role> roleList, AppSystem appSystem, Language lang)
        {
            //处理superAdmin的权限值
            Action<List<RoleMenu>> actSuper = null;
            actSuper = (obj) =>
            {
                if (obj != null && obj.Count() > 0)
                {
                    obj.ForEach(m =>
                    {
                        m.MenuName = m.MenuName.JsonToObj<MultipleLanguage>().GetContent(lang);
                        //设置当前菜单权限
                        m.PermissionValue = PermissionValue.VE;
                        //子对象不为空时，设置子对象权限
                        m.MenuChildrenObjects?.ForEach(o =>
                        {
                            o.PermissionValue = PermissionValue.VE;
                            o.ObjectName = o.ObjectName.JsonToObj<MultipleLanguage>().GetContent(lang);
                        });
                        //子菜单不为空，递归调用当前方法
                        if (m.ChildrenMenus != null && m.ChildrenMenus.Count() > 0)
                        {
                            actSuper(m.ChildrenMenus);
                        }
                    });
                }
            };
            //权限中有超级管理员角色的，返回所有菜单数据
            if (roleList.Any(a => a.IsSuperAdmin))
            {
                var menuList = masterContext.Menus.Where(a => !a.IsDisabled && a.SystemID == appSystem.ID).Include(a => a.Children).Include(a => a.AuthorizationObjects).ToList();
                //var roleMenu = Mapper.Map<List<RoleMenu>>(menuList).ToList();
                var obj = Mapper.Map<List<RoleMenu>>(menuList.Where(a => a.ParentID == null));
                //全部权限设置为ve
                actSuper(obj);
                return new UserInfoAndRoleRight() { RoleMenus = obj, SysCode = appSystem.SystemCode, UserInfo = Mapper.Map<UserInfo>(user) };
            }
            else
            {
                /*
                 * 1，遍历角色列表，取对应权限数据
                 * 2，根据角色权限数据，同一个类型和code的权限值取并集。
                 * */
                //获取当前账号的所有角色权限数据
                var roleRights = masterContext.RoleRights.Where(a => roleList.Select(r => r.ID).Contains(a.RoleID)).Include(a => a.Menu).Include(a => a.AuthorizationObject).ToList();
                //获取菜单类型
                var menuRightList = roleRights.Where(a => a.RightType == RightType.Menu);
                //获取数据功能类型
                var objRightList = roleRights.Where(a => a.RightType == RightType.Data || a.RightType == RightType.Function);
                var objIdList = objRightList.Select(a => a.AuthorizationObjectID).ToList();
                var roleMenus = new List<RoleMenu>();
                #region 菜单及子菜单数据处理
                Action<RoleMenu> actMenu = null;
                actMenu = (m) =>
                {
                    var currentMenuId = menuRightList.FirstOrDefault(a => a.Menu.MenuCode == m.MenuCode).MenuID;
                    m.MenuName = m.MenuName.JsonToObj<MultipleLanguage>().GetContent(lang);

                    #region 处理菜单子数据
                    //获取当前菜单下有权限数据的obj（data、function）
                    var childrenObjList = objRightList.Where(a => a.AuthorizationObject.MenuId == currentMenuId)?.Select(a => a.AuthorizationObject).ToList();
                    if (childrenObjList != null)
                    {
                        m.MenuChildrenObjects = Mapper.Map<List<MenuChildrenObject>>(childrenObjList);
                        //遍历当前菜单中数据功能对象
                        m.MenuChildrenObjects.ForEach(a =>
                        {
                            //取其对应当前角色的权限值列表，
                            objRightList.Where(o => o.AuthorizationObject.ObjectCode == a.ObjectCode).ToList()
                            //遍历权限列表求或运算（即权限并集）
                            .ForEach(o1 => a.PermissionValue = (a.PermissionValue | o1.PermissionValue));
                            a.ObjectName = a.ObjectName.JsonToObj<MultipleLanguage>().GetContent(lang);
                        });
                    }
                    #endregion 处理菜单子数据

                    #region 处理子菜单
                    //获取当前menu有权限的子菜单列表
                    var childrenMenu = menuRightList.Where(a => a.Menu.ParentID == currentMenuId)?.Select(a => a.Menu).ToList();
                    if (childrenMenu != null)
                    {
                        m.ChildrenMenus = Mapper.Map<List<RoleMenu>>(childrenMenu);
                        //递归处理子菜单
                        m.ChildrenMenus.ForEach(a => actMenu(a));
                    }
                    #endregion 处理子菜单
                };
                #endregion 菜单及子菜单数据处理
                //遍历根菜单
                foreach (var menu in menuRightList.Where(a => a.Menu.ParentID == null)?.Select(a => a.Menu).Distinct())
                {
                    var menuInfo = Mapper.Map<RoleMenu>(menu);

                    #region 处理子菜单

                    actMenu(menuInfo);
                    roleMenus.Add(menuInfo);
                    #endregion 处理子菜单
                }
                return new UserInfoAndRoleRight() { SysCode = appSystem.SystemCode, RoleMenus = roleMenus, UserInfo = Mapper.Map<UserInfo>(user) };
            }

        }
        /// <summary>
        /// 生成token，并存于redis，同时新增一条sessionlog记录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="sys"></param>
        /// <param name="ip"></param>
        /// <returns>是否成功</returns>
        private async Task<bool> SetTokenAsync(UserInfoAndRoleRight user, AppSystem sys, string ip)
        {
            var utcNow = DateTime.UtcNow;
            var token = SSOHelper.GenerateToken(user.UserInfo.UserID.ToString(), user.UserInfo.AccountName, user.UserInfo.UserName, ip);

            int timespan = ConfigService.TokenOverTime;//分钟

            #region  缓存token
            var tokenKey = ConfigService.GetTokenRedisKey(user.UserInfo.UserID.ToString(), user.UserInfo.AccountName, sys.SystemCode);
            //检查当前用户在当前system下是否已有token。已有则更新log，并在redis中覆盖原有token
            var existTokenValue = redisCache.Get(tokenKey).ToString();

            var userInfoKey = ConfigService.GetUserInfoRedisKey(token, sys.SystemCode);
            //缓存、db记录。
            redisCache.Set(tokenKey, token, TimeSpan.FromMinutes(timespan));
            redisCache.Set(userInfoKey, user, TimeSpan.FromMinutes(timespan));
            //检查是否缓存成功
            var cacheResult = redisCache.Get(tokenKey).ToString() == token && redisCache.Get(userInfoKey).ToString() == user.ToJson();

            if (!existTokenValue.IsNullOrWhiteSpace())
            {
                var existUserInfoKey = ConfigService.GetUserInfoRedisKey(existTokenValue, sys.SystemCode);
                redisCache.Remove(existUserInfoKey);

                //查询session日志记录
                var sessionLog = await masterContext.UserSessionLogs.FirstOrDefaultAsync(a => a.UserToken == existTokenValue && a.UserID == user.UserInfo.UserID);
                if (sessionLog != null)
                {
                    sessionLog.RealExpirationTime = utcNow;
                }
            }
            #endregion 缓存token

            //DB记录token记录
            await masterContext.UserSessionLogs.AddAsync(new UserSessionLog
            {
                AccessHost = ip,
                CreateTime = utcNow,
                ModifyTime = utcNow,
                SystemID = sys.ID,
                SystemName = sys.SystemName,
                UserID = user.UserInfo.UserID,
                UserToken = token,
                ValidTime = timespan * 60,
                RealExpirationTime = utcNow.AddMinutes(timespan)
            });
            var dbSaveResult = await masterContext.SaveChangesAsync();
            if (cacheResult && dbSaveResult > 0)
            {
                user.Token = token;
                return true;
            }
            return false;
        }

        /// <summary>
        ///注销登录
        /// </summary>
        /// <returns></returns>
        public async Task<Result> LogoutAsync(string sysCode)
        {
            /*
             * 1，根据token查找到相关user信息（过滤器中处理）
             * 2，根据user删除token、以及token为key存储的user
             * 3，更新sessionlog
             * **/

            var tokenKey = ConfigService.GetTokenRedisKey(currentUser.UserID.ToString(), currentUser.AccountName, sysCode);
            var userInfoKey = ConfigService.GetUserInfoRedisKey(currentUser.Token, sysCode);
            redisCache.Remove(new string[] { tokenKey, userInfoKey });

            var sessionLog = await masterContext.UserSessionLogs.FirstOrDefaultAsync(a => a.UserToken == currentUser.Token && a.UserID == currentUser.UserID);
            if (sessionLog != null)
            {
                var utcNow = DateTime.UtcNow;
                sessionLog.RealExpirationTime = utcNow;
                sessionLog.ModifyTime = utcNow;
                await masterContext.SaveChangesAsync();
            }
            return new Result { Msg = "注销成功", IsSuccess = true };
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        public Result<UserInfoAndRoleRight> GetCurrentUserInfo(string sysCode)
        {
            var userInfoKey = ConfigService.GetUserInfoRedisKey(currentUser.Token,sysCode);

            return new SuccessResult<UserInfoAndRoleRight>() { Data = redisCache.Get<UserInfoAndRoleRight>(userInfoKey) };
        }

        public async Task<Result<List<User>>> GetUserInfoAsync(UserIDList userIDList)
        {
            if (userIDList == null || userIDList.UserIDs.Count <= 0)
            {
                throw new CustomException("InvalidArguments", "无效的参数");
            }
            var list =await  GetListAsync(a => userIDList.UserIDs.Contains(a.ID), true);
            list?.ForEach(a=>a.Password=string.Empty);

            return new SuccessResult<List<User>> { Data= list};
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="userList"></param>
        /// <returns></returns>
        public async Task<Result> AddUsersAsync(NewUserList userList)
        {
            var result = new Result();
            if (userList == null)
            {
                throw new CustomException( "InvalidArguments","无效的参数");
            }
            //用户名已存在
            var exist = masterDbSet.Any(a => userList.Users.Select(m => m.AccountName).Contains(a.AccountName));
            if (exist)
            {
                //result.Msg = "已存在的账号";
                //result.Code = "AccountIsExist";
                //return result;
                throw new CustomException("AccountIsExist","已存在的账号");
            }
            var users = Mapper.Map<List<User>>(userList.Users).ToArray();
            var insertResult = await InsertManyAsync(users);

            result.IsSuccess = insertResult;
            result.Msg = insertResult? null:"操作失败";
            return result;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> UpdateUsersAsync(UpdateUsers param)
        {
            if (param == null || param.Users == null || param.Users.Count <= 0)
            {
                throw new CustomException("InvalidArguments", "无效的参数");
            }
            var utcNow = DateTime.UtcNow;
            foreach(var u in param.Users)
            { 
                var user=await masterDbSet.FirstOrDefaultAsync(a=>a.ID==u.ID);
                //账号有修改，但有其他用户使用该账号，则全部不保存，退出
                if (user.AccountName != u.AccountName && masterDbSet.Any(a => a.AccountName == u.AccountName))
                {
                    throw new CustomException("AccountNameIsExist", "账号已被占用");               
                }
                user.AccountName = u.AccountName;
                user.Birthday = u.Birthday;
                user.Email = u.Email;
                user.FirstChange = u.FirstChange;
                user.Gender = u.Gender;
                user.IsDisabled = u.IsDisabled;
                user.IsMemberShipPassword = u.IsMemberShipPassword;
                user.Mobile = u.Mobile;
                user.ModifyTime = utcNow;
                //密码有改动，密码更新时间要更新
                if (user.Password!=u.Password)
                {
                    user.Password = u.Password;
                    user.PasswordModifyTime = utcNow;
                }              
                user.PasswordModifyPeriod = u.PasswordModifyPeriod;
                user.QQ = u.QQ;
                user.Telphone = u.Telphone;
                user.UserName = u.UserName;
            }
            var  updateResult =await masterContext.SaveChangesAsync() > 0;
            return updateResult ? new SuccessResult() : new Result() { IsSuccess = false, Code="UpdateError", Msg="更新失败" };
        }

        /// <summary>
        /// 找回密码。发送邮件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> RetrievePasswordAsync(RetrievePassword param,string emailTemplate)
        {
            /*
             * 1，查找用户id，找到关联问题，验证答案
             * 2，发送验证码、记录验证码以及有效期
             * 3，另起修改密码的接口，验证码失效机制
             * **/
            var user = await masterDbSet.Where(a => a.AccountName == param.AccountName).Include(a => a.UserSecurityQuestions)
                 .FirstOrDefaultAsync(a => a.UserSecurityQuestions.Any(q => q.QuestionID == param.QuestionID && q.Answer == param.Answer));
            if (user == null)
            {
                throw new CustomException("AnsowerError", "找回密码失败");
            }
            if (user.IsDisabled)
            {
                throw new CustomException("CurrentUserIsDisabled","账户被禁用");
            }
            //todo:参照原系统中GetEmailVerificationCode_V_1_2 方法。发送邮件
            if (user.Email.IsNullOrWhiteSpace())
            {
                throw new CustomException("EmailIsEmpty","邮箱为空");
            }
            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!regex.IsMatch(user.Email))
            {
                throw new CustomException("EmailError","邮箱信息错误");
            }
            var vCode =new VerificationCode(user.AccountName, VerificationCodeType.ChangePasswordByCode);
            var redisKey = ConfigService.GetVerificationCodeRedisKey(user.AccountName, VerificationCodeType.ChangePasswordByCode, param.SysCode);
            var validTime =Convert.ToInt32( BaseCore.AppSetting["VerificationCodeValidTime"]);
            redisCache.Set(redisKey,vCode, TimeSpan.FromMinutes(validTime));

            var subject = "";
            var sendContent = emailTemplate.Replace("{UserName}", user.UserName)
                                                                        .Replace("{EmailCode}", vCode.Code)
                                                                        .Replace("{ValidDateTime}", validTime.ToString());
            if (param.Url.IsNullOrWhiteSpace())
            {
                sendContent = sendContent.Replace("{Action}", "");
            }
            else
            {
                if (param.Lang == Language.zh_CN)
                {
                    subject = BaseCore.AppSetting["RetrievePasswordEmail:EmailSubject_zh"];
                    sendContent = sendContent.Replace("{Action}", BaseCore.AppSetting["RetrievePasswordEmail:ChangePassWordByCodeEmail_zh"]);
                }
                else
                {
                    subject = BaseCore.AppSetting["RetrievePasswordEmail:EmailSubject_en"];
                    sendContent = sendContent.Replace("{Action}", BaseCore.AppSetting["RetrievePasswordEmail:ChangePassWordByCodeEmail_en"]);
                }
                sendContent = sendContent.Replace("{Url}", "<a href='" + param.Url + "' target='_blank'>" + param.Url + "</a>");
            }
            TEGEMailHelper.SendTE2UNoReplyEmail(user.Email, subject,sendContent);
            //todo:log记录
            return new SuccessResult ();
        }

        /// <summary>
        /// 禁用/启用指定用户
        /// </summary>
        /// <param name="userIdList">用户id</param>
        /// <param name="disable">是否要禁用</param>
        /// <returns></returns>
        public async Task<Result> DisableOrEnableUserAsync(UserIDList userIdList,bool disable)
        {
            var users = masterDbSet.Where(a => userIdList.UserIDs.Contains(a.ID)).ToList();
            if (users == null || users.Count <= 0)
            {
                throw new CustomException("NoData", "未查到任何数据");
            }
            users.ForEach(a => a.IsDisabled = disable);
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

       
    }
}
