//MIT License

//Copyright(c) .NET Foundation and Contributors

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//
// https://github.com/dotnet-architecture/eShopOnContainers
//
// Modifications copyright(C) 2018 ei8/Elmer Bool

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using works.ei8.Cortex.Diary.Application.Dialog;
using works.ei8.Cortex.Diary.Application.Message;
using works.ei8.Cortex.Diary.Application.Navigation;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Models.Edit;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Models.Navigation;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Models.User;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels.Base;
using Xamarin.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        // TODO: This should be configurable and should be more than the cortex graph update interval, transfer to settings
        private const int GraphWaitInterval = 3000;
        private const string DialogTitle = "d#";

        private IMessageService messageService;
        private INeuronApplicationService neuronApplicationService;
        private INeuronQueryService neuronQueryService;
        private string neuronId;
        private string data;
        private int version;
        private string timestamp;
        private ObservableCollection<Terminal> axon;
        private ObservableCollection<Dendrite> dendrites;
        private bool loaded;
        private bool isAxonVisible;
        private ICommand addExistingDendriteCommand;
        private ICommand addTerminalCommand;
        private ICommand createNewDendriteCommand;
        private ICommand deleteCommand;
        private ICommand newCommand;
        private ICommand loadCommand;
        private ICommand saveCommand;
        private ICommand removeDendriteCommand;
        private ICommand removeTerminalCommand;

        public MainViewModel(
            ISettingsService settingsService, 
            IDialogService dialogService, 
            INavigationService<ViewModelBase> navigationService,
            IMessageService messageService,
            INeuronApplicationService neuronApplicationService,
            INeuronQueryService neuronQueryService
            )
            : base(settingsService, dialogService, navigationService)
        {
            this.messageService = messageService;
            this.neuronApplicationService = neuronApplicationService;
            this.neuronQueryService = neuronQueryService;

            this.Loaded = false;
            this.IsAxonVisible = false;

            this.PropertyChanged += this.MainViewModel_PropertyChanged;

            MessagingCenter.Subscribe<MainViewModel>(this, MessageKeys.NeuronSaved, async sender => {
                // if this is a target of the saved neuron
                if (sender.Axon.Any(t => t.TargetId == this.neuronId))
                {
                    Thread.Sleep(MainViewModel.GraphWaitInterval);
                    // reload so dendrites are refreshed
                    await this.LoadAsync();
                }
            });
        }

        private void MainViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(IsBusy):
                    ((Command)this.addExistingDendriteCommand).ChangeCanExecute();
                    ((Command)this.addTerminalCommand).ChangeCanExecute();
                    ((Command)this.createNewDendriteCommand).ChangeCanExecute();
                    ((Command)this.deleteCommand).ChangeCanExecute();
                    ((Command)this.saveCommand).ChangeCanExecute();
                    (this.removeDendriteCommand as Command)?.ChangeCanExecute();
                    (this.removeTerminalCommand as Command)?.ChangeCanExecute();
                    break;
                case nameof(Loaded):
                    ((Command)this.addExistingDendriteCommand).ChangeCanExecute();
                    ((Command)this.addTerminalCommand).ChangeCanExecute();
                    ((Command)this.createNewDendriteCommand).ChangeCanExecute();
                    ((Command)this.deleteCommand).ChangeCanExecute();
                    ((Command)this.newCommand).ChangeCanExecute();
                    ((Command)this.loadCommand).ChangeCanExecute();
                    break;
            }
        }

        public async override Task InitializeAsync(object navigationData)
        {
            this.IsBusy = true;

            if (navigationData is TabParameter)
            {
                // Change selected application tab
                var tabIndex = ((TabParameter)navigationData).TabIndex;
                MessagingCenter.Send(this, MessageKeys.ChangeTab, tabIndex);
            }
            else if (navigationData is NeuronParameter)
            {
                this.NeuronId = ((NeuronParameter)navigationData).NeuronId;
                await this.LoadAsync();
            }
            else if (navigationData is CreateNewDendriteParameter)
            {
                this.Initialize();
                this.Axon.Add(new Terminal {
                    Id = Guid.NewGuid().ToString(),
                    TargetId = ((CreateNewDendriteParameter)navigationData).TargetId, 
                    TargetData = ((CreateNewDendriteParameter)navigationData).TargetData
                }
                );
            }
            else
            {
                this.Initialize();
            }

            await base.InitializeAsync(navigationData);

            this.IsBusy = false;
        }

        private void Initialize()
        {
            this.NeuronId = Guid.NewGuid().ToString();
            this.Data = string.Empty;
            this.Version = 0;
            this.Timestamp = string.Empty;
            this.Axon = new ObservableCollection<Terminal>();
            this.Dendrites = new ObservableCollection<Dendrite>();

            this.Loaded = false;
        }

        private async Task SettingsAsync()
        {
            await NavigationService.NavigateToAsync<SettingsViewModel>();
        }

        public ICommand AddExistingDendriteCommand => 
            this.addExistingDendriteCommand ??
            (this.addExistingDendriteCommand = new Command(async () => await this.AddExistingDendriteAsync(), () => this.Loaded && !this.IsBusy));

        public ICommand AddTerminalCommand =>
            this.addTerminalCommand ??
            (this.addTerminalCommand = new Command(async () => await this.AddTerminalAsync(), () => this.Loaded && !this.IsBusy));

        public ICommand CreateNewDendriteCommand =>
            this.createNewDendriteCommand ??
            (this.createNewDendriteCommand = new Command(async () => await CreateNewDendriteAsync(), () => this.Loaded && !this.IsBusy));

        public ICommand DeleteCommand =>
            this.deleteCommand ??
            (this.deleteCommand = new Command(Delete, () => this.Loaded && !this.IsBusy));

        public ICommand DetailsCommand => new Command(async () => await DetailsAsync());

        public ICommand LoadCommand => 
            this.loadCommand ??
            (this.loadCommand = new Command(async () => await LoadAsync(), () => this.Loaded));
        
        public ICommand RemoveDendriteCommand => 
            this.removeDendriteCommand ??
            (this.removeDendriteCommand = new Command<Dendrite>(d => this.RemoveDendriteAsync(d), d => !this.IsBusy));

        public ICommand RemoveTerminalCommand => 
            this.removeTerminalCommand ??
            (this.removeTerminalCommand = new Command<Terminal>(t => this.RemoveTerminalAsync(t), t => !this.IsBusy));

        public ICommand SaveCommand => 
            this.saveCommand ??
            (this.saveCommand = new Command(Save, () => !this.IsBusy));

        public ICommand SearchCommand => new Command(async () => await this.SearchAsync());

        public ICommand SettingsCommand => new Command(async () => await SettingsAsync());

        public ICommand ShowAxonCommand => new Command(this.ShowAxon);

        public ICommand NewCommand => 
            this.newCommand ?? 
            (this.newCommand = new Command(async () => await NewAsync(), () => this.Loaded));

        public ICommand NavigateAxonTerminalCommand => new Command<Terminal>(async (terminal) => await NavigateAxonTerminalAsync(terminal));

        public ICommand NavigateDendriteCommand => new Command<Dendrite>(async (dendrite) => await NavigateDendriteAsync(dendrite));

        public ICommand LogoutCommand => new Command(async () => await LogoutAsync());

        private async Task AddExistingDendriteAsync()
        {
            await this.NavigationService.NavigateToAsync<SelectViewModel>(new SelectionParameter { CompletionProcessor = this.AddExistingDendriteCallback });
        }

        private void AddExistingDendriteCallback(Neuron selectedNeuron)
        {
            MainViewModel.ExecuteCommand(
                this,
                async () =>
                {
                    await neuronApplicationService.AddTerminalsToNeuron(
                        selectedNeuron.Id,
                        new Terminal[] { new Terminal { TargetId = this.neuronId } },
                        selectedNeuron.Version
                    );
                    return true;
                },
                async () => await this.LoadAsync(),
                this.messageService,
                this.DialogService,
                "Extension added.",
                $"An error occurred while adding an Extension to Neuron '{this.Data}'"
                );
        }

        private async Task AddTerminalAsync()
        {
            await this.NavigationService.NavigateToAsync<SelectViewModel>(new SelectionParameter { CompletionProcessor = this.AddTerminalCallback});
        }

        private void AddTerminalCallback(Neuron selectedNeuron)
        {
            MainViewModel.ExecuteCommand(
                this,
                async () =>
                {
                    await this.neuronApplicationService.AddTerminalsToNeuron(
                        this.neuronId,
                        new Terminal[] { new Terminal { TargetId = selectedNeuron.Id } },
                        this.version
                    );
                    return true;
                },
                async () => await this.LoadAsync(),
                this.messageService,
                this.DialogService,
                "Definition added.",
                $"An error occurred while adding a Definition to Neuron '{this.Data}'"
                );
        }

        private async Task CreateNewDendriteAsync()
        {
            await NavigationService.NavigateToAsync<MainViewModel>(
                new CreateNewDendriteParameter {
                    TargetId = this.neuronId,
                    TargetData = this.data
                }
                );
        }

        private void Delete()
        {
            MainViewModel.ExecuteCommand(
                this,
                async () =>
                {
                    var result = false;
                    if (
                        await this.DialogService.ShowConfirmAsync(
                        "Are you sure you wish to delete this Neuron?",
                        MainViewModel.DialogTitle)
                    )
                    {
                        await this.neuronApplicationService.DeactivateNeuron(
                            this.neuronId,
                            this.version
                            );
                        result = true;
                    }
                    return result;
                },
                async () =>
                {
                    if (this.NavigationService.CanNavigateBack)
                        await this.NavigationService.NavigateBack();
                    else
                    {
                        await this.NewAsync();
                        await this.NavigationService.RemoveLastFromBackStackAsync();
                    }
                },
                this.messageService,
                this.DialogService,
                "Neuron deleted.",
                $"An error occurred while deleting Neuron '{this.Data}'"
                );
        }

        private async Task DetailsAsync()
        {
            await this.DialogService.ShowAlertAsync(
                string.Format($"Id: {this.NeuronId}\nTimestamp: {this.Timestamp}\nVersion: {this.Version}"),
                MainViewModel.DialogTitle,
                "Ok"
                );
        }

        private async Task LoadAsync()
        {
            try
            {
                var neuron = await this.neuronQueryService.GetNeuron(this.neuronId);

                this.Data = neuron.Data;
                this.Timestamp = neuron.Timestamp;
                this.Version = neuron.Version;
                this.Axon = new ObservableCollection<Terminal>(neuron.Axon);
                this.Dendrites = new ObservableCollection<Dendrite>(neuron.Dendrites);

                this.Loaded = true;
            }
            catch (Exception ex)
            {
                string errorBasic = $"An error occurred while loading Neuron '{this.Data}'";
                await this.DialogService.ShowAlertAsync(errorBasic + " - " + ex.Message, "Error", "OK");
                System.Diagnostics.Trace.WriteLine(errorBasic + Environment.NewLine + ex.ToString());
            }
        }

        private async Task NavigateAxonTerminalAsync(Terminal terminal)
        {
            await NavigationService.NavigateToAsync<MainViewModel>(new NeuronParameter { NeuronId = terminal.TargetId });
        }

        private async Task NavigateDendriteAsync(Dendrite dendrite)
        {
            await NavigationService.NavigateToAsync<MainViewModel>(new NeuronParameter { NeuronId = dendrite.Id });
        }

        private async Task NewAsync()
        {
            await NavigationService.NavigateToAsync<MainViewModel>();            
        }

        private void RemoveDendriteAsync(Dendrite dendrite)
        {
            MainViewModel.ExecuteCommand(
                this,
                async () =>
                {
                    await neuronApplicationService.RemoveTerminalsFromNeuron(
                        dendrite.Id,
                        new Terminal[] { new Terminal { TargetId = this.neuronId } },
                        dendrite.Version
                        );
                    return true;
                },
                async () => await this.LoadAsync(),
                this.messageService,
                this.DialogService,
                "Extension removed.",
                $"An error occurred while removing Extension '{dendrite.Data}'"
            );
        }

        private void RemoveTerminalAsync(Terminal terminal)
        {
            MainViewModel.ExecuteCommand(
                this,
                async () =>
                {
                    await neuronApplicationService.RemoveTerminalsFromNeuron(
                        this.neuronId,
                        new Terminal[] { new Terminal { TargetId = terminal.TargetId } },
                        version
                        );
                    return true;
                },
                async () => await this.LoadAsync(),
                this.messageService,
                this.DialogService,
                "Definition removed.",
                $"An error occurred while removing Definition '{terminal.TargetData}'"
            );
        }

        private void Save()
        {
            MainViewModel.ExecuteCommand(
                this,
                async () =>
                {
                    if (this.Loaded)
                        await this.neuronApplicationService.ChangeNeuronData(
                            this.neuronId,
                            this.data,
                            this.version
                            );
                    else
                        await this.neuronApplicationService.CreateNeuron(
                            this.neuronId,
                            this.data,
                            this.Axon
                            );
                    MessagingCenter.Send(this, MessageKeys.NeuronSaved);
                    return true;
                },
                async () => await this.LoadAsync(),
                this.messageService,
                this.DialogService,
                "Neuron saved.",
                $"An error occurred while saving Neuron '{this.Data}'"
                );
        }

        private static void ExecuteCommand(ViewModelBase viewModel, Func<Task<bool>> execute, Func<Task> onCompletionAfterWait, 
            IMessageService messageService, IDialogService dialogService, string successMessage, string errorMessage)
        {
            Task.Run(async () =>
            {
                bool completed = false;
                try
                {
                    Device.BeginInvokeOnMainThread(() => viewModel.IsBusy = true);
                    completed = await execute();
                    if (completed)
                        Device.BeginInvokeOnMainThread(() => messageService.ShortAlert(successMessage));
                }
                catch (Exception ex)
                {
                    await dialogService.ShowAlertAsync($"{errorMessage} - {ex.Message}", "Error", "OK");
                    System.Diagnostics.Trace.WriteLine(errorMessage + Environment.NewLine + ex.ToString());
                }
                finally
                {
                    if (completed) await Task.Delay(MainViewModel.GraphWaitInterval);

                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (completed) await onCompletionAfterWait();
                        viewModel.IsBusy = false;
                    });
                }
            });
        }

        private async Task SearchAsync()
        {
            await this.NavigationService.NavigateToAsync<SelectViewModel>(new SelectionParameter { CompletionProcessorAsync = this.SearchCallback });
        }

        private async Task SearchCallback(Neuron selectedNeuron)
        {
            await NavigationService.NavigateToAsync<MainViewModel>(new NeuronParameter { NeuronId = selectedNeuron.Id });
            if (!this.Loaded)
                await this.NavigationService.RemoveLastFromBackStackAsync();            
        }

        private void ShowAxon()
        {
            this.IsAxonVisible = !this.IsAxonVisible;
        }
        
        private async Task LogoutAsync()
        {
            this.IsBusy = true;

            // Logout
            await NavigationService.NavigateToAsync<LoginViewModel>(new LogoutParameter { Logout = true });
            await NavigationService.RemoveBackStackAsync();

            this.IsBusy = false;
        }

        public string AxonPreview
        {
            get
            {
                string result = string.Empty;
                if (this.axon != null)
                {
                    foreach (var t in this.axon)
                    {
                        if (result.Length > 0)
                            result += ", ";

                        result += t.TargetData;
                    }
                }
                return result;
            }
        }

        public string NeuronId
        {
            get { return neuronId; }
            set
            {
                neuronId = value;
                this.OnPropertyChanged(nameof(NeuronId));
            }
        }        

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                this.OnPropertyChanged(nameof(Data));
            }
        }

        public bool Loaded
        {
            get => this.loaded;
            set
            {
                if (this.loaded != value)
                {
                    this.loaded = value;
                    this.OnPropertyChanged(nameof(Loaded));
                }
            }
        }

        public bool IsAxonVisible
        {
            get => this.isAxonVisible;
            set
            {
                isAxonVisible = value;
                this.OnPropertyChanged(nameof(IsAxonVisible));
            }
        }

        public string Timestamp
        {
            get { return timestamp; }
            set
            {
                timestamp = value;
                this.OnPropertyChanged(nameof(Timestamp));
            }
        }

        public int Version
        {
            get { return version; }
            set
            {
                version = value;
                this.OnPropertyChanged(nameof(Version));
            }
        }
        
        public ObservableCollection<Terminal> Axon
        {
            get { return axon; }
            set
            {
                axon = value;
                this.OnPropertyChanged(nameof(Axon));
                this.OnPropertyChanged(nameof(AxonPreview));
            }
        }

        public ObservableCollection<Dendrite> Dendrites
        {
            get { return dendrites; }
            set
            {
                dendrites = value;
                this.OnPropertyChanged(nameof(Dendrites));
            }
        }
    }
}