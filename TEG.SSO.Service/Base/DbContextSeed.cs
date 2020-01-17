using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using TEG.SSO.Common;
using TEG.SSO.EFCoreContext;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.LogDBContext;

namespace TEG.SSO.Service
{
    public class DbContextInit
    {
        /// <summary>
        /// 数据库更新、数据初始化
        /// </summary>
        /// <param name="svp"></param>
        public static void DbContextInitAll(IServiceProvider svp)
        {
            DbInit(svp);
            if (Convert.ToBoolean(BaseCore.AppSetting["InitData"]))
            {
                ReflectionInfo(svp);
                Seed(svp);
            }
        }
        /// <summary>
        /// 数据库初始化：建表建库
        /// </summary>
        /// <param name="svp"></param>
        private static void DbInit(IServiceProvider svp)
        {
            var masterContext = (BizMasterContext)svp.GetService(typeof(BizMasterContext));
            var readOnlyContext = (BizReadOnlyContext)svp.GetService(typeof(BizReadOnlyContext));
            masterContext.Database.Migrate();
            var masterDbConn = masterContext.Database.GetDbConnection();
            var readOnlyDbConn = readOnlyContext.Database.GetDbConnection();
            //主库从库ip+database相同，则不再对从库初始化
            if ($"{masterDbConn.DataSource}:{masterDbConn.Database}" != $"{readOnlyDbConn.DataSource}:{readOnlyDbConn.Database}")
            {
                readOnlyContext.Database.Migrate();
            }
            var logContext = (LogContext)svp.GetService(typeof(LogContext));
            logContext.Database.Migrate();
        }
        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="svp"></param>
        private static void Seed(IServiceProvider svp)
        {
            var utcNow = DateTime.UtcNow;
            var dbContext = (BizMasterContext)svp.GetService(typeof(BizMasterContext));

            if (!dbContext.Users.Any())
            {
                dbContext.Users.Add(new User
                {
                    AccountName = "SuperAdmin",
                    UserName = "SuperAdmin",
                    Password = "Wcj1/HpAOQcl1wsRhXk0Cw==",
                    FirstChange = false,
                    IsDisabled = false,
                    IsNew = false,
                    PasswordModifyPeriod = 1,
                    CreateTime = utcNow,
                    ValidTime = utcNow.AddYears(5),
                    ModifyTime = utcNow,
                    PasswordModifyTime = utcNow,
                    LastUpdateAccountName = "System"
                });
                dbContext.SaveChanges();
            }

            if (!dbContext.Organizations.Any())
            {
                dbContext.Organizations.AddRange(new List<Organization> {
                   new Organization{  OrgName="科技部",CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System" },
                   new Organization{  OrgName="出境部",CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System" },
                   new Organization{  OrgName="入境部",CreateTime=utcNow, ModifyTime=utcNow ,LastUpdateAccountName="System"},
                   new Organization{  OrgName="票务部",CreateTime=utcNow, ModifyTime=utcNow ,LastUpdateAccountName="System"},
                   new Organization{  OrgName="行政部",CreateTime=utcNow, ModifyTime=utcNow ,LastUpdateAccountName="System"},
                   new Organization{  OrgName="财务部",CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System" },
                   new Organization{  OrgName="客服部",CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System" },
                   new Organization{  OrgName=".NET小组", ParentID=1, CreateTime=utcNow, ModifyTime=utcNow ,LastUpdateAccountName="System"},
                   new Organization{  OrgName="WPF小组", ParentID=1, CreateTime=utcNow, ModifyTime=utcNow ,LastUpdateAccountName="System"},
                   new Organization{  OrgName="客户",CreateTime=utcNow, ModifyTime=utcNow ,LastUpdateAccountName="System"},
                   new Organization{  OrgName="测试1", ParentID=10, CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System" }
               });
                dbContext.SaveChanges();
            }
            if (!dbContext.UserDeptRels.Any())
            {
                dbContext.UserDeptRels.Add(new UserDeptRel { UserID = 1, DeptID = 1, CreateTime = utcNow, ModifyTime = utcNow, LastUpdateAccountName = "System" });
                dbContext.SaveChanges();
            }
            if (!dbContext.Roles.Any())
            {
                //超级管理员不验证权限。所有菜单数据均可访问
                dbContext.Roles.Add(new Role { RoleName = "超级管理员", IsSuperAdmin = true, CreateTime = utcNow, ModifyTime = utcNow, LastUpdateAccountName = "System" });
                dbContext.SaveChanges();
            }

            if (!dbContext.UserRoleRels.Any())
            {
                dbContext.UserRoleRels.Add(new UserRoleRel { UserID = 1, RoleID = 1, CreateTime = utcNow, ModifyTime = utcNow, LastUpdateAccountName = "System" });
                dbContext.SaveChanges();
            }

            if (!dbContext.AppSystems.Any())
            {
                dbContext.AppSystems.Add(new AppSystem { SystemName = "eItS China", SystemCode = "eItS_CH", LastUpdateAccountName = "System", SystemType = SystemType.WinForm, CreateTime = utcNow, ModifyTime = utcNow });
                dbContext.SaveChanges();
            }
            //if (!dbContext.Menus.Any())
            //{
            //    dbContext.Menus.Add(new Menu { MenuCode = "MainMenu", MenuName = "{\"local_Lang\":\"顶级测试菜单\",\"en_US\":\"\"}", SystemID = 1, SortID = 1, LastUpdateAccountName = "System" });
            //    dbContext.SaveChanges();
            //}
            //if (!dbContext.AuthorizationObjects.Any())
            //{
            //    dbContext.AuthorizationObjects.Add(new AuthorizationObject { MenuId = 1, ObjectCode = "MainMenu-Edit", ObjectName = "{\"local_Lang\":\"顶级菜单编辑按钮\",\"en_US\":\"\"}", ObjectType = ObjectType.Function, LastUpdateAccountName = "System" });
            //    dbContext.SaveChanges();
            //}
            if (!dbContext.SecurityQuestions.Any())
            {
                dbContext.SecurityQuestions.AddRange(new List<SecurityQuestion> {
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您母亲的姓名是?\",\"en_US\":\"What's your mother's name?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您父亲的姓名是?\",\"en_US\":\"What's your father's name?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您配偶的姓名是?\",\"en_US\":\"What's your spouse's name?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您的出生地是?\",\"en_US\":\"Where was your birthplace?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您高中班主任的名字是?\",\"en_US\":\"What's the name of your junior high school head teacher?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您初中班主任的名字是?\",\"en_US\":\"What's the name of your senior class teacher?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您小学班主任的名字是?\",\"en_US\":\"What's your primary school teacher's name?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您的学号（或工号）是?\",\"en_US\":\"What's your student number (or work number)?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您父亲的生日是?\",\"en_US\":\"What's your father's birthday?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您母亲的生日是?\",\"en_US\":\"What's your mother's birthday?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"},
                    new SecurityQuestion{  QuestionContent="{\"local_Lang\":\"您配偶的生日是?\",\"en_US\":\"What's your spouse's birthday?\"}", CreateTime=utcNow, ModifyTime=utcNow,LastUpdateAccountName="System"}
                });
                dbContext.SaveChanges();
            }

            if (!dbContext.UserSecurityQuestions.Any())
            {
                dbContext.UserSecurityQuestions.Add(new UserSecurityQuestion { UserID = 1, QuestionID = 1, Answer = "Mother", CreateTime = utcNow, ModifyTime = utcNow, LastUpdateAccountName = "System" });
                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// 反射出所有actioin信息，导入AuthorizeObject表中。【！！！在业务系统中的同职责actionCode必须与本api系统中的相应actionCode相同！！！】
        /// </summary>
        public static void ReflectionInfo(IServiceProvider svp)
        {
            var utcNow = DateTime.UtcNow;
            var dbContext = (BizMasterContext)svp.GetService(typeof(BizMasterContext));
            var currentDirPath = Directory.GetCurrentDirectory();
            var filePaths = BaseCore.AppSetting["ReflectionFiles"].Split(',');
            foreach (var path in filePaths)
            {
                var types = Assembly.LoadFile(currentDirPath + "\\" + path).GetTypes();
                var controllers = types.Where(a => a.BaseType == typeof(ControllerBase) || a.BaseType.BaseType == typeof(ControllerBase));
                foreach (var t in controllers)
                {
                    Console.WriteLine("当前读取类：" + t.Namespace + "." + t.Name);
                    var customAttrs = t.CustomAttributes.Where(a => a.AttributeType.GetTypeInfo().Name == "CustomAuthorizeAttribute").Select(a => a.NamedArguments);
                    // 判断控制器上是否有自定义权限过滤器
                    if (customAttrs != null && customAttrs.Count() > 0)
                    {
                        Console.WriteLine("控制器上特性： " + customAttrs.FirstOrDefault(a => a.FirstOrDefault(m => m.MemberName == "Name") != null).Select(a => a.TypedValue.Value).ToJson());
                    }
                    foreach (var m in t.GetTypeInfo().DeclaredMethods)
                    {
                        Console.WriteLine("方法名:" + m.Name);
                        var attr = m.CustomAttributes.FirstOrDefault(a => a.AttributeType.GetTypeInfo().Name == "CustomAuthorizeAttribute");
                        //无特性或者特性未给命名参数赋值，则进入下次循环
                        if (attr == null || attr.NamedArguments == null || attr.NamedArguments.Count <= 0)
                        {
                            continue;
                        }
                        //不校验token或者不校验权限code的，无需记录入DB
                        var p = attr.NamedArguments.Where(a => a.MemberName == "CheckPermission" || a.MemberName == "Verify");
                        if (p.Any(a => !(bool)a.TypedValue.Value))
                        {
                            continue;
                        }
                        var actionCode = (string)attr.NamedArguments.FirstOrDefault(a => a.MemberName == "ActionCode").TypedValue.Value;
                        if (actionCode.IsNullOrWhiteSpace())
                        {
                            continue;
                        }
                        //已存在，则不再插入
                        if (dbContext.AuthorizationObjects.Any(a => a.ObjectCode == actionCode))
                        {
                            continue;
                        }
                        dbContext.AuthorizationObjects.Add(new AuthorizationObject
                        {
                            ObjectCode = actionCode,
                            ObjectName = new MultipleLanguage { local_Lang = (string)attr.NamedArguments.FirstOrDefault(a => a.MemberName == "Description").TypedValue.Value }.ToJson(),
                            ObjectType = ObjectType.Function,
                            CreateTime = utcNow,
                            ModifyTime = utcNow,
                            LastUpdateAccountName = "System"
                        });
                        Console.WriteLine("插入ActionCode=" + actionCode);
                    }
                }
            }
            dbContext.SaveChanges();
        }
    }
}
