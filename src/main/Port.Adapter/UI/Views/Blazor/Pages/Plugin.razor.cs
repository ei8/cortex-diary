// #define static
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Pages
{
    public partial class Plugin
    {
        [Parameter]
        public string Name { get; set; }

        private Type componentType;
        private Dictionary<string, object> parameters;
        protected override void OnInitialized()
        {
            var defParams = new DefaultComponentParameters()
            {
                HttpContextAccessor = this.HttpContextAccessor, 
                NeuronQueryService = this.NeuronQueryService, 
                NeuronApplicationService = this.NeuronApplicationService, 
                TerminalApplicationService = this.TerminalApplicationService, 
                ToastService = this.ToastService, 
                NavigationManager = this.NavigationManager, 
                JsRuntime = this.JsRuntime, 
                SettingsService = this.SettingsService, 
                IdentityService = this.IdentityService, 
                SubscriptionApplicationService = this.SubscriptionApplicationService, 
                SubscriptionsQueryService = this.SubscriptionsQueryService
            };
            this.parameters = defParams.GetParameterDictionary();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var targetUrl = $"/_content/{this.componentType.Assembly.GetName().Name}/script.js";
                await this.JsRuntime.InvokeVoidAsync("loadJs", targetUrl);
            }
            // return base.OnAfterRenderAsync(firstRender);
        }

        protected override void OnParametersSet()
        {
#if (static)
            // To debug a plugin
            // 1. Uncomment #define static on line 1 of this file
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
            this.componentType = typeof(Diary.Plugins.Tree.Tree);
#else
        //scan assembly from a folder
        var path = this.SettingsService.PluginsPath;
        string[] allfiles = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

        foreach (var file in allfiles)
        {
            var assembly = Assembly.LoadFrom(file);
            var fullname = assembly.GetTypes().FirstOrDefault(x => x.Name.ToLower() == Name.ToLower())?.FullName;
            if (fullname != null)
                this.componentType = assembly.GetType(fullname);
        }
#endif
            base.OnParametersSet();
        }
    }
}