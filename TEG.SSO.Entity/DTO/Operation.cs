using System.ComponentModel.DataAnnotations;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 操作实例
    /// </summary>
    public  class Operation
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        [StringLength(64)]
        public string AccountName { get; set; }
        /// <summary>
        /// token
        /// </summary>
        [StringLength(256)]
        public string UserToken { get; set; }
        /// <summary>
        /// 系统编码
        /// </summary>
        [Required]
        [StringLength(64)]
        public string SystemCode { get; set; }
        /// <summary>
        /// 请求接口编码
        /// </summary>
        [Required]
        [StringLength(64)]
        public string ActionCode { get; set; }
        /// <summary>
        /// 请求端ip
        /// </summary>
        [Required]
        [StringLength(64)]
        public string AccessHost { get; set; }
        /// <summary>
        /// 请求路由
        /// </summary>
        [Required]
        [StringLength(256)]
        public string Url { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [StringLength(512)]
        public string Description { get; set; }
    }
}
