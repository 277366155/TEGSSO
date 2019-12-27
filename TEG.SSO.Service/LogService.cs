using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TEG.SSO.Common;
using TEG.SSO.EFCoreContext;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.Service
{
    public class LogService
    {
        private LogContext logContext;
        public LogService(IServiceProvider serviceProvider)
        {
            logContext = (LogContext)serviceProvider.GetService(typeof(LogContext));
        }

        #region 日志记录方法
        /// <summary>
        /// 错误日志记录
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ErrorLogAsync(ExceptionContext context)
        {
            try
            {
                var token = context.HttpContext.Request.Headers["Authorization"];
                var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
                var url = context.HttpContext.Request.GetUriPath();
                var request = context.HttpContext.Request.GetRequestParam();
                var errorMsg = "msg:" + context.Exception.Message + "\r\nStackTrace:" + context.Exception.StackTrace;

                var utcNow = DateTime.UtcNow;
                await logContext.ErrorLogs.AddAsync(new ErrorLog { Token = token, ErrorMsg = errorMsg, IP = ip, Url = url, Request = request, CreateTime = utcNow, ModifyTime = utcNow });
                await logContext.SaveChangesAsync();
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task OperationLogAsync(Operation param)
        {
            try
            {
                var entity = param.MapTo<OperationLog>();
                var utcNow = DateTime.UtcNow;
                entity.ModifyTime = utcNow;
                entity.CreateTime = utcNow;

                await logContext.OperationLogs.AddAsync(entity);
                await logContext.SaveChangesAsync();
            }
            catch (Exception ex)
            { }
        }
        #endregion 日志记录方法
    }
}
