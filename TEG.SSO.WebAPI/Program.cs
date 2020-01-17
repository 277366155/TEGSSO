using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using TEG.SSO.Common;
using TEG.SSO.Service;

namespace TEG.SSO.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AutoMapperHelper.Config();
            CreateWebHostBuilder(args)
                .Build()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(BaseCore.Configuration)
                .UseStartup<Startup>();
    }
}
