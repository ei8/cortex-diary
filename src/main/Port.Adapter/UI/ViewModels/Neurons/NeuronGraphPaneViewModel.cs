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
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
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
        private readonly IDialogService dialogService;

        public NeuronGraphPaneViewModel(INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, IOriginService originService = null, 
            IStatusService statusService = null, IDialogService dialogService = null)
        {
            this.neuronApplicationService = neuronApplicationService ?? Locator.Current.GetService<INeuronApplicationService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.originService = originService ?? Locator.Current.GetService<IOriginService>();
            this.statusService = statusService ?? Locator.Current.GetService<IStatusService>();
            this.dialogService = dialogService ?? Locator.Current.GetService<IDialogService>();

            this.statusService.WhenPropertyChanged(s => s.Message)
                .Subscribe(s => this.StatusMessage = s.Sender.Message);

            bool DefaultPredicate(Node<Neuron, int> node) => node.IsRoot;
            var cache = new SourceCache<Neuron, int>(x => x.Id);

            this.AddCommand = ReactiveCommand.Create<object>(async(parameter) => await this.OnAddClicked(cache, parameter));
            this.SetAuthorIdCommand = ReactiveCommand.Create(async() => await this.OnSetAuthorIdClicked());
            this.ReloadCommand = ReactiveCommand.Create(async() => await this.OnReloadClicked(cache));

            this.cleanUp = cache.AsObservableCache().Connect()
                .TransformToTree(child => child.CentralId, Observable.Return((Func<Node<Neuron, int>, bool>)DefaultPredicate))
                .Transform(e =>
                    e.Item.Type == RelativeType.Postsynaptic ?
                    (NeuronViewModelBase)(new PostsynapticViewModel(this.avatarUrl, e.Item.Data, e, cache)) :
                    (NeuronViewModelBase)(new PresynapticViewModel(this.avatarUrl, e.Item.Data, e, cache)))
                .Bind(out this.children)
                .DisposeMany()
                .Subscribe();
        }

        private async Task OnAddClicked(SourceCache<Neuron, int> cache, object parameter)
        {
            await Helper.SetStatusOnComplete(async () =>
                {
                    bool stat = false;

                    if ((await this.dialogService.ShowDialogTextInput("Enter Neuron data: ", this.avatarUrl, parameter, out string result)).GetValueOrDefault())
                    {
                        Neuron n = new Neuron
                        {
                            Data = result,
                            Id = Guid.NewGuid().GetHashCode(),
                            NeuronId = Guid.NewGuid().ToString(),
                            Type = RelativeType.NotSet
                        };

                        await this.neuronApplicationService.CreateNeuron(
                            this.avatarUrl,
                            n.NeuronId,
                            n.Data,
                            new Terminal[0],
                            this.originService.GetAvatarByUrl(this.avatarUrl).AuthorId
                            );
                        cache.AddOrUpdate(n);
                        stat = true;
                    }
                    return stat;
                },
                "Neuron added successfully.",
                this.statusService,
                "Neuron addition cancelled."
                );
        }

        private async Task OnSetAuthorIdClicked()
        {
            await Helper.SetStatusOnComplete(() =>
                {
                    this.originService.GetAvatarByUrl(this.avatarUrl).AuthorId = this.authorId;
                    return Task.FromResult(true);
                },
                "Author set successfully.",
                this.statusService
                );
        }

        private async Task OnReloadClicked(SourceCache<Neuron, int> cache)
        {
            await Helper.SetStatusOnComplete(async () =>
                {
                    cache.Clear();
                    var relatives = await this.neuronQueryService.GetNeurons(this.avatarUrl);
                    this.originService.Save(this.avatarUrl);
                    cache.AddOrUpdate(relatives);
                    return true;
                },
                "Reload successful.",
                this.statusService
            );
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
