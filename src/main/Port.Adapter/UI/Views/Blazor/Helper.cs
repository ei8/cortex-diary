using Blazored.Toast.Services;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Notifications;
using ei8.Cortex.Diary.Port.Adapter.Common;
using ei8.Cortex.Library.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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

        internal static async Task CreateRelativeCore(INeuronApplicationService neuronApplicationService, ITerminalApplicationService terminalApplicationService, string avatarUrl, string regionId, string targetNeuronId, RelativeType relativeType, string tag, NeurotransmitterEffect effect, float strength, string neuronExternalReferenceUrl, string terminalExternalReferenceUrl)
        {
            var presynapticNeuronId = string.Empty;
            var postsynapticNeuronId = string.Empty;
            var newNeuronId = string.Empty;

            if (relativeType == RelativeType.Presynaptic)
            {
                newNeuronId = presynapticNeuronId = Guid.NewGuid().ToString();
                postsynapticNeuronId = targetNeuronId;
            }
            else if (relativeType == RelativeType.Postsynaptic)
            {
                presynapticNeuronId = targetNeuronId;
                newNeuronId = postsynapticNeuronId = Guid.NewGuid().ToString();
            }

            await neuronApplicationService.CreateNeuron(
                avatarUrl,
                newNeuronId,
                tag,
                regionId,
                neuronExternalReferenceUrl
            );
            await terminalApplicationService.CreateTerminal(
                avatarUrl,
                Guid.NewGuid().ToString(),
                presynapticNeuronId,
                postsynapticNeuronId,
                effect,
                strength,
                terminalExternalReferenceUrl
                );
        }
        
        public static async Task AddLink(ITerminalApplicationService terminalApplicationService, string avatarUrl, string SourceNeuronID,string TargetNeuronID)
        {
            NeurotransmitterEffect effect = NeurotransmitterEffect.Excite;
            await terminalApplicationService.CreateTerminal(avatarUrl,
                Guid.NewGuid().ToString(),
                SourceNeuronID,
                TargetNeuronID,
                effect,
                1f,
                ""
                );
        }
        public static async Task LinkRelativeCore(ITerminalApplicationService terminalApplicationService, string avatarUrl, string targetNeuronId, RelativeType relativeType, IEnumerable<Neuron> candidates, NeurotransmitterEffect effect, float strength, string terminalExternalReferenceUrl)
        {
            foreach (var n in candidates)
            {
                await terminalApplicationService.CreateTerminal(
                    avatarUrl,
                    Guid.NewGuid().ToString(),
                    relativeType == RelativeType.Presynaptic ? n.Id : targetNeuronId,
                    relativeType == RelativeType.Presynaptic ? targetNeuronId : n.Id,
                    effect,
                    strength,
                    terminalExternalReferenceUrl
                    );
            }
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
                if(m.Success)
                    description = m.Groups["Description"].Value;
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
