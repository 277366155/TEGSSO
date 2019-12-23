using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Entity.DBModel;

namespace TEG.SSO.Service
{
    public class AppSystemService : ServiceBase<AppSystem>
    {
        public AppSystemService(IServiceProvider svp) : base(svp)
        { }
    }
}
