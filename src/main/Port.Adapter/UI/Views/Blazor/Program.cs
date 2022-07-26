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
using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Nucleus.Client.In;
using ei8.Cortex.Diary.Nucleus.Client.Out;
using ei8.Cortex.Diary.Port.Adapter.IO.Persistence.SQLite;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Identity;
using ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Services;
using ei8.Cortex.Library.Client.Out;
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

var builder = WebApplication.CreateBuilder(args);


IdentityModelEventSource.ShowPII = true;

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredToast();
builder.Services
    .AddBlazorise(o =>
    {
        o.ChangeTextOnKeyPress = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();


builder.Services.AddScoped<ISettingsServiceImplementation, SettingsServiceImplementation>();
builder.Services.AddScoped<IDependencyService, DependencyService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();

var sp = builder.Services.BuildServiceProvider();
var ss = sp.GetService<ISettingsService>();

var hcb = builder.Services.AddHttpClient(Options.DefaultName);

if (!ss.ValidateServerCertificate)
{
    hcb.ConfigurePrimaryHttpMessageHandler(
        () => new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        }
    );
}

builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<ITokenManager, TokenManager>();
builder.Services.AddScoped<IRequestProvider, RequestProvider>(sp =>
{
    var result = new RequestProvider();

    result.SetHttpClientHandler(
        ss.ValidateServerCertificate ?
            new HttpClientHandler() :
            new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            }
        );

    return result;
});

builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<INeuronClient, HttpNeuronClient>();
builder.Services.AddScoped<ITerminalClient, HttpTerminalClient>();
builder.Services.AddScoped<INotificationClient, HttpNotificationClient>();
builder.Services.AddScoped<INotificationApplicationService, NotificationApplicationService>();
builder.Services.AddScoped<INeuronQueryService, NeuronQueryService>();
builder.Services.AddScoped<INeuronApplicationService, NeuronApplicationService>();
builder.Services.AddScoped<ITerminalApplicationService, TerminalApplicationService>();
builder.Services.AddScoped<INeuronQueryClient, HttpNeuronQueryClient>();
var vas = new ViewApplicationService(new ViewRepository());
builder.Services.AddSingleton<IEnumerable<View>>(vas.GetAll().Result);


builder.Services.AddAuthentication(options =>
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


var app = builder.Build();


app.Use((context, next) =>
{
    // TODO:var prefix = context.Request.Headers["x-forwarded-prefix"];
    //if (!StringValues.IsNullOrEmpty(prefix))
    //{
    context.Request.PathBase = PathString.FromUriComponent(ss.BasePath);// prefix.ToString());
                                                                        // TODO: subtract PathBase from Path if needed.
                                                                        //}
    return next();
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
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

//app.ApplicationServices
//    .UseBootstrapProviders()
//    .UseFontAwesomeIcons();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();