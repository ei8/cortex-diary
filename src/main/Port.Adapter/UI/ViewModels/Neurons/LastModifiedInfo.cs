using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class LastModifiedInfo
    {
        [ReadOnly(true)]
        public ModificationInfo Neuron { get; set; }

        [ReadOnly(true)]
        public ModificationInfo Terminal { get; set; }

        public override string ToString()
        {
            return this.Neuron.ToString();
        }
    }
}
