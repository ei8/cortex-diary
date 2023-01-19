using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
using ei8.Cortex.Library.Common;
using neurUL.Cortex.Common;
using System;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Common
{
    public static class ExtensionMethods
    {
        public static async Task AddLink(this ITerminalApplicationService terminalApplicationService, string avatarUrl, string sourceNeuronID, string targetNeuronID)
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

        public static async Task CreateTerminalFromViewModel(this ITerminalApplicationService terminalApplicationService, EditorTerminalViewModel value, string targetNeuronId, string avatarUrl)
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
    }
}
