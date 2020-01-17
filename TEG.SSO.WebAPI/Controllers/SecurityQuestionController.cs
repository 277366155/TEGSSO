using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
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
        [CustomAuthorize(Description = "获取当前用户密保问题",ActionCode = "GetCurrentQuestion")]
        public async Task<ActionResult<Result<QuestionInfo>>> GetCurrentQuestionAsync(PasswordRequest param)
        {
            return await _questionService.GetCurrentQuestionByUserAsync(param);
        }

        /// <summary>
        /// 获取密保问题列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>      
        [HttpPost("GetQuestionList")]
        [CustomAuthorize(Description = "获取密保问题列表",ActionCode = "GetQuestionList", Verify = false)]
        public async Task<ActionResult<Result<List<QuestionInfo>>>> GetAllSecurityQuestionAsync(RequestBase param)
        {
            return await _questionService.GetAllQuestionsAsync(param.Lang);
        }

        /// <summary>
        /// 增加新的密保问题选项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("AddSecurityQuestion")]
        [CustomAuthorize(Description = "增加新的密保问题选项",ActionCode = "AddSecurityQuestion")]
        public async Task<ActionResult<Result>> AddSecurityQuestionAsync(AddNewQuestion param)
        {
            return await _questionService.AddSecurityQuestionAsync(param);
        }

        /// <summary>
        /// 查询指定密保问题选项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetQuestionByIDs")]
        [CustomAuthorize(Description = "查询指定密保问题选项",ActionCode = "GetQuestionByIDs")]
        public async Task<ActionResult<Result<List<SecurityQuestion>>>> GetQuestionByIDsAsync(RequestID param)
        {
            var data = await _questionService.GetListAsync(a => param.Data.IDs.Contains(a.ID));
            return new SuccessResult<List<SecurityQuestion>> { Data = data };
        }
        /// <summary>
        /// 更新密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UpdateQuestion")]
        [CustomAuthorize(Description = "更新密保问题选项", ActionCode ="UpdateQuestion")]
        public ActionResult<Result> UpdateQuestionAsync(UpdateQuestion param)
        {
            return _questionService.UpdateSecurityQuestionAsync(param);
        }

        /// <summary>
        /// 删除密保问题选项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DeleteSecurityQuestion")]
        [CustomAuthorize(Description = "删除密保问题选项",ActionCode = "DeleteSecurityQuestion")]
        public async Task<ActionResult<Result>> DeleteSecurityQuestionAsync(RequestID param)
        {
            return await _questionService.DeleteSecurityQuestionAsync(param);
        }
        /// <summary>
        /// 设置密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SetQuestion")]
        [CustomAuthorize(Description = "设置密保",ActionCode = "SetQuestion")]
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
        [CustomAuthorize(Description = "重置密保", ActionCode ="ResetQuestion")]
        public async Task<ActionResult<Result>> ResetSecurityQuestionAsync(ResetSecurityQuestion param)
        {
            return await _questionService.ResetSecurityQuestionAsync(param);
        }

        #endregion 密保问题相关接口
    }
}