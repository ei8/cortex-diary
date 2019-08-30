using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ModificationInfo
    {
        [ReadOnly(true)]
        public AuthorInfo Author { get; set; }
        [ReadOnly(true)]
        public string Timestamp { get; set; }
        [ReadOnly(true)]
        public int Version { get; set; }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(this.Author.Name) ? $"{this.Author.Name}, {this.Timestamp}" : string.Empty;
        }
    }
}
