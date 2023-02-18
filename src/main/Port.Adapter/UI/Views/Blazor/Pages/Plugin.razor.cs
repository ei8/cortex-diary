using Blazored.Toast.Services;
using ei8.Cortex.Diary.Application.Access;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Application.Subscriptions;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
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
        [Inject]
        public IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject]
        public INeuronQueryService NeuronQueryService { get; set; }
        [Inject]
        public INeuronApplicationService NeuronApplicationService { get; set; }
        [Inject]
        public ITerminalApplicationService TerminalApplicationService { get; set; }
        [Inject]
        public IToastService ToastService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public ISettingsService SettingsService { get; set; }
        [Inject]
        public IIdentityService IdentityService { get; set; }
        [Inject]
        public ISubscriptionApplicationService SubscriptionApplicationService { get; set; }
        [Inject]
        public ISubscriptionQueryService SubscriptionsQueryService { get; set; }
        [Inject]
        public IList<Assembly> PluginAssemblies { get; set; }
        [Inject]
        public IAccessApplicationService AccessApplicationService { get; set; }

        [Parameter]
        public string Name { get; set; }

        private Type componentType;
        private Dictionary<string, object> parameters;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.JsRuntime.LoadJsCssAsync($"_plugin/{this.Name}");
            }
        }

        protected override void OnParametersSet()
        {
            foreach (var assembly in this.PluginAssemblies)
            {
                var fullname = assembly.GetTypes().FirstOrDefault(x => x.Name.ToLower() == Name.ToLower())?.FullName;
                if (fullname != null)
                {
                    this.componentType = assembly.GetType(fullname);

                    var pluginSettingsName = assembly.GetTypes().FirstOrDefault(x => x.GetInterfaces().Contains(typeof(IPluginSettingsService)))?.FullName;
                    Type pluginSettingsType = null;
                    if (pluginSettingsName != null)
                        pluginSettingsType = assembly.GetType(pluginSettingsName);

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
                        SubscriptionsQueryService = this.SubscriptionsQueryService,
                        AccessApplicationService = this.AccessApplicationService
                    };

                    if (pluginSettingsType != null)
                    {
                        defParams.PluginSettingsService =  (IPluginSettingsService) Activator.CreateInstance(pluginSettingsType);
                        defParams.PluginSettingsService.LoadConfig();
                    }

                    this.parameters = defParams.GetParameterDictionary();

                    break;
                }
            }
            base.OnParametersSet();
        }
    }
}