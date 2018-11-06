using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
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
        private readonly IStatusService statusService;

        public NeuronGraphPaneViewModel(INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, IOriginService originService = null, IStatusService statusService = null)
        {
            this.neuronApplicationService = neuronApplicationService ?? Locator.Current.GetService<INeuronApplicationService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.originService = originService ?? Locator.Current.GetService<IOriginService>();
            this.statusService = statusService ?? Locator.Current.GetService<IStatusService>();

            this.statusService.WhenPropertyChanged(s => s.Message)
                .Subscribe(s => this.StatusMessage = s.Sender.Message);

            bool DefaultPredicate(Node<Neuron, int> node) => node.IsRoot;
            var cache = new SourceCache<Neuron, int>(x => x.Id);

            this.AddCommand = ReactiveCommand.Create(() =>
                Helper.SetStatusOnComplete(async() =>
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
                        this.originService.GetAvatarByUrl(this.avatarUrl).AuthorId,
                        new Terminal[0]
                        );
                    cache.AddOrUpdate(n);
                },
                "Neuron added successfully.",
                this.statusService
                )
            );

            this.SetAuthorIdCommand = ReactiveCommand.Create(() =>
                Helper.SetStatusOnComplete(async() =>
                {
                    this.originService.GetAvatarByUrl(this.avatarUrl).AuthorId = this.authorId;
                    await Task.CompletedTask;
                },
                "Author set successfully.",
                this.statusService
                )
            );

            this.ReloadCommand = ReactiveCommand.Create(() => 
                Helper.SetStatusOnComplete(async() =>
                {
                    cache.Clear();
                    var relatives = await this.neuronQueryService.GetAll(this.avatarUrl);
                    this.originService.Save(this.avatarUrl);
                    cache.AddOrUpdate(relatives);
                },
                "Reload successful.",
                this.statusService
                )
            );
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

        public ReactiveCommand SetAuthorIdCommand { get; }

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
