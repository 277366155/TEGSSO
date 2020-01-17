using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.Entity.Param
{
    /// <summary>
    /// 用户信息更新参数
    /// </summary>
    public class UpdateUser : RequestBase<List<UpdateUserParam>>
    {
    }
    /// <summary>
    /// 更新用户信息
    /// </summary>
    public class UpdateUserParam : NewUser
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [StringLength(256, MinimumLength = 2)]
        public new string Password { get; set; }
        /// <summary>
        /// 角色信息
        /// </summary>
        public List<int> Roles { get; set; }
        /// <summary>
        /// 部门信息
        /// </summary>
        public List<int> Depts { get; set; }
    }
}
