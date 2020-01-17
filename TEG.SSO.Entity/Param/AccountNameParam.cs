using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 检查登录账号是否存在参数实体
    /// </summary>
    public class AccountNameParam : RequestBase<CheckAccountName>
    {}

    /// <summary>
    /// 检查登录账号是否存在参数实体
    /// </summary>
    public class CheckAccountName
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        [StringLength(64)]
        public string AccountName { get; set; }
    }
}
