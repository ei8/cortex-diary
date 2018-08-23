using ReactiveUI;
using Spiker.Neurons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.WinForms.ViewModels
{
    public class CortexGraphViewModel : ReactiveObject
    {
        private string avatarUri;
        private NeuronCollection neurons;
        private ICortexGraphClient cortexGraphClient;
        private TreeModel treeModel;

        public CortexGraphViewModel(NeuronCollection neurons, ICortexGraphClient cortexGraphClient)
        {
            this.AvatarUri = string.Empty;
            this.neurons = neurons;
            this.cortexGraphClient = cortexGraphClient;

            this.ReloadCommand = ReactiveCommand.Create(this.Reload, this.WhenAnyValue(vm => vm.AvatarUri).Select(s => !string.IsNullOrEmpty(s)));
        }

        public string AvatarUri
        {
            get => this.avatarUri;
            set => this.RaiseAndSetIfChanged(ref this.avatarUri, value);
        }

        public TreeModel TreeModel
        {
            get => this.treeModel;
            set => this.RaiseAndSetIfChanged(ref this.treeModel, value);
        }

        public ReactiveCommand ReloadCommand { get; }

        private async void Reload()
        {
            //try
            //{
                if (Uri.IsWellFormedUriString(this.AvatarUri, UriKind.Absolute))
                {
                    this.neurons.Clear();
                    (await this.cortexGraphClient.GetAll(this.AvatarUri)).ToList().ForEach(n => this.neurons.Add(n));
                    this.TreeModel = new TreeModel(this.neurons);
                    //{
                    //    RootId = string.IsNullOrWhiteSpace(this.rootToolStripTextBox.Text) || this.rootToolStripTextBox.Text == "[All Neurons]"?
                    //    string.Empty :
                    //    Helper.GetNeuronByData(this.rootToolStripTextBox.Text, this.neurons).Id
                    //};
                }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this, ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }
    }
}
