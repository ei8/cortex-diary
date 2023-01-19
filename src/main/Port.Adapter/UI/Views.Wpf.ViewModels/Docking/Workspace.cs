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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ei8.Cortex.Diary.Application.Identity;
using ei8.Cortex.Diary.Application.OpenUrl;
using ei8.Cortex.Diary.Application.Settings;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Notifications;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking
{
    public class Workspace : ReactiveObject
    {
        private readonly ReactiveCommand<Unit, Unit> newNeuronTreeCommand;
        private readonly ReactiveCommand<Unit, Unit> newNotificationsCommand;
        private readonly ReactiveCommand<object, Unit> signInCommand;
        private readonly ReactiveCommand<Unit, Unit> openAboutCommand;
        private readonly IOpenUrlService openUrlService;
        private readonly IDialogService dialogService;
        private readonly ISettingsService settingsService;
        private readonly IIdentityService identityService;

        public Workspace(IOpenUrlService openUrlService = null, IDialogService dialogService = null, ISettingsService settingsService = null, IIdentityService identityService = null)
        {
            this.openUrlService = openUrlService ?? Locator.Current.GetService<IOpenUrlService>();
            this.dialogService = dialogService ?? Locator.Current.GetService<IDialogService>();
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
            this.identityService = identityService ?? Locator.Current.GetService<IIdentityService>();

            this.newNeuronTreeCommand = ReactiveCommand.Create(() =>
            {
                this.panes.Add(new NeuronTreePaneViewModel());
                this.ActiveDocument = this.panes.Last();
            });

            this.newNotificationsCommand = ReactiveCommand.Create(() =>
            {
                this.panes.Add(new NotificationsPaneViewModel());
                this.ActiveDocument = this.panes.Last();
            });

            this.signInCommand = ReactiveCommand.Create<object>(async(parameter) =>
                await this.OnSignInClicked(parameter));

            this.openAboutCommand = ReactiveCommand.Create(() => this.openUrlService.OpenUrl("https://github.com/ei8/cortex-diary"));
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
                    _tools = new ToolViewModel[] {
                        this.PropertyGrid,
                        this.NeuronGraph,
                        this.FileStats,
                        this.EditorTool
                    };
                return _tools;
            }
        }

        private ServerExplorerToolViewModel _fileStats = null;
        private ServerExplorerToolViewModel FileStats
        {
            get
            {
                if (_fileStats == null)
                    _fileStats = new ServerExplorerToolViewModel();

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

        private NeuronGraphViewModel neuronGraph = null;
        public NeuronGraphViewModel NeuronGraph
        {
            get
            {
                if (this.neuronGraph == null)
                    this.neuronGraph = new NeuronGraphViewModel();

                return this.neuronGraph;
            }
        }

        private EditorToolViewModel editorTool = null;
        public EditorToolViewModel EditorTool
        {
            get
            {
                if (this.editorTool == null)
                    this.editorTool = new EditorToolViewModel();

                return this.editorTool;
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

        public ReactiveCommand<Unit, Unit> NewNeuronTreeCommand => this.newNeuronTreeCommand;

        public ReactiveCommand<Unit, Unit> NewNotificationsCommand => this.newNotificationsCommand;

        public ReactiveCommand<object, Unit> SignInCommand => this.signInCommand;

        public ReactiveCommand<Unit, Unit> OpenAboutCommand => this.openAboutCommand;

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

        private async Task OnSignInClicked(object parameter)
        {
            if ((await this.dialogService.ShowLogin(this.settingsService, this.openUrlService, this.identityService, parameter, out bool result)).GetValueOrDefault())
            {
                var r = result;
            }
        }
    }
}
