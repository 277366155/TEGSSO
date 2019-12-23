using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 用户id列表
    /// </summary>
   public  class UserIDList:RequestBase
    {
        /// <summary>
        /// 用户id集合
        /// </summary>
        [Required]
        public List<int> UserIDs { get; set; }
    }

}
