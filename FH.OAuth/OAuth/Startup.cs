using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OAuth
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetType().Assembly.GetName().Name;
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
                .AddAspNetIdentity<AppUser>()
                .AddTestUsers(Config.GetUsers())
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

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                var cerFile = Path.Combine(Environment.ContentRootPath, Configuration["Certificates:CerPath"]);
                builder.AddSigningCredential(new System.Security.Cryptography.X509Certificates.X509Certificate2(
                    cerFile, Configuration["Certificates:Password"])
                );
            }

            services.AddAuthentication()
                // 第三方登录
                //.AddGoogle
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.SaveTokens = true;

                    options.Authority = Configuration["Swagger:Issuer"];
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "client";
                    options.GetClaimsFromUserInfoEndpoint = true;
                });
         
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // 添加初始数据
            SeedData.InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

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
