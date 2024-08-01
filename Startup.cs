using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication28.Areas.Identity.Data;

namespace WebApplication28
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
            services.AddControllersWithViews();
            services.AddSession();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                );
            });


            InitializeAuthorization(usermanager, roleManager).Wait();
        }

        private async Task InitializeAuthorization(UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> roleManager)
        {
            ApplicationUser admin  = await usermanager.FindByNameAsync("admin@gmail.com");
            if ((await usermanager.FindByEmailAsync("admin@gmail.com")) == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true,
                    firstname = "ادمین"
                };
                await usermanager.CreateAsync(admin, "pP=-0987");
            }

            if( await roleManager.RoleExistsAsync("admins") == false)
            {
                IdentityRole adminrole = new IdentityRole("admins");
                await roleManager.CreateAsync(adminrole);
            }
            if (await usermanager.IsInRoleAsync(admin, "admins") == false)
            {
                await usermanager.AddToRoleAsync(admin, "admins");
            }

        }
    }
}
