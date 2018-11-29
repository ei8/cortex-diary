using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
    public class NeuronTreePaneViewModel : PaneViewModel, IDisposable
    {
        private readonly INeuronApplicationService neuronApplicationService;
        private readonly INeuronQueryService neuronQueryService;
        private readonly IOriginService originService;
        private readonly ReadOnlyObservableCollection<NeuronViewModelBase> children;
        private readonly IDisposable cleanUp;
        private readonly IStatusService statusService;
        private readonly IDialogService dialogService;
        private bool reloaded;

        public NeuronTreePaneViewModel(INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, IOriginService originService = null, 
            IStatusService statusService = null, IDialogService dialogService = null)
        {
            this.neuronApplicationService = neuronApplicationService ?? Locator.Current.GetService<INeuronApplicationService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.originService = originService ?? Locator.Current.GetService<IOriginService>();
            this.statusService = statusService ?? Locator.Current.GetService<IStatusService>();
            this.dialogService = dialogService ?? Locator.Current.GetService<IDialogService>();
            this.reloaded = false;

            this.statusService.WhenPropertyChanged(s => s.Message)
                .Subscribe(s => this.StatusMessage = s.Sender.Message);

            bool DefaultPredicate(Node<Neuron, int> node) => node.IsRoot;
            var cache = new SourceCache<Neuron, int>(x => x.Id);

            this.AddCommand = ReactiveCommand.Create<object>(async(parameter) => await this.OnAddClicked(cache, parameter));
            this.SetAuthorCommand = ReactiveCommand.Create<object>(async(parameter) => await this.OnSetAuthorIdClicked(parameter));
            this.ReloadCommand = ReactiveCommand.Create(async() => await this.OnReloadClicked(cache));

            this.cleanUp = cache.AsObservableCache().Connect()
                .TransformToTree(child => child.CentralId, Observable.Return((Func<Node<Neuron, int>, bool>)DefaultPredicate))
                .Transform(e =>
                    e.Item.Type == RelativeType.Postsynaptic ?
                    (NeuronViewModelBase)(new PostsynapticViewModel(this.avatarUrl, e.Item.Tag, e, cache)) :
                    (NeuronViewModelBase)(new PresynapticViewModel(this.avatarUrl, e.Item.Tag, e, cache)))
                .Bind(out this.children)
                .DisposeMany()
                .Subscribe();
        }

        private async Task OnAddClicked(SourceCache<Neuron, int> cache, object parameter)
        {
            await Helper.SetStatusOnComplete(async () =>
                {
                    bool stat = false;
                    bool shouldAddAuthor = this.reloaded && cache.Count == 0;
                    bool addingAuthor = false;
                    if (shouldAddAuthor && (await this.dialogService.ShowDialogYesNo("This Avatar needs to be initialized. Would you like to add its Author?", parameter, out DialogResult yesno)).GetValueOrDefault())
                        addingAuthor = true;

                    if ((await this.dialogService.ShowDialogTextInput(addingAuthor ? "Enter Author Name (E-mail optional)" : "Enter Neuron tag: ", this.avatarUrl, parameter, out string result)).GetValueOrDefault())
                    {
                        Neuron n = new Neuron
                        {
                            Tag = result,
                            Id = Guid.NewGuid().GetHashCode(),
                            NeuronId = Guid.NewGuid().ToString(),
                            Type = RelativeType.NotSet,
                            Version = 1,
                        };

                        await this.neuronApplicationService.CreateNeuron(
                            this.avatarUrl,
                            n.NeuronId,
                            n.Tag,
                            new Terminal[0],
                            addingAuthor ? n.NeuronId : this.originService.GetAvatarByUrl(this.avatarUrl).AuthorId
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

        private async Task OnSetAuthorIdClicked(object parameter)
        {
            await Helper.SetStatusOnComplete(async() =>
                {
                    bool stat = false;

                    if ((await this.dialogService.ShowDialogSelectNeurons("Select Author Neuron", this.avatarUrl, parameter, false, out IEnumerable<Neuron> result)).GetValueOrDefault())
                    {
                        this.AuthorName = result.First().Tag;
                        this.originService.GetAvatarByUrl(this.avatarUrl).AuthorId = result.First().NeuronId;
                        stat = true;
                    }
                    return stat;                    
                },
                "Author set successfully.",
                this.statusService,
                "Author set cancelled."
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
                    this.reloaded = true;
                    return true;
                },
                "Reload successful.",
                this.statusService
            );
        }

        public ReactiveCommand AddCommand { get; }

        private string authorName;

        public string AuthorName
        {
            get => this.authorName;
            set => this.RaiseAndSetIfChanged(ref authorName, value);
        }

        public ReactiveCommand SetAuthorCommand { get; }

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
