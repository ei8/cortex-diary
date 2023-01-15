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
                await this.JsRuntime.LoadJsCssAsync($"_plugin/{this.Name}");
            }
        }

        protected override void OnParametersSet()
        {
            //scan assembly from a folder
            foreach (var assembly in this.PluginAssemblies)
            {
                var fullname = assembly.GetTypes().FirstOrDefault(x => x.Name.ToLower() == Name.ToLower())?.FullName;
                if (fullname != null)
                    this.componentType = assembly.GetType(fullname);
            }
            base.OnParametersSet();
        }
    }
}