using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.LogDBContext
{
    public class LogService
    {
        private LogContext logContext;
        static LogService()
        {
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<Operation, OperationLog>();
            //});
        }
        public LogService(IServiceProvider serviceProvider)
        {
            logContext = (LogContext)serviceProvider.GetService(typeof(LogContext));          
            logContext.Database.Migrate();
        }

        #region 日志记录方法
        /// <summary>
        /// 错误日志记录
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public void ErrorLog(ExceptionContext context)
        {
            try
            {
                var request = context.HttpContext.Request;
                var token = request.Headers["Authorization"];
                var ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
                var url = (request.PathBase + request.Path).ToString();
                var requestParam = context.HttpContext.Request.GetRequestParam();
                var errorMsg = "msg:" + context.Exception.Message + "\r\nStackTrace:" + context.Exception.StackTrace;

                var utcNow = DateTime.UtcNow;
                logContext.ErrorLogs.Add(new ErrorLog { Token = token, ErrorMsg = errorMsg, IP = ip, Url = url, Request = requestParam, CreateTime = utcNow, ModifyTime = utcNow });
                logContext.SaveChanges();
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 操作日志记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public void OperationLogAsync(Operation param)
        {
            try
            {
                var entity = Mapper.Map<OperationLog>(param);
                var utcNow = DateTime.UtcNow;
                entity.ModifyTime = utcNow;
                entity.CreateTime = utcNow;

                logContext.OperationLogs.Add(entity);
                logContext.SaveChanges();
            }
            catch (Exception ex)
            { }
        }
        #endregion 日志记录方法

        #region 日志查询方法
        #region 错误日志
        /// <summary>
        /// 错误日志分页查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<ErrorLog>> GetErrorLogPage(PageParam param)
        {
            var dataList = logContext.ErrorLogs.OrderByDescending(a => a.ID).ToPage(param.Data.PageIndex, param.Data.PageSize);
            return new SuccessResult<Page<ErrorLog>>(dataList);
        }

        /// <summary>
        /// 获取指定id 的日志信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<List<ErrorLog>> GetErrorLogInfo(RequestID param)
        {
            if (param.Data.IDs.Any(a => !logContext.ErrorLogs.Any(m => m.ID == a)))
            {
                throw new CustomException("LogIDError", "错误的日志ID");
            }
            var data = logContext.ErrorLogs.Where(a => param.Data.IDs.Contains(a.ID)).ToList();
            return new SuccessResult<List<ErrorLog>>(data);
        }
        #endregion 错误日志

        #region 操作日志
        /// <summary>
        /// 分页查询操作日志，id倒序
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<Page<OperationLog>> GetOperationLogPage(GetOperationLogPage param)
        {
            var iQueryable = logContext.OperationLogs.Where(a => true);
            if (param.Data.SystemCode.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.SystemCode.Contains(param.Data.SystemCode));
            }
            if (param.Data.URL.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.URL.Contains(param.Data.URL));
            }
            if (param.Data.UserID.HasValue)
            {
                iQueryable = iQueryable.Where(a => a.UserID == param.Data.UserID);
            }
            if (param.Data.UserToken.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.UserToken.Contains(param.Data.UserToken));
            }
            if (param.Data.AccountName.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.AccountName.Contains(param.Data.AccountName));
            }
            if (param.Data.ActionCode.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.ActionCode.Contains(param.Data.ActionCode));
            }
            if (param.Data.Description.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.Description.Contains(param.Data.Description));
            }
            if (param.Data.IP.IsNotNullOrWhiteSpace())
            {
                iQueryable = iQueryable.Where(a => a.AccessHost.Contains(param.Data.IP));
            }
            var data = iQueryable.OrderByDescending(a => a.ID).ToPage(param.Data.PageIndex, param.Data.PageSize);
            return new SuccessResult<Page<OperationLog>>(data);
        }

        /// <summary>
        /// 根据id查询指定操作日志
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Result<List<OperationLog>> GetOperationLogInfo(RequestID param)
        {
            if (param.Data.IDs.Any(a => !logContext.OperationLogs.Any(m => m.ID == a)))
            {
                throw new CustomException("LogIDError", "错误的日志ID");
            }
            var data = logContext.OperationLogs.Where(a => param.Data.IDs.Contains(a.ID)).ToList();
            return new SuccessResult<List<OperationLog>>(data);
        }
        #endregion 操作日志
        #endregion
    }
}
