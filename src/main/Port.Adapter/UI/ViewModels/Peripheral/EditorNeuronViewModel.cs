using neurUL.Cortex.Common;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.Generic;
using ei8.Cortex.Diary.Common;
using ei8.Cortex.Library.Common;
using ei8.Cortex.Diary.Port.Adapter.UI.Common;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral
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
        public IEnumerable<UINeuron> LinkCandidates { get; set; }

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
