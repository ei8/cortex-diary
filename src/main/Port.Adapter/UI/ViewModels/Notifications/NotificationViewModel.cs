using Newtonsoft.Json;
using org.neurul.Common.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Notifications
{
    public class NotificationViewModel : ReactiveObject
    {
        private readonly Notification notification;
        private readonly IDictionary<string, Neuron> neuronCache;
        private const string TypeNamePrefix = "org.neurul.Cortex.Domain.Model.Neurons.";

        public NotificationViewModel(Notification notification, IDictionary<string, Neuron> neuronCache)
        {
            this.notification = notification;
            this.neuronCache = neuronCache;
        }

        public string Timestamp
        {
            get
            {
                var d = DateTime.Parse(this.notification.Timestamp);
                return $"{d.ToShortDateString()} {d.ToShortTimeString()}";
            }
        }

        public string AuthorId => this.notification.AuthorId;

        public string Author => this.neuronCache.ContainsKey(this.notification.AuthorId) ? this.neuronCache[this.notification.AuthorId].Tag : this.notification.AuthorId;

        public string TypeName => this.notification.TypeName;

        public string Type => this.notification.TypeName.Substring(NotificationViewModel.TypeNamePrefix.Length, this.notification.TypeName.IndexOf(',') - NotificationViewModel.TypeNamePrefix.Length);

        public int Version => this.notification.Version;

        public string Id => this.notification.Id;

        public string Tag
        {
            get
            {
                var result = string.Empty;

                if (this.Type == EventTypeNames.TerminalCreated.ToString())
                {
                    dynamic data = JsonConvert.DeserializeObject(this.notification.Data);
                    result = $"{this.SafeGetTag(data.PresynapticNeuronId.ToString())} > { this.SafeGetTag(data.PostsynapticNeuronId.ToString())}";
                }
                else if (this.Type != EventTypeNames.TerminalDeactivated.ToString())
                {
                    result = this.SafeGetTag(this.notification.Id);
                }

                return result;
            }
        }

        private string SafeGetTag(string id)
        {
            return this.neuronCache.ContainsKey(id) ?
                                    this.neuronCache[id].Tag :
                                    $"Neuron with Id '{id}' not found.";
        }

        public string Data => this.notification.Data;

        public string Details
        {
            get
            {
                string result = string.Empty;
                dynamic data = JsonConvert.DeserializeObject(this.notification.Data);
                if (this.Type == EventTypeNames.NeuronCreated.ToString())
                {
                    string layer = 
                        data.LayerId == Guid.Empty.ToString() ? 
                            "Base Layer" :
                            this.neuronCache.ContainsKey(data.LayerId.ToString()) ?
                                this.neuronCache[data.LayerId.ToString()].Tag :
                                "(Layer not found)"
                                ;
                    result = $"Neuron created in layer '{layer}'.";
                }
                else if (this.Type == EventTypeNames.NeuronTagChanged.ToString())
                {
                    result = $"Neuron Tag changed to '{data.Tag}'.";
                }
                else if (this.Type == EventTypeNames.TerminalCreated.ToString())
                {
                    result = $"Terminal created between Presynaptic Neuron '{this.neuronCache[data.PresynapticNeuronId.ToString()].Tag}' and Postsynaptic Neuron '{this.neuronCache[data.PostsynapticNeuronId.ToString()].Tag}'.";
                }
                return result;
            }
        }
    }
}
