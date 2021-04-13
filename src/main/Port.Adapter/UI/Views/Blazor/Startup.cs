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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Logging;
using System.Net.Http;
using ei8.Cortex.Diary.Port.Adapter.UI.Common;
using System;
using ei8.Cortex.Diary.Common;
using ei8.Cortex.Library.Common;

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
            IdentityModelEventSource.ShowPII = true; 

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredToast();
            services
                .AddBlazorise(o =>
                {
                    o.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            services.AddHttpClient();
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<TokenManager>();

            services.AddScoped<ISettingsServiceImplementation, SettingsServiceImplementation>();
            services.AddScoped<IDependencyService, DependencyService>();
            services.AddScoped<ISettingsService, SettingsService>();
            services.AddScoped<IRequestProvider, RequestProvider>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<INeuronClient, HttpNeuronClient>();
            services.AddScoped<ITerminalClient, HttpTerminalClient>();
            services.AddScoped<INotificationClient, HttpNotificationClient>();
            services.AddScoped<INotificationApplicationService, NotificationApplicationService>();
            services.AddScoped<INeuronApplicationService, NeuronApplicationService>();
            services.AddScoped<ITerminalApplicationService, TerminalApplicationService>();
            services.AddScoped<INeuronQueryClient, HttpNeuronQueryClient>();
            services.AddScoped<INeuronQueryService, NeuronQueryService>();
            
            services.AddScoped<ISignInInfoService, SignInInfoService>((sp) =>
            {
                var siis = new SignInInfoService();
                var anonymousSignIn = new SignInInfo();
                anonymousSignIn.GivenName = "Anonymous";
                anonymousSignIn.FamilyName = "User";
                siis.Add(anonymousSignIn);
                return siis;
            });

            var sp = services.BuildServiceProvider();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                var ss = sp.GetService<ISettingsService>();
                options.Authority = ss.OidcAuthority;                
                options.ClientId = ss.ClientId;
                options.ClientSecret = ss.ClientSecret;
                options.ResponseType = Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectResponseType.Code;
                options.Scope.Add("openid");
                options.Scope.Add("profile");       
                options.Scope.Add("email");
                options.Scope.Add("avatarapi");
                options.CallbackPath = "/Account/LoginCallback";
                options.SignedOutCallbackPath = "/Account/LogoutCallback";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.NameClaimType = "given_name";
                // TODO: REMOVE ONCE CERTIFICATE SORTED
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                options.BackchannelHttpHandler = handler;
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

            // TODO: necessary?
            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.ApplicationServices
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });            
        }
    }
}
