using ei8.Cortex.Diary.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels
{
    public class ViewViewModel
    {
        private View view;
        public ViewViewModel(View view)
        {
            this.view = view;
        }

        public string Url => this.view.Url;

        public string ParentUrl => this.view.ParentUrl;

        public string Name => this.view.Name;

        public bool IsDefault => this.view.IsDefault;

        public int Sequence => this.view.Sequence;

        public string Icon => this.view.Icon;
    }
}
