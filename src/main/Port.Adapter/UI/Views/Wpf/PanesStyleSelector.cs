﻿/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/
// Modifications copyright(C) 2018 ei8/Elmer Bool

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Notifications;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle
        {
            get;
            set;
        }

        public Style NeuronTreeStyle
        {
            get;
            set;
        }

        public Style NotificationsStyle
        {
            get;
            set;
        }

        public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
        {
            if (item is ToolViewModel)
                return ToolStyle;

            if (item is NeuronTreePaneViewModel)
                return NeuronTreeStyle;

            if (item is NotificationsPaneViewModel)
                return NotificationsStyle;

            return base.SelectStyle(item, container);
        }
    }
}
