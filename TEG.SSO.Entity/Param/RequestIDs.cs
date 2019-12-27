using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 通过id请求数据
    /// </summary>
    public class RequestIDs:RequestBase
    {
        /// <summary>
        ///id列表
        /// </summary>
        [Required]
        public List<int> IDs { get; set; }
    }

}
