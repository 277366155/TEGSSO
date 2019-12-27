using Microsoft.EntityFrameworkCore;

namespace TEG.SSO.EFCoreContext
{
    public class BizMasterContext:BizContextBase
    {
        /// <summary>
        /// 主库DBContext
        /// </summary>
        public BizMasterContext(DbContextOptions<BizMasterContext> options) : base(options)
        {
        }
    }
}
