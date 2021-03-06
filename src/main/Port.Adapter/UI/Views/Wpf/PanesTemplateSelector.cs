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

using System.Windows;
using System.Windows.Controls;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Notifications;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;
using Xceed.Wpf.AvalonDock.Layout;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {

        }
        
        public DataTemplate NeuronTreeViewTemplate
        {
            get;
            set;
        }

        public DataTemplate ServerExplorerToolViewTemplate
        {
            get;
            set;
        }

        public DataTemplate PropertyGridViewTemplate
        {
            get;
            set;
        }

        public DataTemplate NeuronGraphViewTemplate
        {
            get;
            set;
        }

        public DataTemplate NotificationsPaneViewTemplate
        {
            get;
            set;
        }

        public DataTemplate EditorToolViewTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is NeuronTreePaneViewModel)
                return this.NeuronTreeViewTemplate;

            if (item is ServerExplorerToolViewModel)
                return this.ServerExplorerToolViewTemplate;

            if (item is PropertyGridViewModel)
                return this.PropertyGridViewTemplate;

            if (item is NeuronGraphViewModel)
                return this.NeuronGraphViewTemplate;

            if (item is NotificationsPaneViewModel)
                return this.NotificationsPaneViewTemplate;

            if (item is EditorToolViewModel)
                return this.EditorToolViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
