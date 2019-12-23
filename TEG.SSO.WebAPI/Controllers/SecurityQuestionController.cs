using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Filter;

namespace TEG.SSO.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityQuestionController : BaseController
    {
        SecurityQuestionService _questionService;
        public SecurityQuestionController(SecurityQuestionService questionService)
        {
            _questionService = questionService;
        }

        #region 密保问题相关接口
        /// <summary>
        /// 获取当前用户密保问题
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetCurrentQuestion")]
        [CustomAuthorize]
        public async Task<ActionResult<Result<QuestionInfo>>> GetCurrentQuestionAsync(PasswordRequest param)
        {
            return await _questionService.GetCurrentQuestionByUserAsync(param);
        }

        /// <summary>
        /// 获取所有密保问题列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetQuestionList")]
        public async Task<ActionResult<Result<List<QuestionInfo>>>> GetAllSecurityQuestionAsync(RequestBase param)
        {
            return await _questionService.GetAllQuestionsAsync(param.Lang);
        }
        /// <summary>
        /// 设置密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SetQuestion")]
        [CustomAuthorize]
        public async Task<ActionResult<Result>> SetSecurityQuestionAsync(SetSecurityQuestion param)
        {
            return await _questionService.SetSecurityQuestionAsync(param);
        }

        /// <summary>
        /// 重置密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ResetQuestion")]
        [CustomAuthorize]
        public async Task<ActionResult<Result>> ResetSecurityQuestionAsync(ResetSecurityQuestion param)
        {
            return await _questionService.ResetSecurityQuestionAsync(param);
        }

        #endregion 密保问题相关接口
    }
}