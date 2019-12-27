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

        public static T MapTo<T>(this object obj) where T : class
        {
            if (obj == null)
            {
                return null;
            }
            return Mapper.Map<T>(obj);
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
                cfg.CreateMap<NewUser, User>().AfterMap((s, d) =>
                {
                    var utcNow = DateTime.UtcNow;
                    d.CreateTime = utcNow;
                    d.IsNew = true;
                    d.ModifyTime = utcNow;
                    d.PasswordModifyTime = utcNow;
                });

                cfg.CreateMap<AuthorizationObject, MenuChildrenObject>().ForMember(d => d.PermissionValue, s => s.Ignore());

                cfg.CreateMap<Menu, RoleMenu>().ForMember(d => d.PermissionValue, s => s.Ignore())
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

                cfg.CreateMap<Organization, DeptAndChildrenInfo>().AfterMap((s,d)=> {
                    d.DeptID = s.ID;
                    d.DeptName = s.OrgName;
                    if (s.Children != null)
                    {
                        d.Children = s.Children.ToList().MapTo<List<DeptAndChildrenInfo>>();
                    }
                });
                cfg.CreateMap<Organization, DeptAndParentInfo>().AfterMap((s,d)=> {
                    d.DeptID = s.ID;
                    d.DeptName = s.OrgName;
                    if (s.Parent != null)
                    {
                        d.Parent = s.Parent.MapTo<DeptAndParentInfo>();
                    }
                });
                cfg.CreateMap<NewDept, Organization>().AfterMap((s,d)=> {
                    d.ParentID = s.ParentID;
                    d.OrgName = s.DeptName;
                    var utcNow = DateTime.UtcNow;
                    d.ModifyTime = utcNow;
                    d.CreateTime = utcNow;                    
                });

                cfg.CreateMap<Dept, Organization>().AfterMap((s,d)=> {
                    d.ParentID = s.ParentID;
                    d.OrgName = s.DeptName;
                    var utcNow = DateTime.UtcNow;
                    d.ModifyTime = utcNow;
                });
                cfg.CreateMap<Entity.Param.UDRel, Entity.DBModel.UserDeptRel>().AfterMap((s,d)=> {
                    var utcNow = DateTime.UtcNow;
                    d.ModifyTime = utcNow;
                    d.CreateTime = utcNow;
                });
                cfg.CreateMap<Operation, OperationLog>();
            });
        }
    }
}
