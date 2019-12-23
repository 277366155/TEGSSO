using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Entity.Enum;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    ///页面中权限控制对象
    /// </summary>
   public  class MenuChildrenObject
    {
        /// <summary>
        ///view元素code
        /// </summary>
        public string ObjectCode { get; set; }
        /// <summary>
        /// 名称描述
        /// </summary>
        public string ObjectName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ObjectType ObjectType { get; set; }

        /// <summary>
        /// 权限值
        /// </summary>
        public PermissionValue PermissionValue { get; set; }
    }
}
