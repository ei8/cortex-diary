using Blazored.Toast.Services;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Application.Subscriptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common
{
    public class DefaultComponentParameters : IDefaultComponentParameters
    {
        public IHttpContextAccessor HttpContextAccessor { get; set; }
        public INeuronQueryService NeuronQueryService { get; set; }
        public INeuronApplicationService NeuronApplicationService { get; set; }
        public ITerminalApplicationService TerminalApplicationService { get; set; }
        public IToastService ToastService { get; set; }
        public NavigationManager NavigationManager { get; set; }
        public IJSRuntime JsRuntime { get; set; }
        public ISettingsService SettingsService { get; set; }
        public IIdentityService IdentityService { get; set; }
        public ISubscriptionApplicationService SubscriptionApplicationService { get; set; }
        public ISubscriptionQueryService SubscriptionsQueryService { get; set; }
        public IPluginSettingsService PluginSettingsService { get; set; }
    }
}
