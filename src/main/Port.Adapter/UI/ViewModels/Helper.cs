using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Application.Notifications;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public static class Helper
    {
        public static string CleanForJSON(string s)
        {
            if (s == null || s.Length == 0)
            {
                return "";
            }

            char c = '\0';
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            String t;

            for (i = 0; i < len; i += 1)
            {
                c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            t = "000" + String.Format("X", c);
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        internal async static Task<Neuron> CreateNeuron(Func<Task<string>> tagRetriever, object owner, IDialogService dialogService, INeuronQueryService neuronQueryService, INeuronApplicationService neuronApplicationService, INotificationApplicationService notificationApplicationService, IStatusService statusService, string avatarUrl, string layerId)
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
                            Id = Guid.NewGuid().GetHashCode(),
                            NeuronId = Guid.NewGuid().ToString(),
                            Version = 1,
                        };

                        await neuronApplicationService.CreateNeuron(
                            avatarUrl,
                            n.NeuronId,
                            ViewModels.Helper.CleanForJSON(n.Tag),
                            layerId
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

        internal async static Task<bool> CreateRelative(Func<Task<string>> tagRetriever, Func<object, Task<string[]>> terminalParametersRetriever, object owner, IDialogService dialogService, INeuronQueryService neuronQueryService, INeuronApplicationService neuronApplicationService, ITerminalApplicationService terminalApplicationService, IStatusService statusService, string avatarUrl, string layerId, string targetNeuronId, RelativeType relativeType)
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
                        ViewModels.Helper.CleanForJSON(tag),
                        layerId
                    );
                    await terminalApplicationService.CreateTerminal(
                        avatarUrl,
                        Guid.NewGuid().ToString(),
                        presynapticNeuronId,
                        postsynapticNeuronId,
                        (NeurotransmitterEffect)int.Parse(tps[0]),
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
                            relativeType == RelativeType.Presynaptic ? n.NeuronId : targetNeuronId,
                            relativeType == RelativeType.Presynaptic ? targetNeuronId : n.NeuronId,
                            (NeurotransmitterEffect)int.Parse(tps[0]),
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
