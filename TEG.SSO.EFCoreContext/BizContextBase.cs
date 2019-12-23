using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;

namespace TEG.SSO.EFCoreContext
{
    public class BizContextBase : DbContext
    {
        //bool _readOnly;
        //public BizContextBase(bool readOnly)
        //{
        //    this._readOnly = readOnly;
        //}

        ///// <summary>
        ///// 根据实例类型选择数据库连接
        ///// </summary>
        ///// <param name="optionsBuilder"></param>
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(BaseCore.Configuration.GetConnectionString(_readOnly? "ReadOnlyConn" : "MasterConn"));
        //    base.OnConfiguring(optionsBuilder);
        //}

        public BizContextBase(DbContextOptions<BizMasterContext> contextOptions) : base(contextOptions)
        {
        }
        public BizContextBase(DbContextOptions<BizReadOnlyContext> contextOptions) : base(contextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSystem>();
            modelBuilder.Entity<Menu>();
            modelBuilder.Entity<AuthorizationObject>();            
            modelBuilder.Entity<Organization>();
            modelBuilder.Entity<Role>();
            modelBuilder.Entity<RoleRight>();
            modelBuilder.Entity<SecurityQuestion>();
            modelBuilder.Entity<User>();
            modelBuilder.Entity<UserDeptRel>();
            modelBuilder.Entity<UserRoleRel>();
            modelBuilder.Entity<UserSecurityQuestion>();
            modelBuilder.Entity<UserSessionLog>();
        }

        public DbSet<AppSystem> AppSystems { get; set; }
        public DbSet<AuthorizationObject> AuthorizationObjects { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleRight> RoleRights { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }
        public DbSet<UserSecurityQuestion> UserSecurityQuestions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDeptRel> UserDeptRels { get; set; }
        public DbSet<UserRoleRel> UserRoleRels { get; set; }
        public DbSet<UserSessionLog> UserSessionLogs { get; set; }
    }
}
