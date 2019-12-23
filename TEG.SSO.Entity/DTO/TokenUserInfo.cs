using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Entity.DTO
{
   public class TokenUserInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录ip
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 当前身份凭证
        /// </summary>
        public string Token { get; set; }
    }
}
