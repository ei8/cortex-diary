// #define staticLinkAssembly
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
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Services;
using ei8.Cortex.Library.Client.Out;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using neurUL.Common.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;

/// <summary>
/// To be used with Razor class libraries already referenced by this project
/// </summary>
void ConfigureStaticLibraries(ApplicationPartManager partManager)
{
    var assembly = typeof(ProcessInfo).Assembly;
    var applicationPart = new AssemblyPart(assembly);

    partManager.ApplicationParts.Add(applicationPart);
}

/// <summary>
/// To be used with Razor class libraries loaded dynamically
/// </summary>
void LoadDynamicLibraries(ApplicationPartManager partManager, string binFolder, IList<Assembly> pluginsAssemblies)
{
#if (staticLinkAssembly)
    // To debug a plugin:
    // 1. Uncomment #define staticLinkAssembly on line 1 of this file
    // 2. Add project reference to plugin project
    // 3. Change startup project to Blazor.csproj
    // 4. Grab values from var1.env (docker-compose)
    //     "environmentVariables": {
    //       "ASPNETCORE_ENVIRONMENT": "Development",
    //       "OIDC_AUTHORITY": "",
    //       "CLIENT_ID": "",
    //       "CLIENT_SECRET": "",
    //       "UPDATE_CHECK_INTERVAL": "1000000",
    //       "DATABASE_PATH": "",
    //       "BASE_PATH": "",
    //       "PLUGINS_PATH": "",
    //       "VALIDATE_SERVER_CERTIFICATE": "false",
    //       "APP_TITLE": "",
    //       "APP_ICON": ""
    //     },
    //     "applicationUrl": "" - use value from docker-compose.override.yml
    StaticAddAssembly(partManager, pluginsAssemblies, typeof(ei8.Cortex.Diary.Plugins.Tree.Tree).Assembly);
#else
    // get the full filepath of any dll starting with the rcl_ prefix
    string prefix = string.Empty; 
    string searchPattern = $"{prefix}*.dll";
    string[] libraryPaths = Directory.GetFiles(binFolder, searchPattern, SearchOption.AllDirectories);

    if (libraryPaths != null && libraryPaths.Length > 0)
    {
        // create the load context
        var loadContext = new LibraryLoadContext(binFolder);

        foreach (string libraryPath in libraryPaths)
        {
            // load each assembly using its filepath
            var assembly = loadContext.LoadFromAssemblyPath(libraryPath);

            AddAssembly(
                partManager, 
                pluginsAssemblies, 
                assembly,
                assembly => libraryPath.EndsWith(".Views.dll") ? new CompiledRazorAssemblyPart(assembly) : new AssemblyPart(assembly),
                () => !libraryPath.EndsWith(".Views.dll")
                );
        }
    }
#endif
}

static void StaticAddAssembly(ApplicationPartManager partManager, IList<Assembly> pluginsAssemblies, Assembly assembly) =>
    AddAssembly(partManager, pluginsAssemblies, assembly, (a) => new AssemblyPart(a), () => (true));

static void AddAssembly(ApplicationPartManager partManager, IList<Assembly> pluginsAssemblies, Assembly assembly, 
    Func<Assembly, ApplicationPart> partCreator,
    Func<bool> addChecker)
{
    // create an application part for that assembly
    var applicationPart = partCreator(assembly); 

    // register the application part
    partManager.ApplicationParts.Add(applicationPart);

    // if it is NOT the *.Views.dll add it to a list for later use
    if (addChecker())
        pluginsAssemblies.Add(assembly);
}

/// <summary>
/// Registers a <see cref="CompositeFileProvider"/> for each dynamically loaded assembly.
/// </summary>
void RegisterDynamicLibariesStaticFiles(IWebHostEnvironment env, IList<Assembly> pluginsAssemblies)
{
    foreach (var a in pluginsAssemblies)
    {
        // TODO: See https://stackoverflow.com/a/74985201
        // create a "web root" file provider for the embedded static files found on wwwroot folder
        var fileProvider = new ManifestEmbeddedFileProvider(a, "wwwroot");

        // register a new composite provider containing
        // the old web root file provider
        // and the new one we just created
        env.WebRootFileProvider = new CompositeFileProvider(env.WebRootFileProvider, fileProvider);
    }
}

var builder = WebApplication.CreateBuilder(args);

IdentityModelEventSource.ShowPII = true;

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredToast();
builder.Services
    .AddBlazorise(o =>
    {
        o.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ISettingsServiceImplementation, SettingsServiceImplementation>();
builder.Services.AddScoped<IDependencyService, DependencyService>();

var ss = new SettingsService(
    new DependencyService(
        new SettingsServiceImplementation()
        )
    );
builder.Services.AddSingleton<ISettingsService>(ss);
var pluginsAssemblies = new List<Assembly>();
builder.Services.AddSingleton<IList<Assembly>>(pluginsAssemblies);
builder.Services.AddControllersWithViews()
    .ConfigureApplicationPartManager(partManager => {
        // static RCLs
        ConfigureStaticLibraries(partManager);
        // dynamic RCLs
        LoadDynamicLibraries(partManager, ss.PluginsPath, pluginsAssemblies);
    });

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
// TODO: Add other receiver info type registrations for subscription client once implemented
builder.Services.AddScoped<ISubscriptionClient, HttpSubscriptionClient>();
builder.Services.AddScoped<ISubscriptionApplicationService, SubscriptionApplicationService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
builder.Services.AddScoped<ISubscriptionConfigurationClient, HttpSubscriptionConfigurationClient>();
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
    context.Request.PathBase = PathString.FromUriComponent(ss.BasePath ?? string.Empty);// prefix.ToString());
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

// register file providers for the dynamically loaded libraries
if (pluginsAssemblies.Count > 0)
    RegisterDynamicLibariesStaticFiles(app.Environment, pluginsAssemblies);

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});

app.Run();