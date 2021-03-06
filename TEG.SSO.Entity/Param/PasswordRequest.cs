﻿using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.Param
{
    public   class PasswordRequest:RequestBase
    {
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Password { get; set; }

    }
}
