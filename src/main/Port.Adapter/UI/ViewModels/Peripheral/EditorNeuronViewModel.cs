using org.neurul.Cortex.Common;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using works.ei8.Cortex.Diary.Common;

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
        public string RegionId { get; set; }

        [Reactive]
        public string RegionName { get; set; }

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
            this.RegionId = string.Empty;
            this.RegionName = string.Empty;
            this.Version = 0;
        }
    }
}
