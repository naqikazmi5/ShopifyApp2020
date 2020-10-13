using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ShopifyApp.Models.ServiceHelper;

namespace ShopifyApp
{
    public class Startup
    {

        Timer t;
        private static Timer timer;
        //private readonly IConfiguration _configuration;
        //public Startup(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //    t = new Timer(SinkStockLevel);
        //    t.Change(0, 900000);
        //}
        private async void SyncOrder(object state)
        {
          await new ServiceHandler().SyncStoreOrders();
        }
        private async void SyncData(object state)
        {
            await new ServiceHandler().SyncStoreData();
        }
        private async void InitData()
        {
            var DailyTime = "22:00:00";
            var timeParts = DailyTime.Split(new char[1] { ':' });
            var dateNow = DateTime.Now;
            var date = new DateTime(dateNow.Year, dateNow.Month, dateNow.Day,
            int.Parse(timeParts[0]), int.Parse(timeParts[1]), int.Parse(timeParts[2]));
            DateTime date1 = date;
            DateTime date2 = DateTime.Now;
            TimeSpan interval = date1.Subtract(date2);
            int hoursDiff = interval.Hours;
            int minutes = interval.Minutes;
            int minutesTotal = Convert.ToInt32(interval.TotalMinutes);
            int interval2 = minutesTotal * 60000;
            var timerState = new TimerState { Counter = 0 };
            timer = new Timer(
                callback: new TimerCallback(SyncData),
                state: timerState,
                dueTime: interval2,
                period: 86400000);
        }
        public Startup(IConfiguration configuration)
        {
            //Task.Run(() => InitData());
            Configuration = configuration;
            t = new Timer(SyncOrder);
            t.Change(0, 900000);
            var builder = new ConfigurationBuilder()
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
        services.AddControllersWithViews()
       .AddSessionStateTempDataProvider();
            services.AddRazorPages()
                .AddSessionStateTempDataProvider();
            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
            });
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //services.AddControllersWithViews().AddRazorRuntimeCompilation();;
            services.Configure<MyConfig>(Configuration.GetSection("MyConfig"));
            services.AddCors(options => options.AddPolicy("ApiCorsPolicy", builder =>
            {
                builder.WithOrigins("https://logisticsolutionctkusa.azurewebsites.net").AllowAnyMethod().AllowAnyHeader();
            }));
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            //app.UseCors(builder => builder
            //           .AllowAnyOrigin()
            //           .AllowAnyMethod()
            //           .AllowAnyHeader()
            //           .AllowCredentials());
            app.UseCors("ApiCorsPolicy");
            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
