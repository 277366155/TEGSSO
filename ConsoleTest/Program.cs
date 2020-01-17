using ConsoleTest.Filter;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TEG.Framework.Security.SSO;
using TEG.SSO.Common;
using TEG.SSO.EFCoreContext;
using TEG.SSO.Entity.DBModel;

namespace ConsoleTest
{
    [Test(Name = "classPoint")]
    [Test2(Name = "text")]
    class Program
    {
        [Test(Name = "asdf", Age = 20)]
        static void Main(string[] args)
        {
            //ServcieCollectionTest();
            //var token = "fmonOigtV+wmaWJKb38OhrBFelRXrzEbbHyvKyvQvRXlLsCHlwhuNn9W9MPogLBfjab+k4dHP0AWAWGnkfR3KGv2yXh+D1Y732bj9K5zIeiR36mrE+xphWsk7At7R9Ztc5OruUSkoiRzk6u5RKSiJIdRlR6Qw4Fu";
            //PasswordTest();
            TokenTest();

            //  DbContextInit.Test();
            Console.ReadLine();

        }

        [Test(Name = "PasswordTest")]
        static void PasswordTest()
        {
            Console.WriteLine("输入username：");
            var userName = Console.ReadLine();
            Console.WriteLine("输入密码：");
            var pwd = Console.ReadLine();

            var password = SSOHelper.EncryptPassword(pwd, userName);
            Console.WriteLine("密码加密结果：" + password);
            Console.WriteLine("密文解密结果：" + SSOHelper.DecryptPassword(password, userName));
        }

        static void TokenTest()
        {
            var token = SSOHelper.GenerateToken("1", "boo", "boo", "127.0.0.1");
            Console.WriteLine($"token = {token} \r\n");
            List<string> list = null;
            //var result = SSOHelper.IsTokenValid(token, out list);
            var result = SSOHelper.IsTokenValid("abcd", out list);
            Console.WriteLine($"Token Valid Result = {result} ,list = {list.ToJson()}");
        }

        static void ServcieCollectionTest()
        {
            Console.WriteLine("ServcieCollectionTest.....begin.");
            var services = new ServiceCollection();
            services.AddDbContext<BizMasterContext>(options => options.UseSqlServer(BaseCore.Configuration.GetConnectionString("MasterConn")), ServiceLifetime.Scoped);
            var svp = services.BuildServiceProvider();
            using (var scoped = svp.CreateScope())
            {
                var db = scoped.ServiceProvider.GetService<BizMasterContext>();
                db.Database.Migrate();
            }
            Console.WriteLine("ServcieCollectionTest.....end.");
        }
    }
}
