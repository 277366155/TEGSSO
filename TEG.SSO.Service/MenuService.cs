using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.Service
{
    public class MenuService : ServiceBase<Menu>
    {
        public MenuService(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public List<RoleMenu> GetRoleMenuByUserID(int userId)
        {

            return null;
        }
    }
}
