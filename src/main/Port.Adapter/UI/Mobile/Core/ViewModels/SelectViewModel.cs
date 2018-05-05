using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using works.ei8.Cortex.Diary.Application.Dialog;
using works.ei8.Cortex.Diary.Application.Navigation;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.Models.Navigation;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels.Base;
using Xamarin.Forms;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Mobile.Core.ViewModels
{
    public class SelectViewModel : ViewModelBase
    {
        private INeuronQueryService neuronQueryService;
        private string searchValue;
        private ObservableCollection<Neuron> searchResults;
        private SelectionParameter selectionParameter;

        public SelectViewModel(
            ISettingsService settingsService, 
            IDialogService dialogService, 
            INavigationService<ViewModelBase> navigationService, 
            INeuronQueryService neuronQueryService
            ) : base(settingsService, dialogService, navigationService)
        {
            this.neuronQueryService = neuronQueryService;
            this.searchValue = string.Empty;
        }

        public async override Task InitializeAsync(object navigationData)
        {
            this.IsBusy = true;

            this.selectionParameter = navigationData as SelectionParameter;

            await base.InitializeAsync(navigationData);

            this.IsBusy = false;
        }

        public string SearchValue
        {
            get => this.searchValue;
            set
            {
                searchValue = value;
                this.OnPropertyChanged(nameof(SearchValue));
            }
        }
        
        public ObservableCollection<Neuron> SearchResults
        {
            get => this.searchResults;
            set
            {
                this.searchResults = value;
                this.OnPropertyChanged(nameof(SearchResults));
            }
        }

        public ICommand SelectNeuronCommand => new Command<Neuron>(async (neuron) => await SelectNeuronAsync(neuron));
        
        public ICommand SearchCommand => new Command(async () => await SearchAsync());

        private async Task SelectNeuronAsync(Neuron neuron)
        {
            await this.NavigationService.NavigateBack();
            if (this.selectionParameter != null)
            {
                if (this.selectionParameter.CompletionProcessor != null)
                    this.selectionParameter.CompletionProcessor(neuron);
                else if (this.selectionParameter.CompletionProcessorAsync != null)
                    await this.selectionParameter.CompletionProcessorAsync(neuron);
            }
        }

        private async Task SearchAsync()
        {
            try
            {
                this.SearchResults = new ObservableCollection<Neuron>(await this.neuronQueryService.GetAllNeuronsByDataSubstring(this.searchValue));
            }
            catch (Exception ex)
            {
                string errorBasic = "An error occurred while searching: " + this.searchValue;
                await this.DialogService.ShowAlertAsync(errorBasic + " - " + ex.Message, "Error", "OK");
                System.Diagnostics.Trace.WriteLine(errorBasic + Environment.NewLine + ex.ToString());
            }
        }
    }
}
