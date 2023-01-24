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

using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// TODO: using System.Windows.Media;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking
{
    public class PaneViewModel : ReactiveObject
    {
        public PaneViewModel()
        { }


        #region Title

        private string _title = null;
        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }

        #endregion

        public string IconSourcePath
        {
            get;
            protected set;
        }

        #region ContentId

        private string _contentId = null;
        public string ContentId
        {
            get { return _contentId; }
            set { this.RaiseAndSetIfChanged(ref this._contentId, value); }
        }

        #endregion

        #region IsSelected

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { this.RaiseAndSetIfChanged(ref this._isSelected, value); }
        }
        #endregion

        #region IsActive

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { this.RaiseAndSetIfChanged(ref this._isActive, value); }
        }

        #endregion
    }
}
