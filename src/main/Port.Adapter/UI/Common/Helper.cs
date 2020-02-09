using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using works.ei8.Cortex.Graph.Client;
using works.ei8.Cortex.Graph.Common;
using works.ei8.EventSourcing.Common;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Common
{
    public class Helper
    {
        private const string TypeNamePrefix = "org.neurul.Cortex.Domain.Model.Neurons.";

        public static NotificationData CreateNotificationData(Notification notification, IDictionary<string, Neuron> cache)
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
                string layer =
                    data.LayerId == Guid.Empty.ToString() ?
                        "Base Layer" :
                        cache.ContainsKey(data.LayerId.ToString()) ?
                            cache[data.LayerId.ToString()].Tag :
                            "(Layer not found)"
                            ;
                details = $"Neuron created in layer '{layer}'.";
            }
            else if (type == EventTypeNames.NeuronTagChanged.ToString())
            {
                details = $"Neuron Tag changed to '{data.Tag}'.";
            }
            else if (type == EventTypeNames.TerminalCreated.ToString())
            {
                details = $"Terminal created between Presynaptic Neuron '{cache[data.PresynapticNeuronId.ToString()].Tag}' and Postsynaptic Neuron '{cache[data.PostsynapticNeuronId.ToString()].Tag}'.";
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

        public static async Task<IEnumerable<NotificationData>> UpdateCacheGetNotifications(NotificationLog notificationLog, INeuronGraphQueryClient neuronGraphQueryClient, string avatarUrl, IDictionary<string, Neuron> cache)
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
                    // LayerId                    
                    ids.Add(d.LayerId.ToString());
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

            (await neuronGraphQueryClient.GetNeurons(avatarUrl, neuronQuery: new NeuronQuery() { Id = ids.ToArray() }))
                .ToList()
                .ForEach(n => cache.Add(n.Id, n));

            return notificationLog.NotificationList.ToArray().Select(n => Common.Helper.CreateNotificationData(n, cache));
        }

        private static string SafeGetTag(string id, IDictionary<string, Neuron> cache)
        {
            return cache.ContainsKey(id) ?
                                    cache[id].Tag :
                                    $"Neuron with Id '{id}' not found.";
        }
    }
}
