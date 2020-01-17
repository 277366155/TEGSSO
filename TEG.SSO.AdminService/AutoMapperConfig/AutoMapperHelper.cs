using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using TEG.SSO.Entity.DBModel;
using TEG.SSO.Entity.DTO;

namespace TEG.SSO.AdminService
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
                cfg.CreateMap<Operation, OperationLog>();
            });
        }
    }
}
