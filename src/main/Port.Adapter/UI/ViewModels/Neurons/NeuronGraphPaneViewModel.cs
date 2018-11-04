using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI;
using Splat;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Origin;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class NeuronGraphPaneViewModel : PaneViewModel, IDisposable
    {
        private readonly INeuronApplicationService neuronApplicationService;
        private readonly INeuronQueryService neuronQueryService;
        private readonly IOriginService originService;
        private readonly ReadOnlyObservableCollection<NeuronViewModelBase> children;
        private readonly IDisposable cleanUp;

        public NeuronGraphPaneViewModel(INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, IOriginService originService = null)
        {
            this.neuronApplicationService = neuronApplicationService ?? Locator.Current.GetService<INeuronApplicationService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.originService = originService ?? Locator.Current.GetService<IOriginService>();

            bool DefaultPredicate(Node<Neuron, int> node) => node.IsRoot;
            var cache = new SourceCache<Neuron, int>(x => x.Id);

            this.AddCommand = ReactiveCommand.Create(async () =>
                {
                    try
                    {
                        Neuron n = new Neuron
                        {
                            Data = "Root Neuron",
                            Id = Guid.NewGuid().GetHashCode(),
                            NeuronId = Guid.NewGuid().ToString(),
                            Type = RelativeType.NotSet
                        };

                        await this.neuronApplicationService.CreateNeuron(
                            this.avatarUrl,
                            n.NeuronId,
                            n.Data,
                            this.authorId, // TODO: this.originService.GetAvatarByUrl(this.avatarUrl).AuthorId,
                            new Terminal[0]
                            );
                        cache.AddOrUpdate(n);
                    }
                    catch (Exception ex)
                    {
                        this.StatusMessage = ex.Message;
                    }
                }
            );

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

        private string authorId;

        public string AuthorId
        {
            get => this.authorId;
            set => this.RaiseAndSetIfChanged(ref authorId, value);
        }

        private string avatarUrl;

        public string AvatarUrl
        {
            get => this.avatarUrl;
            set => this.RaiseAndSetIfChanged(ref this.avatarUrl, value);
        }

        public ReadOnlyObservableCollection<NeuronViewModelBase> Children => this.children;

        public ReactiveCommand ReloadCommand { get; }

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
