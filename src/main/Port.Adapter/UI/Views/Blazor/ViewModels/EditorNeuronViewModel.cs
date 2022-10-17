using ei8.Cortex.Diary.Port.Adapter.Common;
using ei8.Cortex.Library.Common;
using neurUL.Cortex.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.ViewModels
{
    public class EditorNeuronViewModel
    {
        public const string BaseRegionTag = "[Base]";
        public EditorNeuronViewModel() => this.Initialize();
        public string Id { get; set; }
        public string Tag { get; set; }
        public int Version { get; set; }
        public string RegionId { get; set; }
        public string RegionTag { get; set; }
        public string InitialRegionId { get; set; } = string.Empty;
        public string InitialRegionTag { get; set; } = EditorNeuronViewModel.BaseRegionTag;
        public IEnumerable<Neuron> InitialPostsynaptics { get; set; } = null;
        public string NeuronExternalReferenceUrl { get; set; }
        public IReadOnlyList<EditorTerminalViewModel> Terminals { get; set; }

        public void Initialize()
        {
            this.Id = string.Empty;
            this.Tag = string.Empty;
            this.Version = 0;
            this.InitializeRegion();
            this.NeuronExternalReferenceUrl = string.Empty;
            
            this.Terminals = new EditorTerminalViewModel[0];
            this.InitializePostsynaptic();
        }

        public void InitializeRegion()
        {
            this.RegionId = this.InitialRegionId;
            this.RegionTag = this.InitialRegionTag;
        }

        public void InitializePostsynaptic()
        {
            if (this.InitialPostsynaptics != null)
                this.Terminals = this.InitialPostsynaptics.Select(ip => new EditorTerminalViewModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Neuron = new Neuron(ip),
                        Type = RelativeType.Postsynaptic,
                        Effect = NeurotransmitterEffect.Excite,
                        Strength = 1f
                    }
                ).ToArray();
        }

        public void ClearInitialRegion()
        {
            this.InitialRegionId = null;
            this.InitialRegionTag = EditorNeuronViewModel.BaseRegionTag;
        }
    }
}
