using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 成功，不返回data数据
    /// </summary>
    public  class SuccessResult:Result
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public override bool IsSuccess { get => true; }
    }

    /// <summary>
    /// 成功，返回data数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SuccessResult<T> : Result<T>
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public override bool IsSuccess { get => true; }
    }
}
