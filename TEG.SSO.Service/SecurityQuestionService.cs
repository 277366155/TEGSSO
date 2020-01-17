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
            //验证密码
            var userExist = readOnlyContext.Users.Any(a => a.ID == currentUser.UserID && a.Password == param.Data.Password);
            if (!userExist)
            {
                throw new CustomException("PasswordError", "密码错误");
            }
            //获取密保问题
            var currentQuestion = await masterContext.UserSecurityQuestions.Where(a => a.UserID == currentUser.UserID).Include(a => a.SecurityQuestion)?.FirstOrDefaultAsync();
            if (currentQuestion == null)
            {
                throw new CustomException("NoSecurityQuestion", "未设置密保问题");
            }
            //密保问题映射实体
            var questionInfo = Mapper.Map<QuestionInfo>(currentQuestion.SecurityQuestion);
            questionInfo.QuestionContent = questionInfo.QuestionContent.JsonToObj<MultipleLanguage>().GetContent(param.Lang);

            return new SuccessResult<QuestionInfo> { Data = questionInfo };
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
             * **/
            var userExist = masterContext.Users.Any(a => !a.IsDisabled && a.ID == currentUser.UserID && a.Password == param.Data.Password);
            if (!userExist)
            {
                throw new CustomException("PasswordError", "密码错误");
            }
            var userQuestionExist = masterContext.UserSecurityQuestions.Any(a => a.UserID == currentUser.UserID);
            if (userQuestionExist)
            {
                throw new CustomException("SecurityQuestionExist", "已有密保问题");
            }
            var question = await base.GetFirstOrDefaultAsync(a => !a.IsDisabled && a.ID == param.Data.SecurityQuestionID, fromMasterDb: true);
            if (question == null)
            {
                throw new CustomException("SecurityQuestionError", "未知的问题选项");
            }

            var utcNow = DateTime.UtcNow;
            masterContext.UserSecurityQuestions.Add(new UserSecurityQuestion
            {
                UserID = currentUser.UserID,
                QuestionID = param.Data.SecurityQuestionID,
                Answer = param.Data.Answer,
                CreateTime = utcNow,
                ModifyTime = utcNow,
                LastUpdateAccountName = currentUser.AccountName
            });
            await masterContext.SaveChangesAsync();

            return new SuccessResult();
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

            var userExist = masterContext.Users.Any(a => !a.IsDisabled && a.ID == currentUser.UserID && a.Password == param.Data.Password);
            if (!userExist)
            {
                throw new CustomException("PasswordError", "密码错误");
            }
            var oldQuestioin = masterContext.UserSecurityQuestions.FirstOrDefault(a => a.UserID == currentUser.UserID && a.QuestionID == param.Data.OldSecurityQuestionID && a.Answer == param.Data.OldSecurityQuestionAnswer);

            if (oldQuestioin == null)
            {
                throw new CustomException("OldAnsowerError", "原安全问题回答错误");
            }
            var question = await GetFirstOrDefaultAsync(a => !a.IsDisabled && a.ID == param.Data.SecurityQuestionID, fromMasterDb: true);
            if (question == null)
            {
                throw new CustomException("SecurityQuestionError", "未知的问题选项");
            }
            var utcNow = DateTime.UtcNow;
            oldQuestioin.QuestionID = param.Data.SecurityQuestionID;
            oldQuestioin.Answer = param.Data.Answer;
            oldQuestioin.ModifyTime = utcNow;
            oldQuestioin.LastUpdateAccountName = currentUser.AccountName;
            await masterContext.SaveChangesAsync();

            return new SuccessResult();
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

        /// <summary>
        /// 新增密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> AddSecurityQuestionAsync(AddNewQuestion param)
        {
            var list = new List<SecurityQuestion>();
            var utcNow = DateTime.UtcNow;
            param.Data.ForEach(a =>
            {
                list.Add(new SecurityQuestion
                {
                    CreateTime = utcNow,
                    LastUpdateAccountName = currentUser.AccountName,
                    ModifyTime = utcNow,
                    QuestionContent = a.ToJson()
                });
            });
            await  masterDbSet.AddRangeAsync(list);
            await  masterContext.SaveChangesAsync();
            return new SuccessResult();
        }

        /// <summary>
        /// 更新密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public  Result UpdateSecurityQuestionAsync(UpdateQuestion param)
        {
            var list = new List<SecurityQuestion>();
            var utcNow = DateTime.UtcNow;
            param.Data.ForEach( a =>
            {
                var data=  masterDbSet.FirstOrDefault(m=>m.ID==a.ID);
                data.LastUpdateAccountName = currentUser.AccountName;
                data.ModifyTime = utcNow;
                data.QuestionContent = a.Content.ToJson();
                data.IsDisabled = a.IsDisabled;
                masterContext.SaveChanges();
            });
         
            return new SuccessResult();
        }

        /// <summary>
        /// 删除指定的密保问题
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<Result> DeleteSecurityQuestionAsync(RequestID param)
        {
            if (param.Data.IDs.Any(a => !masterDbSet.Any(m => m.ID == a)))
            {
                throw new CustomException("IDError","错误的ID信息");
            }
            var userQuestionExist = masterDbSet.Where(a => param.Data.IDs.Contains(a.ID)).Include(a => a.UserSecurityQuestions).Any(a => a.UserSecurityQuestions != null && a.UserSecurityQuestions.Count() > 0);
            if (userQuestionExist)
            {
                throw new CustomException("UserQuestionExist", "密保问题被使用");
            }
            await DeleteManyAsync(a => param.Data.IDs.Contains(a.ID));
            return new SuccessResult();
        }
    }
}
