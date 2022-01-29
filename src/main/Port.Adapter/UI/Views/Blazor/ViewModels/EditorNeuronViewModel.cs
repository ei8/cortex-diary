using ei8.Cortex.Diary.Port.Adapter.Common;
using ei8.Cortex.Library.Common;
using neurUL.Cortex.Common;
using System.Collections.Generic;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.ViewModels
{
    public class EditorNeuronViewModel
    {
        public const string BaseRegionTag = "[Base]";
        public EditorNeuronViewModel() => this.Initialize();
        public string Id { get; set; }
        public string Tag { get; set; }
        public int Version { get; set; }
        public RelativeType? Type { get; set; }
        public NeurotransmitterEffect? Effect { get; set; }
        public float? Strength { get; set; }
        public string RegionId { get; set; }
        public string RegionTag { get; set; }
        public string InitialRegionId { get; set; } = string.Empty;
        public string InitialRegionTag { get; set; } = EditorNeuronViewModel.BaseRegionTag;
        public string NeuronExternalReferenceUrl { get; set; }
        public string TerminalExternalReferenceUrl { get; set; }
        public IReadOnlyList<Neuron> LinkCandidates { get; set; }
        public bool IsRoot { get; set; }
        public ContextMenuOption SelectedOption { get; set; }        

        public void Initialize()
        {
            this.Id = string.Empty;
            this.Tag = string.Empty;
            this.Version = 0;
            this.Type = null;
            this.Effect = null;
            this.Strength = null;
            this.InitializeRegion();
            this.NeuronExternalReferenceUrl = string.Empty;
            this.TerminalExternalReferenceUrl = string.Empty;
            this.LinkCandidates = new Neuron[0];
            this.IsRoot = true;
        }

        public void InitializeRegion()
        {
            this.RegionId = this.InitialRegionId;
            this.RegionTag = this.InitialRegionTag;
        }

        public void ClearInitialRegion()
        {
            this.InitialRegionId = string.Empty;
            this.InitialRegionTag = EditorNeuronViewModel.BaseRegionTag;
        }
    }
}
