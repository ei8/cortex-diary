using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using works.ei8.Cortex.Diary.Application.Dependency;
using works.ei8.Cortex.Diary.Application.Identity;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Application.Notifications;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Notifications;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Notifications;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.RequestProvider;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Data;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
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
            var nc = new NotificationClient();
            var nas = new NotificationApplicationService(nc);
            var ngqc = new NeuronGraphQueryClient(rp, ss);
            var nec = new NeuronClient(rp, ss);
            var neas = new NeuronApplicationService(nec);

            services.AddSingleton<IDependencyService>(dp);            
            services.AddSingleton<ISettingsService>(ss);            
            services.AddSingleton<IRequestProvider>(rp);
            services.AddSingleton<IIdentityService>(new IdentityService(ss, rp));
            services.AddSingleton<INotificationClient>(nc);
            services.AddSingleton<INotificationApplicationService>(nas);
            services.AddSingleton<INeuronGraphQueryClient>(ngqc);
            services.AddSingleton<INeuronClient>(nec);
            services.AddSingleton<INeuronApplicationService>(neas);
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
