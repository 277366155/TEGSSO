using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using TEG.SSO.Common;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.AdminService
{
    public  class SecurityQuestionService:BaseService
    {
        static string GetQuestionListUrl=ApiHost+"/api/SecurityQuestion/GetQuestionList";

        public SecurityQuestionService(IHttpContextAccessor accessor) : base(accessor)
        { 
        }


        public List<QuestionInfo> GetQuestionList()
        {
            var data = Post<List<QuestionInfo>>(GetQuestionListUrl);
            if (data.IsSuccess)
            {
                return data.Data;
            }
            return null;
        }

 
    }
}
