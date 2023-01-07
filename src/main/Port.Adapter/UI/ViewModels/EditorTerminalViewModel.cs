using ei8.Cortex.Library.Common;
using neurUL.Cortex.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public class EditorTerminalViewModel
    {
        public EditorTerminalViewModel() => this.Initialize();

        public EditorTerminalViewModel(EditorTerminalViewModel original)
        {
            this.Id = original.Id;
            this.Neuron = new Neuron(original.Neuron);
            this.Type = original.Type;
            this.Effect = original.Effect;
            this.Strength = original.Strength;
            this.TerminalExternalReferenceUrl = original.TerminalExternalReferenceUrl;
            this.Version = original.Version;
        }

        public void Initialize()
        {
            this.Id = string.Empty;
            this.Neuron = null;            
            this.Type = RelativeType.Postsynaptic;
            this.Effect = NeurotransmitterEffect.Excite;
            this.Strength = 1f;
            this.TerminalExternalReferenceUrl = string.Empty;
            this.Version = 0;
        }

        public string Id { get; set; }
        public Neuron Neuron { get; set; }
        public string NeuronTag => this.Neuron != null ? this.Neuron.Tag : string.Empty;
        public RelativeType? Type { get; set; }
        public NeurotransmitterEffect? Effect { get; set; }
        public float? Strength { get; set; }
        public string TerminalExternalReferenceUrl { get; set; }
        public int Version { get; set; }
    }
}
