using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using TEG.SSO.AdminService;
using TEG.SSO.AdminService.Base;
using TEG.SSO.AdminWeb.Filter;
using TEG.SSO.Common;
using TEG.SSO.LogDBContext;

namespace TEG.SSO.AdminWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            AutoMapperHelper.Config();
            Configuration = configuration;          
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<LogContext>(opt=> opt.UseSqlServer(BaseCore.Configuration.GetConnectionString("LogConn")));
            services.AddScoped<AccountService>();
            services.AddScoped<SecurityQuestionService>();
            services.AddScoped<LogService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc(option =>
            {
                option.Filters.Add(typeof(GlobalExceptionFilter));               
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }            
            DbContextSeed.DbInit(svp);
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Default}/{action=Login}");
                routes.MapRoute("home", "{controller=home}/{action=main}");
            });
        }
    }
}
