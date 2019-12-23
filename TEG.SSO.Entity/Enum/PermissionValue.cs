using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Entity.Enum
{
    /// <summary>
    /// 权限值
    /// </summary>
    public enum PermissionValue
    {
        /// <summary>
        /// 可见的
        /// </summary>
        Visiable=1,
        /// <summary>
        /// 可执行的
        /// </summary>
        Executable = 2,
        /// <summary>
        /// 可见的+可执行的
        /// </summary>
        VE=Visiable| Executable
    }
}
