using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
    public class SystemCode : RequestBase<SystemCodes>
    { }

    /// <summary>
    /// 获取指定系统的信息
    /// </summary>
    public class SystemCodes
    {
        /// <summary>
        /// 获取相应信息的目标系统码
        /// </summary>
        [Required]
        public List<string> SysCodes { get; set; }
    }
}
