using Blazored.Toast;
using ei8.Cortex.Diary.Application.Dependency;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Notifications;
using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Diary.Nucleus.Client.Out;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.ViewModels;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Services;
using ei8.Cortex.Library.Client.Out;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using neurUL.Common.Http;
using System.ComponentModel.Design;
using Blazorise;
using Blazorise.Icons.FontAwesome;
using Blazorise.Bootstrap;
using ei8.Cortex.Diary.Domain.Model;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
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
            services.AddBlazoredToast();
            services.AddHttpContextAccessor();

            services
                .AddBlazorise(o =>
                {
                    o.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            var ssi = new SettingsServiceImplementation();
            var dp = new Services.DependencyService(ssi);
            var ss = new SettingsService(dp);
            var rp = new RequestProvider();
            var nec = new HttpNeuronClient(rp);
            var tec = new HttpTerminalClient(rp);
            var nc = new HttpNotificationClient(rp);
            var nas = new NotificationApplicationService(nc);
            var neas = new NeuronApplicationService(nec);
            var tas = new TerminalApplicationService(tec);
            var nqc = new HttpNeuronQueryClient(rp);
            var nqs = new NeuronQueryService(nqc);
            var siis = new SignInInfoService();
            var anonymousSignIn = new SignInInfo();
            anonymousSignIn.GivenName = "Anonymous";
            anonymousSignIn.FamilyName = "User";
            siis.Add(anonymousSignIn);

            services.AddSingleton<IDependencyService>(dp);            
            services.AddSingleton<ISettingsService>(ss);            
            services.AddSingleton<IRequestProvider>(rp);
            services.AddSingleton<IIdentityService>(new IdentityService(ss, rp));
            services.AddSingleton<INotificationApplicationService>(nas);
            services.AddSingleton<INeuronClient>(nec);
            services.AddSingleton<INeuronApplicationService>(neas);
            services.AddSingleton<ITerminalApplicationService>(tas);
            services.AddSingleton<INeuronQueryClient>(nqc);
            services.AddSingleton<INeuronQueryService>(nqs);
            services.AddSingleton<ISignInInfoService>(siis);

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                })
                .AddCookie("cookie")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "http://192.168.1.110:61700";
                    options.ClientId = ss.ClientId;
                    options.ClientSecret = ss.ClientSecret;

                    options.ResponseType = "code id_token";
                    options.UsePkce = true;
                    options.ResponseMode = "query";

                    // options.CallbackPath = "/signin-oidc"; // default redirect URI

                    // options.Scope.Add("oidc"); // default scope
                    // options.Scope.Add("profile"); // default scope
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("avatar");
                    options.SaveTokens = true;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePathBase("/d23");

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

            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });            
        }
    }
}
