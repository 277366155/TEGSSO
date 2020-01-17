using System;
using System.Collections.Generic;
using System.Text;

namespace TEG.SSO.Entity.DTO
{
    /// <summary>
    /// 响应结果，无数据
    /// </summary>
    public class Result
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public virtual bool IsSuccess { get; set; }
        /// <summary>
        /// 返回码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Msg { get; set; }

    }

    /// <summary>
    /// 响应结果，带数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T> : Result
    {
        public Result()
        {
        }
        public Result(T data)
        {
            Data = data;
        }
        /// <summary>
        /// 响应数据
        /// </summary>
        public T Data { get; set; }
    }
}
