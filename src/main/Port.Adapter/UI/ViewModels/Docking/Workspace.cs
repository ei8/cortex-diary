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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.IO;
using ReactiveUI;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;
using Splat;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking
{
    public class Workspace : ReactiveObject
    {
        private readonly ReactiveCommand newNeuronGraph;

        public Workspace()
        {
            this.newNeuronGraph = ReactiveCommand.Create(() =>
            {
                this.panes.Add(new NeuronGraphPaneViewModel(Locator.Current.GetService<INeuronService>()));
                this.ActiveDocument = this.panes.Last();
            });
        }

        ObservableCollection<PaneViewModel> panes = new ObservableCollection<PaneViewModel>();
        ReadOnlyObservableCollection<PaneViewModel> _readonlyPanes = null;
        public ReadOnlyObservableCollection<PaneViewModel> Panes
        {
            get
            {
                if (_readonlyPanes == null)
                    _readonlyPanes = new ReadOnlyObservableCollection<PaneViewModel>(panes);

                return _readonlyPanes;
            }
        }

        ToolViewModel[] _tools = null;

        public IEnumerable<ToolViewModel> Tools
        {
            get
            {
                if (_tools == null)
                    _tools = new ToolViewModel[] { this.FileStats, this.PropertyGrid };
                return _tools;
            }
        }

        private NeuronGraphStatsViewModel _fileStats = null;
        private NeuronGraphStatsViewModel FileStats
        {
            get
            {
                if (_fileStats == null)
                    _fileStats = new NeuronGraphStatsViewModel();

                return _fileStats;
            }
        }

        private PropertyGridViewModel propertyGrid = null;
        public PropertyGridViewModel PropertyGrid
        {
            get
            {
                if (this.propertyGrid == null)
                    this.propertyGrid = new PropertyGridViewModel();

                return this.propertyGrid;
            }
        }

        // TODO: #region OpenCommand
        // RelayCommand _openCommand = null;
        //public ICommand OpenCommand
        //{
        //    get
        //    {
        //        if (_openCommand == null)
        //        {
        //            _openCommand = new RelayCommand((p) => OnOpen(p), (p) => CanOpen(p));
        //        }

        //        return _openCommand;
        //    }
        //}

        //private bool CanOpen(object parameter)
        //{
        //    return true;
        //}

        //private void OnOpen(object parameter)
        //{
        //    var dlg = new OpenFileDialog();
        //    if (dlg.ShowDialog().GetValueOrDefault())
        //    {
        //        var fileViewModel = Open(dlg.FileName);
        //        ActiveDocument = fileViewModel;
        //    }
        //}

        //public FileViewModel Open(string filepath)
        //{
        //    var fileViewModel = _files.FirstOrDefault(fm => fm.FilePath == filepath);
        //    if (fileViewModel != null)
        //        return fileViewModel;

        //    fileViewModel = new FileViewModel(filepath);
        //    _files.Add(fileViewModel);
        //    return fileViewModel;
        //}

        //#endregion 

        public ReactiveCommand NewNeuronGraph => this.newNeuronGraph;

        #region ActiveDocument

        private PaneViewModel _activeDocument = null;
        public PaneViewModel ActiveDocument
        {
            get { return _activeDocument; }
            set
            {
                this.RaiseAndSetIfChanged(ref this._activeDocument, value);
                this.ActiveDocumentChanged?.Invoke(this, EventArgs.Empty);                
            }
        }

        public event EventHandler ActiveDocumentChanged;

        #endregion


        // TODO: internal void Close(FileViewModel fileToClose)
        //{
        //    if (fileToClose.IsDirty)
        //    {
        //        var res = MessageBox.Show(string.Format("Save changes for file '{0}'?", fileToClose.FileName), "AvalonDock Test App", MessageBoxButton.YesNoCancel);
        //        if (res == MessageBoxResult.Cancel)
        //            return;
        //        if (res == MessageBoxResult.Yes)
        //        {
        //            Save(fileToClose);
        //        }
        //    }

        //    _files.Remove(fileToClose);
        //}

        //internal void Save(FileViewModel fileToSave, bool saveAsFlag = false)
        //{
        //    if (fileToSave.FilePath == null || saveAsFlag)
        //    {
        //        var dlg = new SaveFileDialog();
        //        if (dlg.ShowDialog().GetValueOrDefault())
        //            fileToSave.FilePath = dlg.SafeFileName;
        //    }

        //    File.WriteAllText(fileToSave.FilePath, fileToSave.TextContent);
        //    ActiveDocument.IsDirty = false;
        //}
    }
}
