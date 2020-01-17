namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 失败，不返回data数据
    /// </summary>
    public class FailResult:Result
    {
        public FailResult()
        { }
        public FailResult(string msg)
        {
            this.Msg = msg;
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        public override bool IsSuccess { get => false; }
    }
    /// <summary>
    /// 失败，返回data数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FailResult<T> : Result<T>
    {
        public override bool IsSuccess { get => false; }
    }
}
