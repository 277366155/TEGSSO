using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TEG.SSO.Common;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Entity.Enum;
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
                cfg.CreateMap<RequestBase, SystemCode>().AfterMap((s,d)=> {
                    d.Data = new SystemCodes();                    
                });
                cfg.CreateMap<SecurityQuestion, QuestionInfo>();
                cfg.CreateMap<User, UserInfo>().AfterMap((s, d) =>
                {
                    d.UserID = s.ID;
                });

                cfg.CreateMap<User, UserAndRoleAndDeptInfo>().AfterMap((s, d) => {                 
                    //下面的映射方式会报错
                    //d.Roles = s.UserRoleRels.Select(a => a.Role).ToList();
                    //d.Depts = s.UserDeptRels.Select(a=>a.Dept).ToList();
                });
                cfg.CreateMap<NewUser, User>().AfterMap((s, d) =>
                {
                    var utcNow = DateTime.UtcNow;
                    d.CreateTime = utcNow;
                    d.IsNew = true;
                    d.ModifyTime = utcNow;
                    d.PasswordModifyTime = utcNow;
                });
                cfg.CreateMap<UpdateUserParam,User>().ForMember(d=>d.Password,s=>s.Ignore()).AfterMap((s,d)=> {
                    var utcNow = DateTime.UtcNow;
                    d.ModifyTime = utcNow;
                });
                cfg.CreateMap<AuthorizationObject, MenuChildrenObject>().ForMember(d => d.PermissionValue, s => s.Ignore());

                cfg.CreateMap<Menu, AuthRight>().ForMember(d => d.PermissionValue, s => s.Ignore())
                  .AfterMap((s, d) =>
                  {
                      //if (s.Children != null)
                      //{
                      //    d.ChildrenMenus = Mapper.Map<List<Entity.DTO.AuthRight>>(s.Children.ToList());
                      //}
                      d.MenuID = s.ID;
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
                cfg.CreateMap<UDRel, UserDeptRel>().AfterMap((s,d)=> {
                    var utcNow = DateTime.UtcNow;
                    d.ModifyTime = utcNow;
                    d.CreateTime = utcNow;
                });
                cfg.CreateMap<AuthorizationObject, AuthObj>().AfterMap((s,d)=> {
                    d.ObjID = s.ID;
                    d.ObjCode = s.ObjectCode;
                    d.ObjName = s.ObjectName;
                    d.ObjType = s.ObjectType;
                });
                cfg.CreateMap<Menu, MenuTree>().AfterMap((s,d)=> {
                    d.MenuID = s.ID;                    
                    if (s.AuthorizationObjects != null && s.AuthorizationObjects.Count() > 0)
                    {
                        d.ChildrenAuthObj = s.AuthorizationObjects.ToList().MapTo<List<AuthObj>>();
                    }
                    //if (s.Children != null && s.Children.Count() > 0)
                    //{
                    //    d.ChildrenMenus = s.Children.ToList().MapTo<List<MenuTree>>();
                    //}
                });
                cfg.CreateMap<NewMenu, Menu>().AfterMap((s,d)=> {
                    d.MenuCode = s.MenuCode;
                    d.MenuName = s.MenuName.ToJson();
                    var utcNow = DateTime.UtcNow;
                    d.CreateTime = utcNow;
                    d.ModifyTime = utcNow;
                    d.ParentID = s.ParentID;
                    d.SystemID = s.SystemID;
                    d.SortID = s.SortID;                    
                });
                cfg.CreateMap<NewObj, AuthorizationObject>().AfterMap((s,d)=> 
                {
                    d.MenuId = s.MenuID;
                    d.ObjectCode = s.ObjCode;
                    d.ObjectName = s.ObjName.ToJson();
                    d.ObjectType = (ObjectType)(int)s.ObjType;
                    var utcNow = DateTime.UtcNow;
                    d.CreateTime = utcNow;
                    d.ModifyTime = utcNow;                    
                });

                cfg.CreateMap<NewRole, Role>().AfterMap((s,d)=> 
                {
                    var utcNow = DateTime.UtcNow;
                    d.CreateTime = utcNow;
                    d.ModifyTime = utcNow;
                });
                cfg.CreateMap<RoleRightInfo, RoleRight>().ForMember(d=>d.RoleID,s=>s.Ignore()).AfterMap((s, d) => 
                {
                    if (s.IsMenu)
                    {
                        d.MenuID = s.RightID;
                    }
                    else
                    {
                        d.AuthorizationObjectID = s.RightID;
                    }
                    d.PermissionValue = s.PermissionValue;
                    var utcNow = DateTime.UtcNow;
                    d.CreateTime = utcNow;
                    d.ModifyTime = utcNow;
                });

                cfg.CreateMap<RoleRight,RightInfo>();
                cfg.CreateMap<Role, RoleAndRightInfo>().AfterMap((s,d)=>
                {
                    d.RoleRightInfos = s.RoleRights.MapTo<IEnumerable<RightInfo>>().ToList();
                });

                cfg.CreateMap<NewAppSystem,AppSystem>().AfterMap((s,d)=> {
                    var utcNow = DateTime.UtcNow;
                    d.CreateTime = utcNow;
                    d.ModifyTime = utcNow;
                });
                cfg.CreateMap<Operation, OperationLog>();
            });
        }
    }
}
