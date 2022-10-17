using Blazored.Toast;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using ei8.Cortex.Diary.Application;
using ei8.Cortex.Diary.Application.Dependency;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Notifications;
using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Application.Subscriptions;
using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Diary.Nucleus.Client.Out;
using ei8.Cortex.Diary.Port.Adapter.IO.Persistence.SQLite;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Services;
using ei8.Cortex.Library.Client.Out;
using ei8.Cortex.Subscriptions.Common.Receivers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using neurUL.Common.Http;
using System.Collections.Generic;
using System.Net.Http;

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
            services.AddHttpContextAccessor();

            services.AddScoped<ISettingsServiceImplementation, SettingsServiceImplementation>();
            services.AddScoped<IDependencyService, DependencyService>();
            services.AddScoped<ISettingsService, SettingsService>();
            
            var sp = services.BuildServiceProvider();
            var ss = sp.GetService<ISettingsService>();

            var hcb = services.AddHttpClient(Options.DefaultName);

            if (!ss.ValidateServerCertificate)
            {
                hcb.ConfigurePrimaryHttpMessageHandler(
                    () => new HttpClientHandler() { 
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator 
                    }
                );
            }

            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<ITokenManager, TokenManager>();
            services.AddScoped<IRequestProvider, RequestProvider>(sp =>
            {
                var result = new RequestProvider();

                result.SetHttpClientHandler(
                    ss.ValidateServerCertificate ?
                        new HttpClientHandler() :
                        new HttpClientHandler() {
                            ServerCertificateCustomValidationCallback = 
                                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        }
                    );

                return result;
            });
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<INeuronClient, HttpNeuronClient>();
            services.AddScoped<ITerminalClient, HttpTerminalClient>();
            services.AddScoped<INotificationClient, HttpNotificationClient>();
            services.AddScoped<INotificationApplicationService, NotificationApplicationService>();
            services.AddScoped<INeuronQueryService, NeuronQueryService>();
            services.AddScoped<INeuronApplicationService, NeuronApplicationService>();
            services.AddScoped<ITerminalApplicationService, TerminalApplicationService>();
            services.AddScoped<INeuronQueryClient, HttpNeuronQueryClient>();
            // TODO: Add other receiver info type registrations for subscription client once implemented
            services.AddScoped<ISubscriptionClient, HttpSubscriptionClient>();
            services.AddScoped<ISubscriptionApplicationService, SubscriptionApplicationService>();
            services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
            services.AddScoped<ISubscriptionConfigurationClient, HttpSubscriptionConfigurationClient>();

            var vas = new ViewApplicationService(new ViewRepository());
            services.AddSingleton<IEnumerable<View>>(vas.GetAll().Result);
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.Authority = ss.OidcAuthority;                
                options.ClientId = ss.ClientId;
                options.ClientSecret = ss.ClientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.Scope.Add("openid");
                options.Scope.Add("profile");       
                options.Scope.Add("email");
                options.Scope.Add("avatarapi");
                options.Scope.Add("offline_access");
                options.CallbackPath = "/Account/LoginCallback";
                options.SignedOutCallbackPath = "/Account/LogoutCallback";
                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.TokenValidationParameters.NameClaimType = "name";
                options.BackchannelHttpHandler =
                    ss.ValidateServerCertificate ?
                        new HttpClientHandler() :
                        new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                        };
                options.RequireHttpsMetadata = ss.OidcAuthority.ToUpper().StartsWith("HTTPS");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ISettingsService settings)
        {
            app.Use((context, next) =>
            {
                // TODO:var prefix = context.Request.Headers["x-forwarded-prefix"];
                //if (!StringValues.IsNullOrEmpty(prefix))
                //{
                context.Request.PathBase = PathString.FromUriComponent(settings.BasePath);// prefix.ToString());
                    // TODO: subtract PathBase from Path if needed.
                //}
                return next();
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto
            });
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
