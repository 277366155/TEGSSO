using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TEG.SSO.Entity.Param
{
    public class UpdateDept : RequestBase<List<Dept>>
    {
    }

    public class Dept:NewDept
    {
        /// <summary>
        /// 部门id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }
    }
}
