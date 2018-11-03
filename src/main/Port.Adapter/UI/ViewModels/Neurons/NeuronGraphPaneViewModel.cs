using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Splat;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class NeuronGraphPaneViewModel : PaneViewModel, IDisposable
    {
        private readonly INeuronService neuronService;
        private readonly INeuronQueryService neuronQueryService;
        private readonly ReadOnlyObservableCollection<NeuronViewModelBase> children;
        private readonly IDisposable cleanUp;

        public NeuronGraphPaneViewModel(INeuronService neuronService = null, INeuronQueryService neuronQueryService = null)
        {
            this.neuronService = neuronService ?? Locator.Current.GetService<INeuronService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();

            bool DefaultPredicate(Node<Neuron, int> node) => node.IsRoot;
            var cache = new SourceCache<Neuron, int>(x => x.Id);

            // TODO: this.AddCommand = ReactiveCommand.Create(() =>
            //    this.neuronService.Add(cache, NeuronService.CreateNeuron("Root Neuron", RelativeType.NotSet)));
            this.ReloadCommand = ReactiveCommand.Create(async () => {
                cache.Clear();
                var relatives = await this.neuronQueryService.GetAll(this.avatarUrl);
                cache.AddOrUpdate(relatives);
            });
            this.cleanUp = cache.AsObservableCache().Connect()
                .TransformToTree(child => child.CentralId, Observable.Return((Func<Node<Neuron, int>, bool>)DefaultPredicate))
                .Transform(e =>
                    e.Item.Type == RelativeType.Postsynaptic ?
                    (NeuronViewModelBase)(new PostsynapticViewModel(avatarUrl, e.Item.Data, e, cache)) :
                    (NeuronViewModelBase)(new PresynapticViewModel(avatarUrl, e.Item.Data, e, cache)))
                .Bind(out this.children)
                .DisposeMany()
                .Subscribe();
        }

        public ReactiveCommand AddCommand { get; }

        private string avatarUrl;

        public string AvatarUrl
        {
            get => this.avatarUrl;
            set => this.RaiseAndSetIfChanged(ref this.avatarUrl, value);
        }

        public ReadOnlyObservableCollection<NeuronViewModelBase> Children => this.children;

        public ReactiveCommand ReloadCommand { get; }

        public void Dispose()
        {
            this.cleanUp.Dispose();
        }
    }
}
