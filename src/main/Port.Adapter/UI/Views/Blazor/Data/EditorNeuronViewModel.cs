using ei8.Cortex.Library.Common;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Data
{
    public class EditorNeuronViewModel
    {
        public EditorNeuronViewModel() => this.Initialize();

        public string Id { get; set; }
        public string Tag { get; set; }
        public int Version { get; set; }
        public RelativeType? RelativeType { get; set; }

        public void Initialize()
        {
            this.Id = string.Empty;
            this.Tag = string.Empty;
            this.Version = 0;
        }
    }
}
