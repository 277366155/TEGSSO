using System;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 用户基本信息
    /// </summary>
    public class UserInfo
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
        /// 用户名字
        /// </summary>   
        public string UserName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>     
        public string Mobile { get; set; }
        /// <summary>
        /// 座机电话
        /// </summary>
        public string Telphone { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// qq号码
        /// </summary>
        public string QQ { get; set; }          
    }
}
