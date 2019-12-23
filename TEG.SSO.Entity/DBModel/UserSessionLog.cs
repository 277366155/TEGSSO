using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 用户token信息生成、销毁记录表
    /// </summary>
    [Table("UserSessionLog")]
    public class UserSessionLog:DBModelBase
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 生成的用户token
        /// </summary>
        [StringLength(256)]
        public string UserToken { get; set; }
        /// <summary>
        /// 所属的系统id
        /// </summary>
        public int SystemID { get; set; }
        /// <summary>
        /// 当时的系统名称
        /// </summary>
        [StringLength(64)]
        public string SystemName { get; set; }
        /// <summary>
        /// 主机IP地址
        /// </summary>
        [StringLength(64)]
        public string AccessHost { get; set; }
        /// <summary>
        /// session有效时间，单位：秒
        /// </summary>
        public int ValidTime { get; set; }
        /// <summary>
        /// session真实过期时间，手动刷新的情况
        /// </summary>
        public DateTime RealExpirationTime { get; set; }

    }
}
