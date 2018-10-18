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
using Splat;
using System;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels.Neurons
{
    public class NeuronGraphStatsViewModel : ToolViewModel
    {
        public NeuronGraphStatsViewModel()
            : base("Graph Stats")
        {
            ContentId = ToolContentId;

            Locator.CurrentMutable.GetService<Workspace>().WhenAnyValue(x => x.ActiveDocument)
              .Subscribe(x =>
              {
                  var y = x as NeuronGraphPaneViewModel;
                  if (y != null)
                      this.FileSize = y.NeuronViewModels.Count;
                  else
                      this.FileSize = 0;
              });
            // TODO: BitmapImage bi = new BitmapImage();
            //bi.BeginInit();
            //bi.UriSource = new Uri("pack://application:,,/Images/property-blue.png");
            //bi.EndInit();
            //IconSource = bi;
        }

        public const string ToolContentId = "FileStatsTool";

        #region FileSize

        private long _fileSize;
        public long FileSize
        {
            get { return _fileSize; }
            set {  this.RaiseAndSetIfChanged(ref this._fileSize, value); }
        }

        #endregion

        #region LastModified

        private DateTime _lastModified;
        public DateTime LastModified
        {
            get { return _lastModified; }
            set { this.RaiseAndSetIfChanged(ref this._lastModified, value); }
        }

        #endregion
    }
}
