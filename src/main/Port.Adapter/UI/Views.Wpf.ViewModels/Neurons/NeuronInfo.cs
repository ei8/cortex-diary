using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class NeuronInfo
    {
        public NeuronInfo(Library.Common.NeuronInfo original)
        {
            if (original != null)
            {
                this.Id = original.Id;
                this.Tag = string.IsNullOrEmpty(original.Tag) ? "[Base]" : original.Tag;
            }
        }

        [ReadOnly(true)]
        public string Id { get; set; }
        [ReadOnly(true)]
        public string Tag { get; set; }

        public override string ToString()
        {
            return this.Tag;
        }
    }
}
