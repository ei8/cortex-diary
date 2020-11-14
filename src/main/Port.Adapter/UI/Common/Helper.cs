using ei8.Cortex.Diary.Common;
using ei8.Cortex.Library.Client.Out;
using ei8.Cortex.Library.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Common
{
    public class Helper
    {
        private const string TypeNamePrefix = "neurUL.Cortex.Domain.Model.Neurons.";

        public static NotificationData CreateNotificationData(Diary.Common.Notification notification, IDictionary<string, UINeuron> cache)
        {
            var d = DateTime.Parse(notification.Timestamp);
            var timestamp = $"{d.ToShortDateString()} {d.ToShortTimeString()}";
            var id = notification.Id;
            var type = notification.TypeName.Substring(TypeNamePrefix.Length, notification.TypeName.IndexOf(',') - TypeNamePrefix.Length);
            var expectedVersion = 0;
            if (type.ToUpper().Contains("NEURON") && cache.ContainsKey(id))
                expectedVersion = cache[id].Version;

            var tag = string.Empty;
            if (type == EventTypeNames.TerminalCreated.ToString())
            {
                dynamic dy = JsonConvert.DeserializeObject(notification.Data);
                tag = $"{SafeGetTag(dy.PresynapticNeuronId.ToString(), cache)} > { SafeGetTag(dy.PostsynapticNeuronId.ToString(), cache)}";
            }
            else if (type != EventTypeNames.TerminalDeactivated.ToString())
                tag = SafeGetTag(id, cache);

            var details = string.Empty;
            dynamic data = JsonConvert.DeserializeObject(notification.Data);
            if (type == EventTypeNames.NeuronCreated.ToString())
            {
                string region =
                    string.IsNullOrEmpty(data.RegionId) ?
                        "Base Region" :
                        !string.IsNullOrEmpty(data.RegionId) && cache.ContainsKey(data.RegionId.ToString()) ?
                            cache[data.RegionId.ToString()].Tag :
                            "(Region not found)"
                            ;
                details = $"Neuron created in region '{region}'.";
            }
            else if (type == EventTypeNames.NeuronTagChanged.ToString())
            {
                details = $"Neuron Tag changed to '{data.Tag}'.";
            }
            else if (type == EventTypeNames.TerminalCreated.ToString())
            {
                var presynapticTag = cache.ContainsKey(data.PresynapticNeuronId.ToString()) ?
                    cache[data.PresynapticNeuronId.ToString()].Tag :
                    "Missing: " + data.PresynapticNeuronId.ToString();

                var postsynaptictag = cache.ContainsKey(data.PostsynapticNeuronId.ToString()) ?
                    cache[data.PostsynapticNeuronId.ToString()].Tag :
                    "Missing: " + data.PostsynapticNeuronId.ToString();

                details = $"Terminal created between Presynaptic Neuron '{presynapticTag}' and Postsynaptic Neuron '{postsynaptictag}'.";
            }

            return new NotificationData(
                timestamp,
                notification.AuthorId,
                cache.ContainsKey(notification.AuthorId) ? cache[notification.AuthorId].Tag : notification.AuthorId,
                notification.TypeName,
                type,
                notification.Version,
                expectedVersion,
                id,
                tag,
                notification.Data,
                details
                );
        }

        public static async Task<IEnumerable<NotificationData>> UpdateCacheGetNotifications(NotificationLog notificationLog, INeuronQueryClient neuronGraphQueryClient, string avatarUrl, IDictionary<string, UINeuron> cache)
        {
            var ids = new List<string>();
            var ns = notificationLog.NotificationList.ToList();
            ns.ForEach(n =>
            {
                ids.Add(n.AuthorId);
                ids.Add(n.Id);
                dynamic d = JsonConvert.DeserializeObject(n.Data);
                // NeuronCreated
                if (n.TypeName.Contains(EventTypeNames.NeuronCreated.ToString()))
                {
                    // RegionId   
                    if (d.RegionId != null)
                        ids.Add(d.RegionId.ToString());
                }
                // TerminalCreated
                else if (n.TypeName.Contains(EventTypeNames.TerminalCreated.ToString()))
                {
                    // PresynapticNeuronId
                    ids.Add(d.PresynapticNeuronId.ToString());
                    // PostsynapticNeuronId
                    ids.Add(d.PostsynapticNeuronId.ToString());
                }
            }
            );
            ids.RemoveAll(i => cache.ContainsKey(i));
            ids = new List<string>(ids.Distinct());

            if (ids.Count() > 0)
                (await neuronGraphQueryClient.GetNeurons(avatarUrl, neuronQuery: new NeuronQuery() { Id = ids.ToArray() }))
                    .Neurons
                    .ToList()
                    .ForEach(n => cache.Add(n.Id, new UINeuron(n))
                );

            return notificationLog.NotificationList.ToArray().Select(n => Common.Helper.CreateNotificationData(n, cache));
        }

        private static string SafeGetTag(string id, IDictionary<string, UINeuron> cache)
        {
            return cache.ContainsKey(id) ?
                                    cache[id].Tag :
                                    $"Neuron with Id '{id}' not found.";
        }
    }
}
