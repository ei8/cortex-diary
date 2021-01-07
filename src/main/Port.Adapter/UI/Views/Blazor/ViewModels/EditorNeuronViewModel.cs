using ei8.Cortex.Diary.Port.Adapter.UI.Common;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
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
        public IReadOnlyList<UINeuron> LinkCandidates { get; set; }
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
            this.LinkCandidates = new UINeuron[0];
            this.IsRoot = true;
        }

        public void InitializeRegion()
        {
            this.RegionId = string.Empty;
            this.RegionTag = EditorNeuronViewModel.BaseRegionTag;
        }
    }
}
