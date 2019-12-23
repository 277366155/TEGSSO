using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 用户信息更新参数
    /// </summary>
    public class UpdateUsers : RequestBase
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        public List<UpdateUser> Users { get; set; }
    }

    public class UpdateUser : NewUser
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }

    }
}
