using ei8.Cortex.Diary.Domain.Model;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public static class Extensions
    {
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

        public static string GenerateBarActiveClass(this View value, NavigationManager navigationManager) =>
            value.Url == navigationManager.Uri ? "active hover" : "hover";

        public static void Navigate(this View value, NavigationManager navigationManager) =>
            navigationManager.NavigateTo(value.Url, true);
    }
}
