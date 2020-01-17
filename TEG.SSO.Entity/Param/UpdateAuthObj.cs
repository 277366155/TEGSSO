using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TEG.SSO.Entity.Param
{
  public   class UpdateAuthObj:RequestBase<List<UpdateAuthObjs>>
    {
    }

    public class UpdateAuthObjs : NewObj
    {
        /// <summary>
        ///功能项id
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int ID { get; set; }
    }
}
