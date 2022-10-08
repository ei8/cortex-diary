using Blazored.Toast.Services;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Notifications;
using ei8.Cortex.Diary.Port.Adapter.Common;
using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.ViewModels;
using ei8.Cortex.Library.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using neurUL.Common.Domain.Model;
using neurUL.Common.Http;
using neurUL.Cortex.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public static class Helper
    {
        internal static void ReinitializeOption(Action<ContextMenuOption> optionSetter)
        {
            optionSetter(ContextMenuOption.NotSet);
            optionSetter(ContextMenuOption.New);
        }
        
        public static async Task AddLink(ITerminalApplicationService terminalApplicationService, string avatarUrl, string sourceNeuronID, string targetNeuronID)
        {
            NeurotransmitterEffect effect = NeurotransmitterEffect.Excite;
            await terminalApplicationService.CreateTerminal(avatarUrl,
                Guid.NewGuid().ToString(),
                sourceNeuronID,
                targetNeuronID,
                effect,
                1f,
                string.Empty
                );
        }

        public static async Task CreateTerminalFromViewModel(EditorTerminalViewModel value, string targetNeuronId, ITerminalApplicationService terminalApplicationService, string avatarUrl)
        {
            var presynapticNeuronId = string.Empty;
            var postsynapticNeuronId = string.Empty;

            if (value.Type == RelativeType.Postsynaptic)
            {
                presynapticNeuronId = targetNeuronId;
                postsynapticNeuronId = value.Neuron.Id;
            }
            else if (value.Type == RelativeType.Presynaptic)
            {
                presynapticNeuronId = value.Neuron.Id;
                postsynapticNeuronId = targetNeuronId;
            }

            await terminalApplicationService.CreateTerminal(
                avatarUrl,
                value.Id,
                presynapticNeuronId,
                postsynapticNeuronId,
                value.Effect.Value,
                value.Strength.Value,
                value.TerminalExternalReferenceUrl
                );
        }

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

        public static bool TryGetHttpRequestExceptionExDetalis(Exception exception, out string description, out string clipboardData)
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

        public static void ShowFriendlyException(IToastService toastService, string message, string description, string clipboardData)
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

        internal static async Task UITryHandler(
            Func<Task<bool>> tryActionInvoker, 
            IToastService toastService,
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
                if (!Blazor.Helper.TryGetHttpRequestExceptionExDetalis(ex, out errorDescription, out clipboardData))
                {
                    errorDescription = ex.Message;
                    clipboardData = ex.ToString();
                }

                Blazor.Helper.ShowFriendlyException(
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

        // TODO: internal async static Task<bool> ChangeNeuronTag(string tag, INeuronApplicationService neuronApplicationService, IStatusService statusService, string avatarUrl, string targetNeuronId, int expectedVersion, string bearerToken)
        //{
        //    bool result = false;

        //    await Neurons.Helper.SetStatusOnComplete(async () =>
        //    {
        //        await neuronApplicationService.ChangeNeuronTag(
        //                avatarUrl,
        //                targetNeuronId,
        //                tag,
        //                expectedVersion,
        //                bearerToken
        //            );

        //        return result = true;
        //    },
        //        "Neuron Tag changed successfully.",
        //        statusService
        //    );
        //    return result;
        //}
    }
}
