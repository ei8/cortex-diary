using DynamicData;
using ReactiveUI;
using Splat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    public class DialogSelectNeuronsViewModel : DialogViewModelBase, IDisposable
    {
        private readonly string avatarUrl;
        private readonly INeuronQueryService neuronQueryService;
        private readonly ReadOnlyObservableCollection<Neuron> neurons;
        private readonly IDisposable cleanUp;
        
        public DialogSelectNeuronsViewModel(string message, string avatarUrl, bool allowMultiSelect, INeuronQueryService neuronQueryService = null) : 
            base(message)
        {
            this.avatarUrl = avatarUrl;
            this.AllowMultiSelect = allowMultiSelect;
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            var list = new SourceList<Neuron>();
            this.ReloadCommand = ReactiveCommand.Create(async() => await this.OnReloadClicked(list));
            this.SelectCommand = ReactiveCommand.Create(this.OnSelectedClicked);
            this.CancelCommand = ReactiveCommand.Create(this.OnCancelledClicked);
            this.UserDialogResult = null;

            this.cleanUp = list.AsObservableList().Connect()
                .Bind(out this.neurons)
                .DisposeMany()
                .Subscribe();
        }

        private async Task OnReloadClicked(SourceList<Neuron> list)
        {
            try
            {
                list.Clear();
                var neurons = await this.neuronQueryService.GetNeurons(this.avatarUrl);
                list.AddRange(neurons);
                this.StatusMessage = "Reload successful.";
            }
            catch (Exception ex)
            {
                this.StatusMessage = ex.Message;
            }
        }

        private void OnSelectedClicked()
        {
            if (this.selectedNeurons != null)
            {
                this.UserDialogResult = this.selectedNeurons.Cast<Neuron>();
                this.DialogResult = true;
            }
            else
                this.StatusMessage = "No Neuron selected.";
        }
        
        private void OnCancelledClicked()
        {
            this.UserDialogResult = null;
            this.DialogResult = false;
        }

        public ReadOnlyObservableCollection<Neuron> Neurons => this.neurons;

        public ReactiveCommand ReloadCommand { get; }

        public ReactiveCommand SelectCommand { get; }

        public ReactiveCommand CancelCommand { get; }

        private IList selectedNeurons;

        public IList SelectedNeurons
        {
            get => this.selectedNeurons;
            set => this.RaiseAndSetIfChanged(ref this.selectedNeurons, value);
        }

        private bool allowMultiSelect;

        public bool AllowMultiSelect
        {
            get => this.allowMultiSelect;
            set => this.RaiseAndSetIfChanged(ref this.allowMultiSelect, value);
        }

        private string statusMessage;

        public string StatusMessage
        {
            get => this.statusMessage;
            set => this.RaiseAndSetIfChanged(ref this.statusMessage, value);
        }

        public void Dispose()
        {
            this.cleanUp.Dispose();
        }
    }
}
