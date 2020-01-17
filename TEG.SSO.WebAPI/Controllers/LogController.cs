using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Param;
using TEG.SSO.LogDBContext;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Filter;

namespace TEG.SSO.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : BaseController
    {
        LogService _logService;
        public LogController(LogService logService)
        {
            _logService = logService;
        }

        /// <summary>
        /// 分页查询错误日志
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetErrorLogPage")]
        [CustomAuthorize(Description = "分页查询错误日志",ActionCode = "GetErrorLogPage")]
        public ActionResult<Result<Page<ErrorLog>>> GetErrorLogPage(PageParam param)
        {
          return   _logService.GetErrorLogPage(param);
        }

        /// <summary>
        /// 根据id查询指定错误日志
        /// </summary>
        /// <param name="param"></param>
        [HttpPost("GetErrorLogInfo")]
        [CustomAuthorize(Description = "根据id查询指定错误日志", ActionCode ="GetErrorLogInfo")]
        public ActionResult<Result<List<ErrorLog>>> GetErrorLogInfo(RequestID param)
        {
            return _logService.GetErrorLogInfo(param);
        }

        /// <summary>
        /// 分页查询操作日志
        /// </summary>
        /// <param name="param"></param>
        [HttpPost("GetOperationLogPage")]
        [CustomAuthorize(Description = "分页查询操作日志",ActionCode = "GetOperationLogPage")]
        public ActionResult<Result<Page<OperationLog>>> GetOperationLogPage(GetOperationLogPage param)
        {
            return _logService.GetOperationLogPage(param);
        }
        /// <summary>
        /// 根据id查询指定操作日志
        /// </summary>
        /// <param name="param"></param>
        [HttpPost("GetOperationLogInfo")]
        [CustomAuthorize(Description = "根据id查询指定操作日志",ActionCode = "GetOperationLogInfo")]
        public ActionResult<Result<List<OperationLog>>> GetOperationLogInfo(RequestID param)
        {
            return _logService.GetOperationLogInfo(param);
        }
    }
}