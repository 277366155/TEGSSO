using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Param;

namespace TEG.SSO.Service
{
    public static class AutoMapperHelper
    {
        public static TD MapTo< TS,TD>(this TS obj)
        {
            return Mapper.Map<TS, TD>(obj);
        }
        public static void Config()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<SecurityQuestion, QuestionInfo>();
                cfg.CreateMap<User, UserInfo>().AfterMap((s, d) =>
                {
                    d.UserID = s.ID;
                });
                cfg.CreateMap<NewUser, User>().AfterMap((s,d)=> {
                    var utcNow = DateTime.UtcNow;
                    d.CreateTime = utcNow;
                    d.IsNew = true;
                    d.ModifyTime = utcNow;
                    d.PasswordModifyTime = utcNow;                         
                });
                cfg.CreateMap<AuthorizationObject, MenuChildrenObject>().ForMember(d=>d.PermissionValue,s=>s.Ignore())
                .AfterMap((s, d) => {                    
                    //d.PermissionValue = s.RoleRights.FirstOrDefault(a => a.PermissionValue);
                });
              cfg.CreateMap<Menu, RoleMenu>().ForMember(d=>d.PermissionValue,s=>s.Ignore())
                .AfterMap((s, d) =>
                {
                    if (s.Children != null)
                    {
                        d.ChildrenMenus = Mapper.Map<List<RoleMenu>>(s.Children.ToList());
                    }
                    if (s.AuthorizationObjects != null)
                    {
                        d.MenuChildrenObjects = Mapper.Map<List<MenuChildrenObject>>(s.AuthorizationObjects.ToList());
                    }
                });
            });
        }
    }
}
