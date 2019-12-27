using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Entity.DBModel;

namespace TEG.SSO.Service
{
    public class AuthorizationObjectService : ServiceBase<AuthorizationObject>
    {
        public AuthorizationObjectService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
