using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrimMinerals.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Store.Data;
using BuisnessLogic;

namespace PrimMinerals
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }




        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();*/

            services.AddHttpContextAccessor();//Http accessor.
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);//Время жизни сессии
                options.Cookie.HttpOnly = true;// к куки файлам есть доступ только у серверных скриптов 
                options.Cookie.IsEssential = true;//Означает что куки использутся только для тех нужд
            });
            services.AddControllersWithViews();
            services.AddRazorPages();


            services.AddSingleton<UserRepository>();
            services.AddSingleton<PaymentRepository>();
            services.AddSingleton<DeliveryRepository>();
            services.AddSingleton<IMessageService, MessageService>();
            services.AddSingleton<OrderService>();
            services.AddSingleton<IOrderRepository, OrderRepositoryMySQL>();
            services.AddSingleton<IOrderRepositoryAdmin, OrderRepositoryMySQL>();
            services.AddSingleton<IProductRepository, ProductRepositoryMySQL>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();//Активация сессии




            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();


                //Точка входа в яндекс
                endpoints.MapAreaControllerRoute(
                    name: "yandex.kassa",
                    areaName: "YandexKassa",
                    pattern: "YandexKassa/{controller=YandexHome}/{action=Index}/{id?}"
                    );

                //Точка входа в яндекс
                endpoints.MapAreaControllerRoute(
                    name: "admin",
                    areaName: "Admin",
                    pattern: "Admin/{controller=AdminHome}/{action=Index}/{id?}"
                    );

            });




        }
    }
}
