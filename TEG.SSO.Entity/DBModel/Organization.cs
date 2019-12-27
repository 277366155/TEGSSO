using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TEG.SSO.Entity.DBModel
{
    /// <summary>
    /// 组织架构信息
    /// </summary>
    [Table("Organization")]
    public class Organization:DBModelBase
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        [StringLength(64)]
        public string OrgName { get; set; }
        
        /// <summary>
        /// 父级id
        /// </summary>        
        public int? ParentID { get; set; }

        /// <summary>
        /// 父级节点
        /// </summary>
        [ForeignKey("ParentID")]
        public virtual Organization Parent { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<Organization> Children { get; set; }
    }
}
