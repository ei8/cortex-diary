using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class AuthorInfo
    {
        [ReadOnly(true)]
        public string Id { get; set; }
        [ReadOnly(true)]
        public string Name { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
