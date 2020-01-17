using Microsoft.EntityFrameworkCore;
using TEG.SSO.Entity.DBModel;

namespace TEG.SSO.LogDBContext
{
    public class LogContext : DbContext
    {
        public LogContext(DbContextOptions<LogContext> options) : base(options)
        {

        }
        public DbSet<OperationLog> OperationLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
    }
}
