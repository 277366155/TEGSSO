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
        [CustomAuthorize("获取当前用户密保问题", "GetCurrentQuestion")]
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
        [CustomAuthorize("获取密保问题列表", "GetQuestionList",false)]
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
        [CustomAuthorize("增加新的密保问题选项", "AddSecurityQuestion")]
        public async Task<ActionResult<Result>> AddSecurityQuestionAsync(AddNewQuestion param)
        {
            return await  _questionService.AddSecurityQuestionAsync(param);
        }

        /// <summary>
        /// 查询指定密保问题选项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("GetQuestionByIDs")]
        [CustomAuthorize("查询指定密保问题选项", "GetQuestionByIDs")]
        public async Task<ActionResult<Result<List<SecurityQuestion>>>> GetQuestionByIDsAsync(RequestIDs param)
        {
            var data= await _questionService.GetListAsync(a=>param.IDs.Contains(a.ID));
            return new SuccessResult<List<SecurityQuestion>> { Data = data };
        }
        /// <summary>
        /// 更新密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("UpdateQuestion")]
        [CustomAuthorize("更新密保问题选项", "UpdateQuestion")]
        public  ActionResult<Result> UpdateQuestionAsync(UpdateQuestion param)
        {
            return   _questionService.UpdateSecurityQuestionAsync(param);
        }

        /// <summary>
        /// 删除密保问题选项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DeleteSecurityQuestion")]
        [CustomAuthorize("删除密保问题选项", "DeleteSecurityQuestion")]
        public async Task<ActionResult<Result>> DeleteSecurityQuestionAsync(RequestIDs param)
        {
            await  _questionService.DeleteManyAsync(a => param.IDs.Contains(a.ID));
            return new  SuccessResult();
        }
        /// <summary>
        /// 设置密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SetQuestion")]
        [CustomAuthorize("设置密保", "SetQuestion")]
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
        [CustomAuthorize("重置密保", "ResetQuestion")]
        public async Task<ActionResult<Result>> ResetSecurityQuestionAsync(ResetSecurityQuestion param)
        {
            return await _questionService.ResetSecurityQuestionAsync(param);
        }

        #endregion 密保问题相关接口
    }
}