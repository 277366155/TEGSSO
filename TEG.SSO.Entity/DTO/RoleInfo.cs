namespace TEG.SSO.Entity.DTO
{
    public class RoleInfo
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public int RoleID { get; set; }
        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSuperAdmin{get;set;}
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
    }
}
