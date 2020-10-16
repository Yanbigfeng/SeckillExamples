using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SeckillExamples.EData;
using SeckillExamples.ToolsClass;

namespace SeckillExamples
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            //ͨ�������ļ�ʹ��������    
            services.AddDbContext<ESHOPContext>(options =>
                     options.UseSqlServer(_configuration.GetConnectionString("TestEntity")));
            //���ó�ʼ����̨����
            // services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, APIBackgroundService>();
            services.AddHostedService<APIBackgroundService>();

            //ע��Redis����������
            var connectionString = _configuration["RedisStr:connectionString"];
            int defaultDB = Convert.ToInt32(_configuration["RedisStr:defaultDB"]);
            services.AddSingleton(new RedisHelp(connectionString, defaultDB));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context =>
                {
                    //api����ʹ���Զ���·��

                    //���ڵ�ҳ�����
                    await context.Response.WriteAsync("Hello World!");


                    //mvc���ÿ�����·��
                    endpoints.MapControllerRoute(
                             name: "default",
                             pattern: "{controller=Home}/{action=Index}/{id?}");
                });
            });
        }
    }
}
