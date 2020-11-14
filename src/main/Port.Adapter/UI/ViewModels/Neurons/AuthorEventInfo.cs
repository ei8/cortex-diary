using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AuthorEventInfo
    {
        public AuthorEventInfo(Library.Common.AuthorEventInfo original)
        {
            if (original != null)
            {
                this.Author = new NeuronInfo(original.Author);
                this.Timestamp = original.Timestamp;
            }
        }

        [ReadOnly(true)]
        public NeuronInfo Author { get; set; }
        [ReadOnly(true)]
        public string Timestamp { get; set; }

        public override string ToString()
        {
            return this.Author != null && this.Timestamp != null && !string.IsNullOrEmpty(this.Author.Tag) ? 
                $"{this.Author.Tag}, {this.Timestamp}" : 
                string.Empty;
        }
    }
}
