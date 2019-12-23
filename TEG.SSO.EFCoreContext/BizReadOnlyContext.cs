using Microsoft.EntityFrameworkCore;

namespace TEG.SSO.EFCoreContext
{
    public  class BizReadOnlyContext :BizContextBase
    {
        /// <summary>
        /// 只读库DBContext
        /// </summary>
        public BizReadOnlyContext(DbContextOptions<BizReadOnlyContext> options) : base(options)
        {
        }
    }
}
