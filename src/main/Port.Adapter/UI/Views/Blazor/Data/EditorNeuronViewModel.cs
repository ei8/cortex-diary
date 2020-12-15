using ei8.Cortex.Library.Common;
using neurUL.Cortex.Common;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Data
{
    public class EditorNeuronViewModel
    {
        public EditorNeuronViewModel() => this.Initialize();

        public string Id { get; set; }
        public string Tag { get; set; }
        public int Version { get; set; }
        public RelativeType? RelativeType { get; set; }
        public NeurotransmitterEffect? Effect { get; set; }
        public float? Strength { get; set; }

        public void Initialize()
        {
            this.Id = string.Empty;
            this.Tag = string.Empty;
            this.Version = 0;
            this.RelativeType = null;
            this.Effect = null;
            this.Strength = null;
        }
    }
}
