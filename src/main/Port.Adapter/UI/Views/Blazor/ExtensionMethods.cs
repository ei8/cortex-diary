using ei8.Cortex.Diary.Domain.Model;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public static class ExtensionMethods
    {
        public static async Task LoadJsCssAsync(this IJSRuntime jsRuntime, string assetPath)
        {
            await jsRuntime.InvokeVoidAsync("loadJs", $"{assetPath}/script.js");
            await jsRuntime.InvokeVoidAsync("loadCSS", $"{assetPath}/style.css");
        }

        public static bool HasActiveChild(this View value, IEnumerable<View> views, string currentUrl)
        {
            bool result = false;

            var currentView = views.SingleOrDefault(v => v.Url == currentUrl);
            while (currentView != null)
            {
                // get parent of currentview
                currentView = views.SingleOrDefault(v => v.Url == currentView.ParentUrl);

                if (value == currentView)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public static RenderFragment GenerateIconName(this View value) =>
            new RenderFragment(b => b.AddMarkupContent(0, $"<span class=\"oi {value.Icon}\" aria-hidden=\"true\"></span>"));

        public static string GenerateBarActiveClass(this View value, NavigationManager navigationManager, NavBarType navBarType) =>
            value.Url == navigationManager.Uri ? $"active{(navBarType == NavBarType.Top ? "top" : string.Empty)} hover" : "hover";

        public static void Navigate(this View value, NavigationManager navigationManager) =>
            navigationManager.NavigateTo(value.Url, true);

        public static string Truncate(this string value, int maxChars)
        {
            return !string.IsNullOrEmpty(value) ?
                (
                    value.Length <= maxChars ?
                        value :
                        value.Substring(0, maxChars) + "..."
                        )
                    : string.Empty;
        }
    }
}
