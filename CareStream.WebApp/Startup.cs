using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using NLog;
using CareStream.Models;
using CareStream.Scheduler;
using System.Reflection;
using Quartz;
using System.Collections.Specialized;
using Quartz.Impl;
using CareStream.Utility;
using CareStream.LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;

namespace CareStream.WebApp
{
    public class Startup
    {
        private IScheduler _scheduler;

        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            CareStreamConst.CareStreamConnectionString = Configuration.GetConnectionString("CareStremConnection");

            services.AddDbContext<CareStreamContext>(options =>
                    options.UseSqlServer(CareStreamConst.CareStreamConnectionString, x => x.MigrationsAssembly(typeof(CareStreamContext).GetTypeInfo().Assembly.GetName().Name)));

            services.AddAuthentication(AzureADB2CDefaults.AuthenticationScheme)
               .AddAzureADB2C(options => Configuration.Bind("AzureAdB2CLogin", options));

            // Update by Venu
            //services.Configure<JwtBearerOptions>(
            //AzureADB2CDefaults.JwtBearerAuthenticationScheme, options =>
            //{
            //    options.Authority = $"{Configuration["AzureAdB2CLogin: Instance"]}/{Configuration["AzureAdB2C:Tenant"]}/{Configuration["AzureAdB2CLogin:SignUpSignInPolicyId"]}/v2.0/";
            //    options.Audience = Configuration["AzureAdB2C:ClientId"];
            //    options.RequireHttpsMetadata = false;
            //    options.Events = new JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = AuthenticationFailed
            //    };
            //});

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupMemberService, GroupMemberService>();
            services.AddScoped<IGroupOwnerService, GroupOwnerService>();
            services.AddScoped<IUserGroupService, UserGroupService>();
            services.AddScoped<IUserAttributeService, UserAttributeService>();
            services.AddSingleton<ILoggerManager, LoggerManager>();
            services.AddSingleton(provider => GetScheduler().Result);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();

            });

            _scheduler = app.ApplicationServices.GetService<IScheduler>();

            SetupAndRunSchedulerJob();
        }

        private async Task<IScheduler> GetScheduler()
        {
            var properties = new NameValueCollection
            {
                { "quartz.scheduler.instanceName", "CareStream" },
                { "quartz.scheduler.instanceId", "CareStream" },
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.useProperties", "true" },
                { "quartz.jobStore.dataSource", "default" },
                { "quartz.jobStore.tablePrefix", "QRTZ_" },
                {
                    "quartz.dataSource.default.connectionString",
                    CareStreamConst.CareStreamConnectionString
                },
                { "quartz.dataSource.default.provider", "SqlServer" },
                { "quartz.threadPool.threadCount", "1" },
                { "quartz.serializer.type", "json" },
            };
            var schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = await schedulerFactory.GetScheduler();
            //await scheduler.Start();
            //update for test
            return scheduler;
        }

        private void SetupAndRunSchedulerJob()
        {
            ITrigger trigger = TriggerBuilder.Create()
             .WithIdentity($"Care Stream Scheduler-{DateTime.Now}")
             .StartNow()
             //.WithSchedule(CronScheduleBuilder.CronSchedule(Configuration["Settings:SchedulerCronExpr"]))
             .WithCronSchedule(Configuration["Settings:SchedulerCronExpr"])
             //.WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
             .WithPriority(1)
             .Build();

            IJobDetail job = JobBuilder.Create<SchedulerJob>()
                        .WithIdentity("CareStreamUniqueIdentity")
                        .Build();

            _scheduler.ScheduleJob(job, trigger);
        }

        private Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            var s = $"AuthenticationFailed: {arg.Exception.Message}";
            arg.Response.ContentLength = s.Length;
            arg.Response.Body.Write(Encoding.UTF8.GetBytes(s), 0, s.Length);
            return Task.FromResult(0);
        }
    }
}
