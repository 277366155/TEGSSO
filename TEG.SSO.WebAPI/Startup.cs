using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using TEG.Framework.Standard.Cache;
using TEG.SSO.Common;
using TEG.SSO.EFCoreContext;
using TEG.SSO.Entity.DTO;
using TEG.SSO.Service;
using TEG.SSO.WebAPI.Filter;

namespace TEG.SSO.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AutoMapperHelper.Config();
            //BaseCore.InitConfigurationBuilder();//参数可指定加载其他json配置文件
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<LogContext>();
            services.AddDbContext<LogContext>(option => option.UseSqlServer(BaseCore.Configuration.GetConnectionString("LogConn")), ServiceLifetime.Scoped);
            services.AddDbContext<BizMasterContext>(option => option.UseSqlServer(BaseCore.Configuration.GetConnectionString("MasterConn")), ServiceLifetime.Scoped);
            services.AddDbContext<BizReadOnlyContext>(option => option.UseSqlServer(BaseCore.Configuration.GetConnectionString("ReadOnlyConn")), ServiceLifetime.Scoped);
            services.AddScoped<UserService>();
            services.AddScoped<LogService>();
            services.AddScoped<SecurityQuestionService>();
            services.AddScoped<RedisCache>();
            services.AddScoped<UserInfoAndRoleRight>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc(opt =>
            {
                opt.Filters.Add(typeof(GlobalExceptionFilter));
            });
            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new Info
                {
                    Title = "SSO WebAPI",
                    Version = "v1"
                });
                cfg.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "TEG.SSO.Entity.xml"));
                cfg.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "TEG.SSO.WebAPI.xml"));
                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } } };
                cfg.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                cfg.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey",
                    Description = "身份凭据(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\""
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider svp)
        {
            if (env.IsDevelopment())
            {
              app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            BaseCore.ServiceProvider = svp;
            DbContextInit.DbContextInitAll(svp);
            //app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "default", template: "{controller}/{action=index}");
            });
            app.UseStaticFiles();

            app.UseSwagger(cfg => { cfg.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(cfg =>
            {
                cfg.SwaggerEndpoint("v1/swagger.json", "SSO WebApi");
            });
        }
    }
}
