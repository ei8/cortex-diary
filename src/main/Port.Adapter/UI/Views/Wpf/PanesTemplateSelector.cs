/************************************************************************

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
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;
using Xceed.Wpf.AvalonDock.Layout;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {

        }
        
        public DataTemplate NeuronGraphViewTemplate
        {
            get;
            set;
        }

        public DataTemplate NeuronGraphStatsViewTemplate
        {
            get;
            set;
        }

        public DataTemplate PropertyGridViewTemplate
        {
            get;
            set;
        }

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as LayoutContent;

            if (item is NeuronGraphPaneViewModel)
                return this.NeuronGraphViewTemplate;

            if (item is NeuronGraphStatsViewModel)
                return this.NeuronGraphStatsViewTemplate;

            if (item is PropertyGridViewModel)
                return this.PropertyGridViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
