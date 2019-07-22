using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OAuth.Data;

namespace OAuth
{
    public class Startup
    {
        public IHostingEnvironment Env { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            // AspNetIdentity配置
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(connectionString));

            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // 配置IdentityServer4 
            // 结合EntityFramework添加用户管理
            /* NuGet
             * IdentityServer4
             * IdentityServer4.AspNetIdentity
             * IdentityServer4.EntityFramework
             * Microsoft.EntityFrameworkCore.SqlServer
             * Microsoft.EntityFrameworkCore.Tools
             * */
            var builder = services.AddIdentityServer()
                .AddAspNetIdentity<AppUser>()     // AspNetIdentity账号
                //.AddTestUsers(Config.GetUsers())  -- 测试环境账号
                // 添加(clients, resources )配置到数据库
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // 添加操作(codes, tokens, consents )数据到数据库
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                      b.UseSqlServer(connectionString,
                          sql => sql.MigrationsAssembly(migrationsAssembly));

                    // 设置动态清除token
                    options.EnableTokenCleanup = true;
                });
            
            // 设置属性
            builder.AddProfileService<ProfileService<AppUser>>();
            if (Env.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                // windows和linux目录兼容判断
                var path = "";
                var cerdDir = Configuration["Certificates:CerdDir"];
                var cerName = Configuration["Certificates:CerName"];
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    path = cerdDir + "\\" + cerName;
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    path = cerdDir + "/" + cerName;
                }
                var cerFile = Path.Combine(Env.ContentRootPath, path);
                builder.AddSigningCredential(new System.Security.Cryptography.X509Certificates.X509Certificate2(
                    cerFile, Configuration["Certificates:Password"])
                );
            }

   
            // 认证配置
            services.AddAuthentication()
                // 第三方Github Google qq  ...
                //.AddGoogle
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.SaveTokens = true;

                    options.Authority = Environment.GetEnvironmentVariable("OAUTH_ISSUER");
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "client";
                    options.GetClaimsFromUserInfoEndpoint = true;
                });
         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseCors("CorsPolicy");
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

   
    }
}
