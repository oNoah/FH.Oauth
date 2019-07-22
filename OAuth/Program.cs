using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OAuth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var seed = args.Any(x => x == "/seed");
            if (seed) args = args.Except(new[] { "/seed" }).ToArray();

            var host = CreateWebHostBuilder(args).Build();

            // 初始化种子数据 只有第一次启动项中配置/seed的时候 才初始化
            if (seed)
            {
                using (var scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
                {
                    SeedData.EnsureSeedData(scope.ServiceProvider);
                    return;
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                // 认证中心默认5000端口，客户端5010,其他API资源不限制端口，默认80 容器启动的时候指定端口或者随机分配
                .ConfigureKestrel(options =>
                {
                    options.ListenAnyIP(5000);
                })
                .UseStartup<Startup>();
    }
}
