using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral
{
    public class EditorNeuronViewModel : ReactiveObject
    {
        public EditorNeuronViewModel() => this.Init();

        [Reactive]
        public string Id { get; set; }

        [Reactive]
        public string Tag { get; set; }

        [Reactive]
        public NeurotransmitterEffect? Effect { get; set; }

        [Reactive]
        public string LayerId { get; set; }

        [Reactive]
        public string LayerName { get; set; }

        [Reactive]
        public IEnumerable<Neuron> LinkCandidates { get; set; }

        [Reactive]
        public RelativeType? RelativeType { get; set; }

        [Reactive]
        public float? Strength { get; set; }

        [Reactive]
        public int Version { get; set; }

        internal void Init(bool includeId = true)
        {
            if (includeId)
                this.Id = string.Empty;

            this.Tag = string.Empty;
            this.Effect = null;
            this.Strength = null;
            this.RelativeType = null;
            this.LayerId = string.Empty;
            this.LayerName = string.Empty;
            this.Version = 0;
        }
    }
}
