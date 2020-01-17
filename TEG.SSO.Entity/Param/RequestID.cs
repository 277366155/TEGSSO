using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    public class RequestID : RequestBase<RequestIDs>
    { }
    /// <summary>
    /// 通过id请求数据
    /// </summary>
    public class RequestIDs
    {
        /// <summary>
        ///id列表
        /// </summary>
        [Required]
        public List<int> IDs { get; set; }
    }

}
