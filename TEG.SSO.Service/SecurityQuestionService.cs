using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.Service
{
    public class SecurityQuestionService : ServiceBase<SecurityQuestion>
    {
        public SecurityQuestionService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 获取当前密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result<QuestionInfo>> GetCurrentQuestionByUserAsync(PasswordRequest param)
        {
            var result = new Result<QuestionInfo>();
            //验证密码
            var userExist = readOnlyContext.Users.Any(a => a.ID == currentUser.UserID && a.Password == param.Password);
            if (!userExist)
            {
                result.Code = "password error";
                result.Msg = "密码错误";
                return result;
            }
            //获取密保问题
            var currentQuestion = await masterContext.UserSecurityQuestions.Where(a => a.UserID == currentUser.UserID).Include(a => a.SecurityQuestion)?.FirstOrDefaultAsync();
            if (currentQuestion == null)
            {
                result.Code = "no security question";
                result.Msg = "没有密保问题";
                return result;
            }
            //密保问题映射实体
            var questionInfo = Mapper.Map<QuestionInfo>(currentQuestion.SecurityQuestion);
            questionInfo.QuestionContent = questionInfo.QuestionContent.JsonToObj<MultipleLanguage>().GetContent(param.Lang);
            result.Data = questionInfo;
            result.IsSuccess = true;

            return result;
        }

        /// <summary>
        /// 设置安全问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> SetSecurityQuestionAsync(SetSecurityQuestion param)
        {
            /*
             * 1，校验密码、questionid是否存在数据
             * 2，存储user-question
             * 3，日志记录
             * **/
            var result = new Result();
            var userExist = masterContext.Users.Any(a => !a.IsDisabled && a.ID == currentUser.UserID && a.Password == param.Password);
            if (!userExist)
            {
                result.Code = "SSO.Global.Error.Content..13";
                result.Msg = "密码错误";
                return result;
            }
            var userQuestionExist = masterContext.UserSecurityQuestions.Any(a => a.UserID == currentUser.UserID);
            if (userQuestionExist)
            {
                result.Code = "SSO.Global.Error.Content..12";
                result.Msg = "已设置安全问题";
                return result;
            }
            var question = await base.GetFirstOrDefaultAsync(a => !a.IsDisabled && a.ID == param.SecurityQuestionID, fromMasterDb: true);
            if (question == null)
            {
                result.Code = "SSO.Global.Error.Content..11";
                result.Msg = "未知的问题选项";
                return result;
            }

            var utcNow = DateTime.UtcNow;
            masterContext.UserSecurityQuestions.Add(new UserSecurityQuestion
            {
                UserID = currentUser.UserID,
                QuestionID = param.SecurityQuestionID,
                Answer = param.Answer,
                CreateTime = utcNow,
                ModifyTime = utcNow
            });
            var saveResult = await masterContext.SaveChangesAsync() > 0;
            //todo:异步日志记录
            return new Result() { IsSuccess = saveResult, Msg = saveResult ? null : "保存失败" };
        }

        /// <summary>
        /// 重置安全问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> ResetSecurityQuestionAsync(ResetSecurityQuestion param)
        {
            /*
             * 1，校验密码、原安全问题答案、新问题数据
             * 2，更新user-question记录
             * 3，日志记录
             * **/
            var result = new Result();

            var userExist = masterContext.Users.Any(a => !a.IsDisabled && a.ID == currentUser.UserID && a.Password == param.Password);
            if (!userExist)
            {
                result.Code = "SSO.Global.Error.Content..13";
                result.Msg = "密码错误";
                return result;
            }
            var oldQuestioin = masterContext.UserSecurityQuestions.FirstOrDefault(a => a.UserID == currentUser.UserID && a.QuestionID == param.OldSecurityQuestionID && a.Answer == param.OldSecurityQuestionAnswer);

            if (oldQuestioin == null)
            {
                result.Code = "SSO.Global.Error.Content..11";
                result.Msg = "原安全问题错误";
                return result;
            }
            var question = await GetFirstOrDefaultAsync(a => !a.IsDisabled && a.ID == param.SecurityQuestionID, fromMasterDb: true);
            if (question == null)
            {
                result.Code = "SSO.Global.Error.Content..11";
                result.Msg = "未知的问题选项";
                return result;
            }
            var utcNow = DateTime.UtcNow;
            oldQuestioin.QuestionID = param.SecurityQuestionID;
            oldQuestioin.Answer = param.Answer;
            oldQuestioin.ModifyTime = utcNow;
            var saveResult = await masterContext.SaveChangesAsync() > 0;

            //todo:异步日志记录
            return new Result() { IsSuccess = saveResult, Msg = saveResult ? null : "保存失败" };
        }

        /// <summary>
        /// 获取所有问题列表
        /// </summary>
        /// <param name="lang">语言类型</param>
        /// <returns></returns>
        public async Task<Result<List<QuestionInfo>>> GetAllQuestionsAsync(Language lang)
        {
            var list = await GetListAsync(a => !a.IsDisabled, a => a.ID, SortOption.ASC);
            list.ForEach(a => a.QuestionContent = a.QuestionContent.JsonToObj<MultipleLanguage>().GetContent(lang));
            return new SuccessResult<List<QuestionInfo>> { Data = Mapper.Map<List<QuestionInfo>>(list) };
        }
    }
}
