using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;

namespace TEG.SSO.EFCoreContext
{
    public class LogContext:DbContext
    {
        ///// <summary>
        ///// 根据实例类型选择数据库连接
        ///// </summary>
        ///// <param name="optionsBuilder"></param>
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(BaseCore.Configuration.GetConnectionString("LogConn"));
        //    base.OnConfiguring(optionsBuilder);
        //}

        public LogContext(DbContextOptions<LogContext> options):base(options)
        {

        }
        public DbSet<OperationLog> OperationLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
    }
}
