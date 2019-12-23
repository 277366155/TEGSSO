using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 错误日志
    /// </summary>
    [Table("ErrorLog")]
    public class ErrorLog: DBModelBase
    {
        /// <summary>
        /// 请求token值
        /// </summary>
        [StringLength(256)]
        public string Token { get; set; }
        /// <summary>
        /// 请求的接口地址
        /// </summary>
        [StringLength(1024)]
        public string Url { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        [Column(TypeName = "text")]
        public string Request { get; set; }
        /// <summary>
        /// 请求方ip
        /// </summary>
        [StringLength(128)]
        public string IP { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        [Column(TypeName = "text")]
        public string ErrorMsg { get; set; }
    }
}
