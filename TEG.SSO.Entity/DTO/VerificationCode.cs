using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DTO
{
   public  class VerificationCode
    {
        public VerificationCode(string accountName ,VerificationCodeType type)
        {
            Type = type;
            AccountName = accountName;
            Code = GetRandCode(6);
        }

        private static string GetRandCode(int length)
        {
            string lib = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string code = "";
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                code += lib.Substring(random.Next(lib.Length), 1);
            }

            return code;
        }

        /// <summary>
        /// 登录账号
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 验证码类型
        /// </summary>
        public VerificationCodeType Type { get; set; }
        
    }


}
