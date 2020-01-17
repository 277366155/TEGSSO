using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.LogDBContext;

namespace TEG.SSO.AdminService.Base
{
   public class DbContextSeed
    {

        public static void DbInit(IServiceProvider svp)
        {
          var   logContext = (LogContext)svp.GetService(typeof(LogContext));
            logContext.Database.Migrate();
        }
    }
}
