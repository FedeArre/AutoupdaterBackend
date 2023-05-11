using DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC.BackgroundTasks;
using MVC.DiscordBot;
using Objects;
using Objects.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC
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
            var versions = new GameVersioningHandler();
            services.AddControllersWithViews();

            services.AddSession();

            services.AddScoped<IModRepository, ModRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEarlyAccessRepository, EarlyAccessRepository>();

            services.AddSingleton(versions);

            services.AddDbContext<AutoupdaterContext>(options =>
            {
                options.UseMySQL(Configuration.GetConnectionString("Default"));
                
            });

            services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = 5001;
            });

            services.AddSingleton<IHostedService, TelemetryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IModRepository mods)
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

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseAuthentication();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AutoupdaterContext>();

                TelemetryHandler.GetInstance().Services = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
                BotHandler.GetInstance().Services = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
                context.Database.Migrate();
            }

            BotHandler.GetInstance().Configuration = Configuration;
            BotHandler.GetInstance().MainAsync();
        }
    }
}
