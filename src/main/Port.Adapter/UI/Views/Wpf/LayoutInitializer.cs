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

using System.Linq;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;
using Xceed.Wpf.AvalonDock.Layout;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public class LayoutInitializer : ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            //AD wants to add the anchorable into destinationContainer
            //just for test provide a new anchorablepane 
            //if the pane is floating let the manager go ahead
            LayoutAnchorablePane destPane = destinationContainer as LayoutAnchorablePane;
            if (destinationContainer != null &&
                destinationContainer.FindParent<LayoutFloatingWindow>() != null)
                return false;

            var destinationPane = string.Empty;
            bool autoHide = false;
            anchorableToShow.AutoHideWidth = 400;
            anchorableToShow.AutoHideHeight = 300;

            if (anchorableToShow.Content is NeuronGraphViewModel)
                destinationPane = "TopToolsPane";
            else if (anchorableToShow.Content is EditorToolViewModel)
                destinationPane = "DocumentBottomPane";
            else if (anchorableToShow.Content is ServerExplorerToolViewModel)
            {
                destinationPane = "LeftToolsPane";
                autoHide = true;
            }
            else
                destinationPane = "BottomToolsPane";

            var success = AddAnchorableToPane(destinationPane, layout, anchorableToShow);
            if (autoHide)
                anchorableToShow.ToggleAutoHide();

            return success;
        }

        private static bool AddAnchorableToPane(string paneName, LayoutRoot layout, LayoutAnchorable anchorableToShow)
        {
            var toolsPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == paneName);
            if (toolsPane != null)
            {
                toolsPane.Children.Add(anchorableToShow);
                return true;
            }
            return false;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
        }


        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {

        }
    }
}
