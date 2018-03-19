using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Controls
{
    public class DynamicEditor : Editor
    {
        public DynamicEditor()
        {
            this.TextChanged += (sender, e) => this.InvalidateMeasure();
        }
    }
}
