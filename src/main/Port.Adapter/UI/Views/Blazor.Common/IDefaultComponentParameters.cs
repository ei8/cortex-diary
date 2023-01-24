using Blazored.Toast.Services;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Application.Subscriptions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common
{
    public interface IDefaultComponentParameters
    {
        IHttpContextAccessor HttpContextAccessor { get; set; }
        INeuronQueryService NeuronQueryService { get; set; }
        INeuronApplicationService NeuronApplicationService { get; set; }
        ITerminalApplicationService TerminalApplicationService { get; set; }
        IToastService ToastService { get; set; }
        NavigationManager NavigationManager { get; set; }
        IJSRuntime JsRuntime { get; set; }
        ISettingsService SettingsService { get; set; }
        IIdentityService IdentityService { get; set; }
        ISubscriptionApplicationService SubscriptionApplicationService { get; set; } 
        ISubscriptionQueryService SubscriptionsQueryService { get; set; }
        IPluginSettingsService PluginSettingsService { get; set; }
    }
}
