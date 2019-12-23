using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TEG.SSO.EFCoreContext;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.Enum;

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
            Seed(svp);
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
                    PasswordModifyTime = utcNow
                });
                dbContext.SaveChanges();
            }

            if (!dbContext.Organizations.Any())
            {
                dbContext.Organizations.AddRange(new List<Organization> {
                   new Organization{  OrgName="科技部",CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="出境部",CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="入境部",CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="票务部",CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="行政部",CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="财务部",CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="客服部",CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName=".NET小组", ParentID=1, CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="WPF小组", ParentID=1, CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="客户",CreateTime=utcNow, ModifyTime=utcNow },
                   new Organization{  OrgName="测试1", ParentID=10, CreateTime=utcNow, ModifyTime=utcNow }
               });
                dbContext.SaveChanges();
            }
            if (!dbContext.UserDeptRels.Any())
            {
                dbContext.UserDeptRels.Add(new UserDeptRel { UserID = 1, DeptID = 1,  CreateTime = utcNow, ModifyTime = utcNow });
                dbContext.SaveChanges();
            }
            if (!dbContext.Roles.Any())
            {
                //超级管理员不验证权限。所有菜单数据均可访问
                dbContext.Roles.Add(new Role { RoleName = "超级管理员",  IsSuperAdmin=true, CreateTime = utcNow, ModifyTime = utcNow });
                dbContext.SaveChanges();
            }

            if (!dbContext.UserRoleRels.Any())
            {
                dbContext.UserRoleRels.Add(new UserRoleRel { UserID = 1, RoleID = 1, CreateTime = utcNow, ModifyTime = utcNow });
                dbContext.SaveChanges();
            }

            if (!dbContext.AppSystems.Any())
            {
                dbContext.AppSystems.Add(new AppSystem { SystemName = "eItS China", SystemCode = "eItS_CH", SystemType = SystemType.WinForm, CreateTime = utcNow, ModifyTime = utcNow });
                dbContext.SaveChanges();
            }
            if (!dbContext.Menus.Any())
            {
                dbContext.Menus.Add(new Menu { MenuCode="MainMenu", MenuName= "{\"local_Lang\":\"顶级测试菜单\",\"en_US\":\"\"}", SystemID=1, SortID=1 });
                dbContext.SaveChanges();
            }
            if (!dbContext.AuthorizationObjects.Any())
            {
                dbContext.AuthorizationObjects.Add(new AuthorizationObject { MenuId=1, ObjectCode="MainMenu-Edit", ObjectName= "{\"local_Lang\":\"顶级菜单编辑按钮\",\"en_US\":\"\"}", ObjectType= ObjectType.Function });
                dbContext.SaveChanges();
            }
            if (!dbContext.SecurityQuestions.Any())
            {
                dbContext.SecurityQuestions.AddRange(new List<SecurityQuestion> {
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您母亲的姓名是?\",\"en_US\":\"What's your mother's name?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您父亲的姓名是?\",\"en_US\":\"What's your father's name?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您配偶的姓名是?\",\"en_US\":\"What's your spouse's name?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您的出生地是?\",\"en_US\":\"Where was your birthplace?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您高中班主任的名字是?\",\"en_US\":\"What's the name of your junior high school head teacher?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您初中班主任的名字是?\",\"en_US\":\"What's the name of your senior class teacher?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您小学班主任的名字是?\",\"en_US\":\"What's your primary school teacher's name?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您的学号（或工号）是?\",\"en_US\":\"What's your student number (or work number)?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您父亲的生日是?\",\"en_US\":\"What's your father's birthday?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您母亲的生日是?\",\"en_US\":\"What's your mother's birthday?\"}", CreateTime=utcNow, ModifyTime=utcNow},
                    new SecurityQuestion{  QuestionContent="{\"zh_CN\":\"您配偶的生日是?\",\"en_US\":\"What's your spouse's birthday?\"}", CreateTime=utcNow, ModifyTime=utcNow}
                });
                dbContext.SaveChanges();
            }

            if (!dbContext.UserSecurityQuestions.Any())
            {
                dbContext.UserSecurityQuestions.Add(new UserSecurityQuestion { UserID = 1, QuestionID = 1, Answer = "Mother", CreateTime = utcNow, ModifyTime = utcNow });
                dbContext.SaveChanges();
            }
        }
    }
}
