using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 用户操作日志信息
    /// </summary>
    [Table("OperationLog")]
   public   class OperationLog:DBModelBase
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 用户姓名
        /// </summary>
        [StringLength(64)]
        public string UserName { get; set; }
        /// <summary>
        /// 生成的用户token
        /// </summary>
        [StringLength(256)]
        public string UserToken { get; set; }
        /// <summary>
        /// 系统id
        /// </summary>
        public int SystemID { get; set; }
        /// <summary>
        /// 系统名
        /// </summary>
        [StringLength(64)]
        public string SystemName { get; set; }
        /// <summary>
        /// 系统主机
        /// </summary>
        [StringLength(64)]
        public string AccessHost { get; set;}
        /// <summary>
        /// 操作code
        /// </summary>
        [StringLength(64)]
        public string ActionCode { get; set; }
        /// <summary>
        /// 操作路径
        /// </summary>
        [StringLength(256)]
        public string URL { get; set; }
        /// <summary>
        /// 操作说明
        /// </summary>
        [StringLength(512)]
        public string Description { get; set; }
    }
}
