using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using neurUL.Common.Domain.Model;
using neurUL.Common.Http;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common
{
    public static class ExtensionMethods
    {
        public static Dictionary<string, object> GetParameterDictionary<T>(this T value)
            where T : IDefaultComponentParameters
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            foreach (var property in value.GetType().GetProperties())
                parameters.Add(property.Name, property.GetValue(value));
            return parameters;
        }

        private static void ShowFriendlyException(IToastService toastService, string message, string description, string clipboardData)
        {
            toastService.ShowError(
                    new RenderFragment(builder => {
                        builder.AddContent(1, message);
                        builder.OpenElement(2, "br");
                        builder.CloseElement();
                        builder.OpenElement(3, "br");
                        builder.CloseElement();

                        int x = 4;
                        if (!string.IsNullOrWhiteSpace(description))
                        {
                            builder.OpenElement(x++, "i");
                            builder.AddContent(x++, description);
                            builder.CloseElement();
                            builder.OpenElement(x++, "br");
                            builder.CloseElement();
                            builder.OpenElement(x++, "br");
                            builder.CloseElement();
                        }

                        builder.OpenElement(x++, "button");
                        builder.AddAttribute(x++, "onclick", $"copyToClipboard('{System.Text.Json.JsonEncodedText.Encode(clipboardData)}');");
                        builder.AddContent(x++, "Copy to clipboard");
                        builder.CloseElement();
                    }
                    )
                    );
        }

        public static async Task UITryHandler(
            this IToastService toastService,
            Func<Task<bool>> tryActionInvoker,
            Func<string> processDescriptionGenerator,
            Action preActionInvoker = null,
            Action postActionInvoker = null
            )
        {
            AssertionConcern.AssertArgumentNotNull(tryActionInvoker, nameof(tryActionInvoker));
            AssertionConcern.AssertArgumentNotNull(toastService, nameof(toastService));
            AssertionConcern.AssertArgumentNotNull(processDescriptionGenerator, nameof(processDescriptionGenerator));

            if (preActionInvoker != null)
                preActionInvoker.Invoke();

            try
            {
                if (await tryActionInvoker.Invoke())
                    toastService.ShowSuccess($"{processDescriptionGenerator.Invoke()} successful.");
            }
            catch (Exception ex)
            {
                string errorDescription, clipboardData;
                if (!ExtensionMethods.TryGetHttpRequestExceptionExDetalis(ex, out errorDescription, out clipboardData))
                {
                    errorDescription = ex.Message;
                    clipboardData = ex.ToString();
                }

                ExtensionMethods.ShowFriendlyException(
                    toastService,
                    $"{processDescriptionGenerator.Invoke()} failed:",
                    errorDescription,
                    clipboardData
                );
            }
            finally
            {
                if (postActionInvoker != null)
                    postActionInvoker.Invoke();
            }
        }

        private static bool TryGetHttpRequestExceptionExDetalis(Exception exception, out string description, out string clipboardData)
        {
            bool result = false;
            description = string.Empty;
            clipboardData = string.Empty;
            if (exception is HttpRequestExceptionEx)
            {
                var m = System.Text.RegularExpressions.Regex.Match(exception.Message, @"Description\"":\""(?<Description>(?!\"",\""Type\"").*)\"",\""Type\""");
                if (m.Success)
                    description = m.Groups["Description"].Value;
                else
                {
                    var m2 = System.Text.RegularExpressions.Regex.Match(exception.Message, @"detail\"":\""(?<Detail>(?!\n).*)\n");
                    if (m2.Success)
                    {
                        var detail = m2.Groups["Detail"].Value;
                        // get first line only
                        // TODO: use regex only instead of substring + indexof
                        description = detail.Substring(0, detail.IndexOf("\\n"));
                    }
                }
                clipboardData = exception.Message;
                result = true;
            }
            return result;
        }

        public static void LoadConfig(this IPluginSettingsService value)
        {
            var thisAssembly = value.GetType().Assembly;
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Path.GetDirectoryName(thisAssembly.Location))
                .AddJsonFile($"{thisAssembly.GetName().Name}.appsettings.json",
                optional: false,
                reloadOnChange: true);

            value.Configuration = configurationBuilder.Build();
        }
    }
}
