using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Blazor.Data;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.RequestProvider;
using works.ei8.Cortex.Diary.Application.Identity;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Application.Dependency;

namespace Blazor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            var ssi = new SettingsServiceImplementation();
            var dp = new DependencyService(ssi);
            var ss = new SettingsService(dp);
            var rp = new RequestProvider();

            services.AddSingleton<IDependencyService>(dp);            
            services.AddSingleton<ISettingsService>(ss);            
            services.AddSingleton<IRequestProvider>(rp);
            services.AddSingleton<IIdentityService>(new IdentityService(ss, rp));            
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
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
