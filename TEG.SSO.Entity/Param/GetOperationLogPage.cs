namespace TEG.SSO.Entity.Param
{
    public class GetOperationLogPage : RequestBase<GetOperationLogPager>
    { }
    public class GetOperationLogPager:Pager
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public int? UserID { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string AccountName { get; set; }
        /// <summary>
        /// token
        /// </summary>
        public string UserToken { get; set; }
        /// <summary>
        /// 系统码
        /// </summary>
        public string SystemCode { get; set; }
        /// <summary>
        /// 系统ip
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 接口码
        /// </summary>
        public string ActionCode { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Description { get; set; }
    }
}
