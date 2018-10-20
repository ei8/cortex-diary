using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Splat;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels.Neurons
{
    public class NeuronGraphPaneViewModel : PaneViewModel, IDisposable
    {
        private readonly INeuronService neuronService;
        private readonly ReadOnlyObservableCollection<NeuronViewModelBase> children;
        private readonly IDisposable cleanUp;

        public NeuronGraphPaneViewModel(INeuronService neuronService = null)
        {
            this.neuronService = neuronService ?? Locator.Current.GetService<INeuronService>();

            bool DefaultPredicate(Node<NeuronDto, int> node) => node.IsRoot;
            var cache = new SourceCache<NeuronDto, int>(x => x.Id);

            this.AddCommand = ReactiveCommand.Create(() =>
                this.neuronService.Add(cache, NeuronService.CreateNeuron("Root Neuron", ChildType.NotSet)));
            this.ReloadCommand = ReactiveCommand.Create(() => {
                cache.Clear();
                this.neuronService.Reload(cache);
                });
            this.cleanUp = cache.AsObservableCache().Connect()
                .TransformToTree(child => child.ParentId, Observable.Return((Func<Node<NeuronDto, int>, bool>)DefaultPredicate))
                .Transform(e =>
                    e.Item.Type == ChildType.Postsynaptic ?
                    (NeuronViewModelBase)(new PostsynapticViewModel(e.Item.Data, e, cache)) :
                    (NeuronViewModelBase)(new PresynapticViewModel(e.Item.Data, e, cache)))
                .Bind(out this.children)
                .DisposeMany()
                .Subscribe();
        }

        public ReactiveCommand AddCommand { get; } 

        public ReadOnlyObservableCollection<NeuronViewModelBase> Children => this.children;

        public ReactiveCommand ReloadCommand { get; }

        public void Dispose()
        {
            this.cleanUp.Dispose();
        }
    }
}
