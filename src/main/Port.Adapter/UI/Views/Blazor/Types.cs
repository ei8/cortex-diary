using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public enum ContextMenuOption
    {
        NotSet,
        New,
        Edit,
        Delete,
        AddRelative
    }

    public enum RenderDirectionValue
    {
        TopToBottom,
        BottomToTop
    }

    public enum ExpansionState
    {
        Collapsed,
        Expanding,
        Expanded
    }

    public enum NavBarType
    {
        Side,
        Top
    }
}
