using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RoLaMoDS.Data;
using RoLaMoDS.Models;
using RoLaMoDS.Services;
using RoLaMoDS.Services.Interfaces;

namespace RoLaMoDS
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

            services.AddDbContext<ApplicationDBContext>(opt =>
                opt.UseSqlServer(Configuration.GetConnectionString("Windows")));
            services.AddIdentity<UserModel, IdentityRole<Guid>>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 6;
                opt.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
<<<<<<< HEAD
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(opt =>
                {
                    opt.Events.OnRedirectToLogin = (context) =>
                        {
                            context.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        };
                });
=======
            services.AddAuthentication("CookieAuth" )
                .AddCookie("CookieAuth")
                .AddGoogle("GoogleAuth", googleOptions =>
            {
                googleOptions.ClientId = Configuration["Google:Authentication:Client_Id"];
                googleOptions.ClientSecret = Configuration["Google:Authentication:Client_Secret"];
            });
>>>>>>> 9d198b4b1633309de920499864efac7e3f9b23a2

            services.AddTransient<IImageWorkerService, ImageWorkerService>();
            services.AddTransient<IMainControllerService, MainControllerSevice>();
            services.AddSingleton<IImageValidator, ImageValidator>();
            services.AddSingleton<IFileService, FileService>();
            services.AddLogging();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IFileService fileService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            fileService.CreateImagePathes();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Main}/{action=Index}/{id?}");
            });
        }
    }
}
