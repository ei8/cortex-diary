using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Notifications;
using ei8.Cortex.Diary.Common;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public static class Helper
    {
        internal static void FillUIIds(this IEnumerable<Neuron> neurons, Neuron central)
        {
            neurons.ToList().ForEach(un =>
            {
                un.UIId = Guid.NewGuid().GetHashCode();
                un.CentralUIId = central != null ? central.UIId : int.MinValue;
            });
        }

        internal async static Task<Neuron> CreateNeuron(Func<Task<string>> tagRetriever, object owner, IDialogService dialogService, INeuronQueryService neuronQueryService, INeuronApplicationService neuronApplicationService, INotificationApplicationService notificationApplicationService, IStatusService statusService, string avatarUrl, string regionId)
        {
            Neuron result = null;
            await Neurons.Helper.SetStatusOnComplete(async () =>
            {
                bool stat = false;
                bool addingOwner = false;
                var shouldAddOwner = (await notificationApplicationService.GetNotificationLog(avatarUrl, string.Empty)).NotificationList.Count == 0;
                if (shouldAddOwner && (await dialogService.ShowDialogYesNo("This Avatar needs to be initialized with an Owner Neuron. Do you wish to continue by creating one?", owner, out DialogResult yesno)).GetValueOrDefault())
                    addingOwner = true;

                if (!shouldAddOwner || addingOwner)
                {
                    string tag = await tagRetriever();
                    if (!string.IsNullOrEmpty(tag) &&
                        await Neurons.Helper.PromptSimilarExists(neuronQueryService, dialogService, avatarUrl, owner, tag)
                    )
                    {
                        Neuron n = new Neuron
                        {
                            Tag = tag,
                            UIId = Guid.NewGuid().GetHashCode(),
                            Id = Guid.NewGuid().ToString(),
                            Version = 1,
                        };

                        await neuronApplicationService.CreateNeuron(
                            avatarUrl,
                            n.Id,
                            n.Tag,
                            regionId
                            );
                        result = n;
                        stat = true;
                    }
                }
                return stat;
            },
                "Neuron created successfully.",
                statusService,
                "Neuron creation cancelled."
            );
            return result;
        }

        internal async static Task<bool> CreateRelative(Func<Task<string>> tagRetriever, Func<object, Task<string[]>> terminalParametersRetriever, object owner, IDialogService dialogService, INeuronQueryService neuronQueryService, INeuronApplicationService neuronApplicationService, ITerminalApplicationService terminalApplicationService, IStatusService statusService, string avatarUrl, string regionId, string targetNeuronId, RelativeType relativeType)
        {
            bool result = false;
            await Neurons.Helper.SetStatusOnComplete(async () =>
            {
                bool stat = false;
                var tag = await tagRetriever();

                if (!string.IsNullOrEmpty(tag) &&
                    await Neurons.Helper.PromptSimilarExists(neuronQueryService, dialogService, avatarUrl, owner, tag))
                {
                    string[] tps = await terminalParametersRetriever(owner);
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
                        regionId
                    );
                    await terminalApplicationService.CreateTerminal(
                        avatarUrl,
                        Guid.NewGuid().ToString(),
                        presynapticNeuronId,
                        postsynapticNeuronId,
                        (neurUL.Cortex.Common.NeurotransmitterEffect)int.Parse(tps[0]),
                        float.Parse(tps[1])
                        );
                    result = true;
                    stat = true;
                }
                return stat;
            },
                $"{relativeType.ToString()} created successfully.",
                statusService,
                $"{relativeType.ToString()} creation cancelled."
            );
            return result;
        }

        internal async static Task<bool> LinkRelative(Func<Task<IEnumerable<Neuron>>> linkCandidatesRetriever, Func<object, Task<string[]>> terminalParametersRetriever, object owner, ITerminalApplicationService terminalApplicationService, IStatusService statusService, string avatarUrl, string targetNeuronId, RelativeType relativeType)
        {
            bool result = false;
            await Neurons.Helper.SetStatusOnComplete(async () =>
            {
                bool stat = false;
                var candidates = await linkCandidatesRetriever();

                if (candidates != null && candidates.Count() > 0)
                {
                    string[] tps = await terminalParametersRetriever(owner);

                    foreach (var n in candidates)
                    {
                        await terminalApplicationService.CreateTerminal(
                            avatarUrl,
                            Guid.NewGuid().ToString(),
                            relativeType == RelativeType.Presynaptic ? n.Id : targetNeuronId,
                            relativeType == RelativeType.Presynaptic ? targetNeuronId : n.Id,
                            (neurUL.Cortex.Common.NeurotransmitterEffect)int.Parse(tps[0]),
                            float.Parse(tps[1])
                            );
                    }
                    result = true;
                    stat = true;
                }
                                
                return stat;
            },
                $"{relativeType.ToString()} linked successfully.",
                statusService,
                $"{relativeType.ToString()} linking cancelled."
            );
            return result;
        }

        internal async static Task<bool> ChangeNeuronTag(string tag, INeuronApplicationService neuronApplicationService, IStatusService statusService, string avatarUrl, string targetNeuronId, int expectedVersion)
        {
            bool result = false;

            await Neurons.Helper.SetStatusOnComplete(async () =>
                {
                    await neuronApplicationService.ChangeNeuronTag(
                            avatarUrl,
                            targetNeuronId,
                            tag,
                            expectedVersion
                        );

                    return result = true;
                },
                "Neuron Tag changed successfully.",
                statusService
            );
            return result;
        }
    }
}
