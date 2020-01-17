using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;
using TEG.Framework.Utility;
using System.Linq.Expressions;
using TEG.Framework.Security.SSO;

namespace TEG.SSO.Service
{
    public class UserService : ServiceBase<User>
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
        public async Task<Result<string>> LoginAsync(UserLogin userLogin)
        {
            var result = new Result<string>();
            if (userLogin.Data.LoginName.IsNullOrWhiteSpace() || userLogin.Data.Password.IsNullOrWhiteSpace())
            {
                throw new CustomException("IDOrPasswordIsEmpty", "登录账号或密码为空");
            }
            var user = await masterContext.Users.Where(a => a.AccountName == userLogin.Data.LoginName && a.Password == userLogin.Data.Password)
                                                                                .Include(a => a.UserDeptRels)
                                                                                .Include(a => a.UserRoleRels)
                                                                                .ThenInclude(a => a.Role).FirstOrDefaultAsync();
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
                throw new CustomException("SystemError", "系统信息有误或已禁用");
            }
            //当前用户的角色列表
            //缓存db数据10分钟。后面验证权限时使用
            var roleList = TryToGetFromRedis(ConfigService.GetDBUserRoleRedisKey(user.AccountName, user.ID),
                () =>
                {
                    return user.UserRoleRels.AsQueryable().Select(a => a.Role).ToList();
                });
            ////无角色，不影响登录。只是看不到任何页面
            //if (roleList == null || roleList.Count() == 0)
            //{
            //    throw new CustomException("NoPermission", "账号无权限");
            //}

            //缓存user信息，包括菜单权限信息
            var userInfo= SetUserInfo(user, roleList, appSystem, userLogin.Lang);
            if (user.IsNew)
            {
                user.IsNew = false;
            }
            //如果时间不足30天，每次登录延长90天
            if (user.ValidTime < utcNow.AddDays(Convert.ToInt32(BaseCore.AppSetting["AccountValidTime:MinValidTime"])))
            {
                user.ValidTime = utcNow.AddDays(Convert.ToInt32(BaseCore.AppSetting["AccountValidTime:ValidTime"]));
            }
            await masterContext.SaveChangesAsync();

            result.Msg = "登录成功。";
            result.IsSuccess = true;

            if (user.FirstChange)
            {
                result.Code = "ChangeYourPassword";
                result.Msg += "首次登陆请修改密码！";
            }

            if (await SetTokenAsync(userInfo, appSystem, userLogin.RequestIP))
            {
                result.Data = userInfo.Token;
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
        private UserInfoAndRoleRight SetUserInfo(User user, List<Role> roleList, AppSystem appSystem, Language lang)
        {
 
            var depts = user.UserDeptRels.Select(a => a.Dept).ToList().MapTo<List<DeptAndParentInfo>>();
            //权限中有超级管理员角色的，返回所有菜单数据
            var isSuperAdmin = roleList.Any(a => a.IsSuperAdmin);
            if (isSuperAdmin)
            {
                var menuList = masterContext.Menus.Where(a => !a.IsDisabled && a.SystemID == appSystem.ID).Include(a => a.AuthorizationObjects).ToList();
                var obj = Mapper.Map<List<AuthRight>>(menuList);
                //全部权限设置为ve               
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
                    });
                }
                return new UserInfoAndRoleRight() { RoleMenus = obj, IsSuperAdmin = isSuperAdmin, SysCode = appSystem.SystemCode, UserInfo = Mapper.Map<UserInfo>(user) , Dept= depts };
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
                var menuRightList = roleRights.Where(a => a.IsMenu);
                //获取数据功能类型
                var objRightList = roleRights.Where(a => !a.IsMenu);
                var objIdList = objRightList.Select(a => a.AuthorizationObjectID).ToList();
                var roleMenus = new List<AuthRight>();
                #region 菜单数据处理             
                Action<Entity.DTO.AuthRight> actMenu = (m) =>
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
                };
                #endregion 菜单数据处理
                //遍历根菜单
                foreach (var menu in menuRightList.Select(a=>a.Menu).Distinct())
                {
                    var menuInfo = Mapper.Map<AuthRight>(menu);
                    actMenu(menuInfo);
                    roleMenus.Add(menuInfo);
                }
                return new UserInfoAndRoleRight() { SysCode = appSystem.SystemCode, IsSuperAdmin = isSuperAdmin, RoleMenus = roleMenus, UserInfo = Mapper.Map<UserInfo>(user) };
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
            #region 获取用户部门信息
            var orgService = (OrganizationService)_serviceProvider.GetService(typeof(OrganizationService));
            orgService.currentUser = new TokenUserInfo
            {
                AccountName = user.UserInfo.AccountName,
                UserID = user.UserInfo.UserID,
                UserName = user.UserInfo.UserName,
                IP = ip,
                Token = token
            };
            user.Token = token;
            user.Dept = await orgService.GetCurrentUserDeptsAndParentsAsync();
            #endregion

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

            //用户有旧token，要清除该token对应的userInfo信息
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
            return new SuccessResult { Msg = "注销成功" };
        }

        /// <summary>
        /// 通过旧密码来修改密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> ChangPasswordAsync(ChangePassword param)
        {
            /*
             * 1，两次密码是否一致
             * 2，验证原账号密码、未禁用
             * 3，修改除密码之外的字段
             * **/
            if (param.Data.NewPassword != param.Data.ConfirmPassword)
            {
                throw new CustomException("PasswordsDiffer", "两次输入密码不一致");
            }
            var user = await masterDbSet.FirstOrDefaultAsync(a => a.Password == param.Data.OldPassword && a.ID == currentUser.UserID);
            if (user == null)
            {
                throw new CustomException("PasswordError", "密码错误");
            }
            var utcNow = DateTime.UtcNow;
            user.Password = param.Data.NewPassword;
            user.ModifyTime = utcNow;
            user.PasswordModifyTime = utcNow;
            user.LastUpdateAccountName = currentUser.AccountName;
            await masterContext.SaveChangesAsync();
            return new SuccessResult { Msg = "修改成功" };
        }

        /// <summary>
        /// 修改指定用户id的密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> ChangePasswordByUserIDAsync(AdminChangePassword param)
        {
            /*
             * 1，指定id用户是否存在
             * 2，修改密码
             * **/
            var user = await masterDbSet.Where(a => a.ID == param.Data.UserID).Include(a => a.UserRoleRels).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new CustomException("UserIDIsNotExist", "用户ID不存在");
            }

            user.Password = param.Data.Password;
            user.LastUpdateAccountName = currentUser.AccountName;
            await masterContext.SaveChangesAsync();
            return new SuccessResult { Msg = "修改成功" };
        }

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <returns></returns>
        public Result<UserInfoAndRoleRight> GetCurrentUserInfo(string sysCode)
        {
            var userInfoKey = ConfigService.GetUserInfoRedisKey(currentUser.Token, sysCode);

            return new SuccessResult<UserInfoAndRoleRight>() { Data = redisCache.Get<UserInfoAndRoleRight>(userInfoKey) };
        }

        public async Task<Result<string>> GetEmailByAccountNameAsync(RequestBase<string> param)
        {
            if (param.Data.IsNullOrWhiteSpace())
            {
                throw new CustomException("InvalidArguments", "无效的参数");
            }
            var user = await readOnlyDbSet.FirstOrDefaultAsync(a=>a.AccountName==param.Data);
            if (user == null)
            {
                throw new CustomException("UserIsNotExist", "用户不存在");
            }
            if (user.Email.IsNullOrWhiteSpace())
            {
                throw new CustomException("NoEmail", "用户未设置邮箱");
            }
            return new SuccessResult<string>(user.Email);   
        }

        /// <summary>
        ///根据userIdlist  获取用户信息列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result<List<UserAndRoleAndDeptInfo>>> GetUserInfoByIDsAsync(RequestID param)
        {
            if (param == null || param.Data.IDs.Count <= 0)
            {
                throw new CustomException("InvalidArguments", "无效的参数");
            }

            var list = await masterDbSet.Where(a => param.Data.IDs.Contains(a.ID)).Include(a => a.UserRoleRels).ThenInclude(a => a.Role)
                .Include(a => a.UserDeptRels).ThenInclude(a => a.Dept).ToListAsync();

            var resultData = new List<UserAndRoleAndDeptInfo>();
            list.ForEach(a =>
            {
                var data = a.MapTo<UserAndRoleAndDeptInfo>();
                data.Roles = a.UserRoleRels?.Select(m => new RoleInfo { RoleID = m.Role.ID, RoleName = m.Role.RoleName, IsSuperAdmin = m.Role.IsSuperAdmin })?.ToList();
                data.Depts = a.UserDeptRels?.Select(m => new DeptInfo { DeptID = m.Dept.ID, DeptName = m.Dept.OrgName })?.ToList();
                resultData.Add(data);
            });
            return new SuccessResult<List<UserAndRoleAndDeptInfo>> { Data = resultData };
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
                throw new CustomException("InvalidArguments", "无效的参数");
            }
            //用户名已存在
            var exist = masterDbSet.Any(a => userList.Data.Select(m => m.AccountName).Contains(a.AccountName));
            if (exist)
            {
                //result.Msg = "已存在的账号";
                //result.Code = "AccountIsExist";
                //return result;
                throw new CustomException("AccountIsExist", "已存在的账号");
            }
            var users = Mapper.Map<List<User>>(userList.Data);
            var accountValidTime = Convert.ToInt32(BaseCore.AppSetting["AccountValidTime:ValidTime"]);//配置中读取新建账号有效期，单位：天
            var validTime = DateTime.UtcNow.AddDays(accountValidTime);
            users.ForEach(a =>
            {
                a.LastUpdateAccountName = currentUser.AccountName;
                a.ValidTime = validTime;
            });
            var insertResult = await InsertManyAsync(users.ToArray());

            result.IsSuccess = insertResult;
            result.Msg = insertResult ? null : "操作失败";
            return result;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> UpdateUsersAsync(UpdateUser param)
        {
            if (param == null || param.Data == null || param.Data.Count <= 0)
            {
                throw new CustomException("InvalidArguments", "无效的参数");
            }
            var roleIds = new List<int>();
            var deptIds = new List<int>();
            param.Data.ForEach(a =>
            {
                roleIds.AddRange(a.Roles);
                deptIds.AddRange(a.Depts);
            });
            if (roleIds.Any(m => !masterContext.Roles.Any(a => a.ID == m)))
            {
                throw new CustomException("RoleIDError", "含有错误的角色id");
            }
            if (deptIds.Any(m => !masterContext.Organizations.Any(a => a.ID == m)))
            {
                throw new CustomException("DeptIDError", "含有错误的部门id");
            }

            var utcNow = DateTime.UtcNow;
            foreach (var u in param.Data)
            {
                #region 更新user信息
                var user = await masterDbSet.FirstOrDefaultAsync(a => a.ID == u.ID);
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
                user.PasswordModifyPeriod = u.PasswordModifyPeriod;
                user.QQ = u.QQ;
                user.Telphone = u.Telphone;
                user.UserName = u.UserName;
                user.LastUpdateAccountName = currentUser.AccountName;
                //请求参数中如果密码为空表示不更新密码
                //密码有改动，密码更新时间要更新
                if (u.Password.IsNotNullOrWhiteSpace() && user.Password != u.Password)
                {
                    user.Password = u.Password;
                    user.PasswordModifyTime = utcNow;
                }
                #endregion 更新user信息

                #region 更新角色信息
                if (u.Roles != null)
                {
                    var deleteURRel = masterContext.UserRoleRels.Where(a => a.UserID == u.ID);
                    masterContext.UserRoleRels.RemoveRange(deleteURRel);
                    var urRel = new List<UserRoleRel>();
                    u.Roles.ForEach(a => urRel.Add(new UserRoleRel
                    {
                        UserID = u.ID,
                        RoleID = a,
                        CreateTime = utcNow,
                        ModifyTime = utcNow,
                        LastUpdateAccountName = currentUser.AccountName
                    }));
                    await masterContext.UserRoleRels.AddRangeAsync(urRel);
                }
                #endregion 更新角色信息

                #region 更新部门信息
                if (u.Depts != null)
                {
                    var deleteUDRel = masterContext.UserDeptRels.Where(a => a.UserID == u.ID);
                    masterContext.UserDeptRels.RemoveRange(deleteUDRel);
                    var udRel = new List<UserDeptRel>();
                    u.Depts.ForEach(a => udRel.Add(new UserDeptRel
                    {
                        UserID = u.ID,
                        DeptID = a,
                        CreateTime = utcNow,
                        ModifyTime = utcNow,
                        LastUpdateAccountName = currentUser.AccountName
                    }));
                    await masterContext.UserDeptRels.AddRangeAsync(udRel);
                }
                #endregion 更新部门信息
            }
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 找回密码。发送邮件
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> RetrievePasswordAsync(RetrievePassword param, string emailTemplate)
        {
            /*
             * 1，查找用户id，找到关联问题，验证答案
             * 2，发送验证码、记录验证码以及有效期
             * **/
            var sysIsExist = await masterContext.AppSystems.AnyAsync(a => !a.IsDisabled && a.SystemCode == param.SysCode);
            if (!sysIsExist)
            {
                throw new CustomException("SysCodeError", "错误的系统码");
            }
            var user = await masterDbSet.Where(a => a.AccountName == param.Data.AccountName).Include(a => a.UserSecurityQuestions).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new CustomException("UserIsNotExist", "账号不存在");
            }
            if (user.IsDisabled)
            {
                throw new CustomException("CurrentUserIsDisabled", "账户被禁用");
            }
            if (!user.UserSecurityQuestions.Any(q => q.QuestionID == param.Data.QuestionID && q.Answer == param.Data.Answer))
            {
                throw new CustomException("AnsowerError", "答案错误");
            }
            if (user.Email.IsNullOrWhiteSpace())
            {
                throw new CustomException("EmailIsEmpty", "邮箱为空");
            }
            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            if (!regex.IsMatch(user.Email))
            {
                throw new CustomException("EmailError", "邮箱信息错误");
            }
            var vCode = new VerificationCode(user.AccountName, VerificationCodeType.ChangePasswordByCode);
            var redisKey = ConfigService.GetVerificationCodeRedisKey(user.AccountName, VerificationCodeType.ChangePasswordByCode, param.SysCode);
            var validTime = Convert.ToInt32(BaseCore.AppSetting["VerificationCodeValidTime"]);
            redisCache.Set(redisKey, vCode, TimeSpan.FromMinutes(validTime));

            var subject = "";
            var sendContent = emailTemplate.Replace("{UserName}", user.UserName)
                                                                        .Replace("{EmailCode}", vCode.Code)
                                                                        .Replace("{ValidDateTime}", validTime.ToString());
            if (param.Data.Url.IsNullOrWhiteSpace())
            {
                sendContent = sendContent.Replace("{Action}", "");
            }
            else
            {
                if (param.Lang == Language.local_Lang)
                {
                    subject = BaseCore.AppSetting["RetrievePasswordEmail:EmailSubject_zh"];
                    sendContent = sendContent.Replace("{Action}", BaseCore.AppSetting["RetrievePasswordEmail:ChangePassWordByCodeEmail_zh"]);
                }
                else
                {
                    subject = BaseCore.AppSetting["RetrievePasswordEmail:EmailSubject_en"];
                    sendContent = sendContent.Replace("{Action}", BaseCore.AppSetting["RetrievePasswordEmail:ChangePassWordByCodeEmail_en"]);
                }
                sendContent = sendContent.Replace("{Url}", "<a href='" + param.Data.Url + "' target='_blank'>" + param.Data.Url + "</a>");
            }
            //发送邮件
            TEGEMailHelper.SendTE2UNoReplyEmail(user.Email, subject, sendContent);
            return new SuccessResult();
        }

        /// <summary>
        /// 通过邮件重置密码
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> ResetPasswordAsync(ResetPassword param)
        {
            /*0，两次密码是否一致
             * 1，accountName检测用户是否存在/系统码是否合法
             * 2，验证码是否正确、remove验证码
             * 3，修改密码
             * 4，返回success
             * **/
            if (param.Data.NewPassword != param.Data.ConfirmPassword)
            {
                throw new CustomException("PasswordsDiffer", "两次输入密码不一致");
            }
            var user = await masterDbSet.FirstOrDefaultAsync(a => !a.IsDisabled && a.AccountName == param.Data.AccountName);
            if (user == null)
            {
                throw new CustomException("AccountError", "账号不存在");
            }
            var sysCodeIsExist = await masterContext.AppSystems.AnyAsync(a => !a.IsDisabled && a.SystemCode == param.SysCode);
            if (!sysCodeIsExist)
            {
                throw new CustomException("SysCodeError", "错误的系统码");
            }

            var codeKey = ConfigService.GetVerificationCodeRedisKey(param.Data.AccountName, VerificationCodeType.ChangePasswordByCode, param.SysCode);
            var vCode = redisCache.Get<VerificationCode>(codeKey);
            if (vCode == null || vCode.AccountName != param.Data.AccountName || vCode.Type != VerificationCodeType.ChangePasswordByCode || vCode.Code != param.Data.VerificationCode)
            {
                throw new CustomException("VerificationCodeError", "错误的验证码");
            }
            redisCache.Remove(codeKey);
            if (redisCache.Exists(codeKey))
            {
                //未知错误记录异常日志
                throw new Exception($"redis's key remove() error . Key:[{codeKey}]");
            }
            var utcNow = DateTime.UtcNow;
            user.Password = param.Data.NewPassword;
            user.IsNew = false;
            user.ModifyTime = utcNow;
            user.PasswordModifyTime = utcNow;
            user.LastUpdateAccountName = param.Data.AccountName;
            await masterContext.SaveChangesAsync();
            return new SuccessResult() { Code="ResetPasswordSuccess", Msg="密码重置成功，请用新密码登录"};
        }
        /// <summary>
        /// 禁用/启用指定用户
        /// </summary>
        /// <param name="userIdList">用户id</param>
        /// <param name="disable">是否要禁用</param>
        /// <returns></returns>
        public async Task<Result> DisableOrEnableUserAsync(RequestID userIdList, bool disable)
        {
            var users = masterDbSet.Where(a => userIdList.Data.IDs.Contains(a.ID)).ToList();
            if (users == null || users.Count <= 0)
            {
                throw new CustomException("NoData", "未查到任何数据");
            }

            var utcNow = DateTime.UtcNow;
            users.ForEach(a =>
            {
                a.IsDisabled = disable;
                a.LastUpdateAccountName = currentUser.AccountName;
                a.ModifyTime = utcNow;
            });
            await masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 分页查询user信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<User>> GetPage(GetUserPage param)
        {
            var dataIQueryable = GetIQueryable(a => true);
            if (param.Data.UserID.HasValue)
            {
                dataIQueryable = dataIQueryable.Where(a => a.ID == param.Data.UserID);
            }
            if (param.Data.AccountName.IsNotNullOrWhiteSpace())
            {
                dataIQueryable = dataIQueryable.Where(a => a.AccountName.Contains(param.Data.AccountName));
            }
            if (param.Data.UserName.IsNotNullOrWhiteSpace())
            {
                dataIQueryable = dataIQueryable.Where(a => a.UserName.Contains(param.Data.UserName));
            }
            if (param.Data.Gender.HasValue)
            {
                dataIQueryable = dataIQueryable.Where(a => a.Gender == param.Data.Gender);
            }
            if (param.Data.Email.IsNotNullOrWhiteSpace())
            {
                dataIQueryable = dataIQueryable.Where(a => a.Email.Contains(param.Data.Email));
            }
            if (param.Data.Mobile.IsNotNullOrWhiteSpace())
            {
                dataIQueryable = dataIQueryable.Where(a => a.Mobile.Contains(param.Data.Mobile));
            }
            if (param.Data.IsNew.HasValue)
            {
                dataIQueryable = dataIQueryable.Where(a => a.IsNew == param.Data.IsNew);
            }
            if (param.Data.IsMemberShipPassword.HasValue)
            {
                dataIQueryable = dataIQueryable.Where(a => a.IsMemberShipPassword == param.Data.IsMemberShipPassword);
            }
            if (param.Data.IsDisabled.HasValue)
            {
                dataIQueryable = dataIQueryable.Where(a => a.IsDisabled == param.Data.IsDisabled);
            }

            var data = dataIQueryable.ToPage(param.Data.PageIndex, param.Data.PageSize);
            return new SuccessResult<Page<User>> { Data = data };
        }
        /// <summary>
        /// 删除指定的账号信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> DeleteUserAsync(RequestID param)
        {
            if (param.Data.IDs.Any(a => !masterDbSet.Any(m => m.ID == a)))
            {
                throw new CustomException("UserIDError", "错误的用户Id");
            }
            await DeleteManyAsync(a => param.Data.IDs.Contains(a.ID));
            return new SuccessResult();
        }

        /// <summary>
        /// 分页获取数据//todo:MapToDTO()...
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <param name="sortOption"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fromMasterDb"></param>
        /// <returns></returns>
        public override Page<User> GetPage<TKey>(Expression<Func<User, bool>> filter = null, Expression<Func<User, TKey>> sort = null, SortOption sortOption = SortOption.ASC, int pageIndex = 0, int pageSize = 10, bool fromMasterDb = false)
        {
            var dataIQueryable = GetIQueryable(filter, sort, sortOption, fromMasterDb)
                                                                .Include(a => a.UserDeptRels).ThenInclude(a => a.Dept)
                                                                .Include(a => a.UserRoleRels).ThenInclude(a => a.Role)
                                                                .Include(a => a.UserSecurityQuestions).ThenInclude(a => a.SecurityQuestion);
            return dataIQueryable.ToPage(pageIndex, pageSize);
        }
    }
}
