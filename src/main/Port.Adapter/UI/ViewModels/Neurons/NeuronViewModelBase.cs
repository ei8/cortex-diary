//The MIT License(MIT)

//Copyright(c) 2016 Chris Condron

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
// https://github.com/condron/ReactiveUI-TreeView
// Modifications copyright(C) 2018 ei8/Elmer Bool

using DynamicData;
using DynamicData.Binding;
using DynamicData.Kernel;
using org.neurul.Cortex.Common;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Common;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    // TODO: are the Post and Pre viewmodel derivations necessary?
    public abstract class NeuronViewModelBase : ReactiveObject, IDisposable, IEquatable<NeuronViewModelBase>
    {
        private readonly IAvatarViewer host;
        private readonly IDisposable cleanUp;
        private int id;
        private string neuronId;
        private string tag;
        private NeurotransmitterEffect effect;
        private float strength;

        private LastModifiedInfo lastModified;
        private LayerInfo layerInfo;
        private string terminalId;

        private bool isExpanded;
        private bool isSelected;
        private bool isHighlighted;
        private ReadOnlyObservableCollection<NeuronViewModelBase> children;
        private IExtendedSelectionService selectionService;
        private IExtendedSelectionService highlightService;
        private readonly INeuronApplicationService neuronApplicationService;
        private readonly INeuronQueryService neuronQueryService;
        private readonly ITerminalApplicationService terminalApplicationService;
        private readonly IStatusService statusService;
        private readonly IDialogService dialogService;
        private bool settingNeuron;
        // cortex graph poll interval + 100 millisecond allowance
        private const int ReloadDelay = 2100;
        private const string NeuronCategory = "Neuron";
        private const string TerminalCategory = "Terminal";
        private const string MiscCategory = "Misc";
        
        protected NeuronViewModelBase(IAvatarViewer host, Node<Neuron, int> node, SourceCache<Neuron, int> cache, NeuronViewModelBase parent = null, INeuronApplicationService neuronApplicationService = null,
            INeuronQueryService neuronQueryService = null, ITerminalApplicationService terminalApplicationService = null, IExtendedSelectionService selectionService = null, IExtendedSelectionService highlightService = null, IStatusService statusService = null, IDialogService dialogService = null)
        {
            this.host = host;
            this.Id = node.Key;
            this.Parent = parent;
            this.SetNeuron(node.Item);

            this.ReloadCommand = ReactiveCommand.Create(async () => await this.OnReload(cache));
            this.ReloadExpandCommand = ReactiveCommand.Create(async () =>
            {
                await this.OnReload(cache);
                this.IsExpanded = true;
            });
            this.AddPostsynapticCommand = ReactiveCommand.Create<object>(async (parameter) => await this.OnAddPostsynaptic(cache, parameter));
            this.AddPresynapticCommand = ReactiveCommand.Create<object>(async (parameter) => await this.OnAddPresynaptic(cache, parameter));
            this.DeleteCommand = ReactiveCommand.Create<object>(async (parameter) => await this.OnDeleteClicked(cache, parameter));

            this.neuronApplicationService = neuronApplicationService ?? Locator.Current.GetService<INeuronApplicationService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.terminalApplicationService = terminalApplicationService ?? Locator.Current.GetService<ITerminalApplicationService>();
            this.selectionService = selectionService ?? Locator.Current.GetService<IExtendedSelectionService>(SelectionContract.Select.ToString());
            this.highlightService = highlightService ?? Locator.Current.GetService<IExtendedSelectionService>(SelectionContract.Highlight.ToString());
            this.statusService = statusService ?? Locator.Current.GetService<IStatusService>();
            this.dialogService = dialogService ?? Locator.Current.GetService<IDialogService>();

            var childrenLoader = new Lazy<IDisposable>(() => node.Children.Connect()
                .Transform(e =>
                    e.Item.Type == RelativeType.Postsynaptic ?
                    (NeuronViewModelBase)(new PostsynapticViewModel(this.host, e.Item.Tag, e, cache, this)) :
                    (NeuronViewModelBase)(new PresynapticViewModel(this.host, e.Item.Tag, e, cache, this)))
                .Bind(out this.children)
                .DisposeMany()
                .Subscribe()
                );

            var shouldExpand = node.IsRoot ?
                Observable.Return(true) :
                Parent.Value.WhenValueChanged(t => t.IsExpanded);

            var expander = shouldExpand
                .Where(isExpanded => isExpanded)
                .Take(1)
                .Subscribe(_ =>
                {
                    var x = childrenLoader.Value;
                });

            var childrenCount = node.Children.CountChanged
                .Select(count =>
                {
                    if (count == 0)
                        return "0 Synapses";
                    else
                        return $"{node.Children.Items.Count(n => n.Item.Type == RelativeType.Postsynaptic)} Postsynaptic; " +
                            $"{node.Children.Items.Count(n => n.Item.Type == RelativeType.Presynaptic)} Presynaptic";
                })
                .Subscribe(text => this.ChildrenCountText = text);

            var changeTag = this.WhenPropertyChanged(p => p.Tag, false)
                .Subscribe(async (x) => await this.OnNeuronTagChanged(cache, x));

            var selector = this.WhenPropertyChanged(p => p.IsSelected)
                .Where(p => p.Value)
                .Subscribe(x =>
                {
                    this.selectionService.SetSelectedComponents(new object[] { x.Sender });
                    this.host.Target = NeuronViewModelBase.ConvertNeuronViewModelToEditorNeuron((NeuronViewModelBase)x.Sender);
                });

            var highlighter = this.highlightService.WhenPropertyChanged(a => a.SelectedComponents)
                .Subscribe(p =>
                {
                    if (p.Sender.SelectedComponents != null)
                    {
                        var selection = p.Sender.SelectedComponents.OfType<object>().ToArray();
                        if (selection.Count() > 0 && selection[0] is string)
                        {
                            if (selection.Count() < 2)
                            {
                                this.IsHighlighted = this.NeuronId == p.Sender.PrimarySelection.ToString();
                            }
                            else
                            {
                                this.IsHighlighted =
                                    this.NeuronId == p.Sender.PrimarySelection.ToString() &&
                                    this.TerminalId == selection[1].ToString();
                            }
                        }
                    }
                }
                );

            this.cleanUp = Disposable.Create(() =>
            {
                expander.Dispose();
                childrenCount.Dispose();
                if (childrenLoader.IsValueCreated)
                    childrenLoader.Value.Dispose();
                changeTag.Dispose();
                selector.Dispose();
                highlighter.Dispose();
            });
        }

        private static EditorNeuronData ConvertNeuronViewModelToEditorNeuron(NeuronViewModelBase n)
        {
            return new EditorNeuronData(
                n.NeuronId,
                n.Tag,
                n.Effect == NeurotransmitterEffect.NotSet ? (NeurotransmitterEffect?)null : n.Effect,
                n.Neuron.Type == RelativeType.NotSet ? (float?)null : n.Strength,
                n.Neuron.Type == RelativeType.NotSet ? (RelativeType?)null : n.Neuron.Type,
                n.Layer.Id,
                n.Layer.Name,
                n.Neuron.Version
            );
        }

        private async Task OnDeleteClicked(SourceCache<Neuron, int> cache, object parameter)
        {
            await Helper.SetStatusOnComplete(async () =>
                {
                    bool stat = false;
                    string message = string.Empty;
                    if (this.Neuron.Type == RelativeType.NotSet)
                        message = $"Are you sure you wish to delete Neuron '{this.Tag}'?";
                    else
                        message = $"Are you sure you wish to delete relative '{this.Tag}' of '{this.Parent.Value.Neuron.Tag}'?";

                    if ((await this.dialogService.ShowDialogYesNo(message, parameter, out DialogResult result)).GetValueOrDefault() &&
                        result == DialogResult.Yes)
                    {
                        switch (this.Neuron.Type)
                        {
                            case RelativeType.NotSet:
                                await this.neuronApplicationService.DeactivateNeuron(
                                    this.host.AvatarUrl,
                                    this.NeuronId,
                                    this.Neuron.Version
                                );
                                cache.Remove(this.Neuron);
                                break;
                            case RelativeType.Postsynaptic:
                                await this.terminalApplicationService.DeactivateTerminal(
                                    this.host.AvatarUrl,
                                    this.Neuron.Terminal.Id,
                                    this.Neuron.Terminal.Version
                                    );
                                cache.Remove(this.Neuron);
                                break;
                            case RelativeType.Presynaptic:
                                await this.terminalApplicationService.DeactivateTerminal(
                                    this.host.AvatarUrl,
                                    this.Neuron.Terminal.Id,
                                    this.Neuron.Terminal.Version
                                    );
                                cache.Remove(this.Neuron);
                                break;
                        }
                        stat = true;
                    }
                    return stat;
                },
                "Deletion successful.",
                this.statusService,
                "Deletion cancelled."
            );
        }

        private async Task OnNeuronTagChanged(SourceCache<Neuron, int> cache, PropertyValue<NeuronViewModelBase, string> x)
        {
            if (!this.settingNeuron)
            {
                var success = await ViewModels.Helper.ChangeNeuronTag(
                    x.Value,
                    this.neuronApplicationService,
                    this.statusService,
                    this.host.AvatarUrl,
                    this.neuronId,
                    this.Neuron.Version
                    );

                if (success)
                    await this.OnReload(cache, NeuronViewModelBase.ReloadDelay);
            }
        }

        private void SetNeuron(Neuron neuron)
        {
            this.settingNeuron = true;
            this.Neuron = neuron;
            this.NeuronId = neuron.Id;
            this.Tag = neuron.Tag;
            this.LastModified = new LastModifiedInfo()
            {
                Neuron = new ModificationInfo()
                {
                    Author = new AuthorInfo()
                    {
                        Id = neuron.AuthorId,
                        Name = neuron.AuthorTag
                    },
                    Timestamp = neuron.Timestamp,
                    Version = neuron.Version
                }
            };
            this.layerInfo = new LayerInfo()
            {
                Id = neuron.LayerId,
                Name = string.IsNullOrEmpty(neuron.LayerTag) ? "[Base]" : neuron.LayerTag
            };
        
            if (neuron.Terminal != null)
            {
                this.TerminalId = neuron.Terminal.Id;
                if (int.TryParse(neuron.Terminal.Effect, out int ie))
                    this.Effect = (NeurotransmitterEffect)ie;
                if (float.TryParse(neuron.Terminal.Strength, out float fs))
                    this.Strength = fs;
                this.LastModified.Terminal = new ModificationInfo()
                {
                    Author = new AuthorInfo()
                    {
                        Id = neuron.Terminal.AuthorId,
                        Name = neuron.Terminal.AuthorTag
                    },
                    Timestamp = neuron.Terminal.Timestamp,
                    Version = neuron.Terminal.Version
                };
            }
            this.settingNeuron = false;
        }

        private async Task OnReload(SourceCache<Neuron, int> cache, int millisecondsDelay = 0)
        {
            this.host.Loading = true;

            if (millisecondsDelay > 0)
                await Task.Delay(millisecondsDelay);

            await Helper.SetStatusOnComplete(async () =>
            {
                // reload self
                var reloadedNeuron = (await this.neuronQueryService.GetNeuronById(
                    this.host.AvatarUrl,
                    this.Neuron.Id,
                    this.Parent.ConvertOr(n => n.Neuron.Id, () => null),
                    this.Neuron.Type
                    )).First();
                NeuronViewModelBase.CopyNeuronData(this.Neuron, reloadedNeuron);
                this.SetNeuron(this.Neuron);

                // reload relatives
                cache.Remove(NeuronViewModelBase.GetAllChildren(cache, this.Neuron.UIId));
                var relatives = await this.neuronQueryService.GetNeurons(this.host.AvatarUrl, this.Neuron.Id);
                relatives.FillUIIds(this.Neuron);
                cache.AddOrUpdate(relatives);
                return true;
            },
            "Neuron reloaded successfully.",
            this.statusService
            );

            this.host.Loading = false;
        }

        private static IEnumerable<Neuron> GetAllChildren(SourceCache<Neuron, int> cache, int parentId)
        {
            var currList = cache.Items.Where(i => i.CentralUIId == parentId);
            currList.ToList().ForEach(n => currList = currList.Concat(NeuronViewModelBase.GetAllChildren(cache, n.UIId)));
            return currList;
        }

        private static void CopyNeuronData(Neuron target, Neuron source)
        {
            // TODO: Use Copy Constructor instead?
            target.Tag = source.Tag;
            target.AuthorId = source.AuthorId;
            target.AuthorTag = source.AuthorTag;
            target.LayerId = source.LayerId;
            target.LayerTag = source.LayerTag;
            target.Timestamp = source.Timestamp;
            target.Version = source.Version;
            target.Errors = source.Errors;

            if (source.Terminal != null)
                target.Terminal = new Terminal(source.Terminal);
        }

        private async Task OnAddPresynaptic(SourceCache<Neuron, int> cache, object owner)
        {
            var success = await ViewModels.Helper.CreateRelative(
                async () =>
                {
                    string result = string.Empty;

                    if (
                        (await this.dialogService.ShowDialogTextInput(
                        "Enter Presynaptic Tag: ",
                        owner,
                        out string r
                        )).GetValueOrDefault()
                        )
                        result = r;

                    return result;
                },
                async (o) => await this.AskUserTerminalParameters(o),
                owner,
                this.dialogService,
                this.neuronQueryService,
                this.neuronApplicationService,
                this.terminalApplicationService,
                this.statusService,                
                this.host.AvatarUrl,
                this.host.LayerId,
                this.Neuron.Id,
                RelativeType.Presynaptic
                );

            if (success)
                await this.OnReload(cache, NeuronViewModelBase.ReloadDelay);
        }

        private async Task<string[]> AskUserTerminalParameters(object parameter)
        {
            (await this.dialogService.ShowDialogTextInput("Enter Terminal Parameters (Format = '[effect],[strength]', 'Cancel' to use default): ", parameter, out string terminalParams)).GetValueOrDefault();
            string[] tps = terminalParams.Split(',');
            if (tps.Length == 1)
                tps = new string[] { "1", "1" };
            return tps;
        }

        private async Task OnAddPostsynaptic(SourceCache<Neuron, int> cache, object owner)
        {
            var success = await ViewModels.Helper.LinkRelative(
                async () =>
                {
                    IEnumerable<Neuron> result = new Neuron[0];

                    if (
                        (await this.dialogService.ShowDialogSelectNeurons(
                            "Select Neuron(s)", 
                            this.host.AvatarUrl, 
                            owner, 
                            true, 
                            out IEnumerable<Neuron> r)).GetValueOrDefault()
                        )
                        result = r;

                    return result;
                },
                async (o) => await this.AskUserTerminalParameters(o),
                owner,
                this.terminalApplicationService,
                this.statusService,
                this.host.AvatarUrl,
                this.Neuron.Id,
                RelativeType.Postsynaptic
                );
            
            if (success)
                await this.OnReload(cache, NeuronViewModelBase.ReloadDelay);
        }

        [Browsable(false)]
        public Neuron Neuron { get; private set; }

        private string childrenCountText;

        [Browsable(false)]
        public string ChildrenCountText
        {
            get => this.childrenCountText;
            set => this.RaiseAndSetIfChanged(ref this.childrenCountText, value);
        }

        [ReadOnly(true)]
        [Browsable(false)]
        public int Id
        {
            get => this.id;
            set => this.RaiseAndSetIfChanged(ref this.id, value);
        }

        [ParenthesizePropertyName(true)]
        [ReadOnly(true)]
        [Category(NeuronViewModelBase.NeuronCategory)]
        public string NeuronId
        {
            get => this.neuronId;
            set => this.RaiseAndSetIfChanged(ref this.neuronId, value);
        }

        [Category(NeuronViewModelBase.NeuronCategory)]
        public string Tag
        {
            get => this.tag;
            set => this.RaiseAndSetIfChanged(ref this.tag, value);
        }

        [ReadOnly(true)]
        [Category(NeuronViewModelBase.TerminalCategory)]
        public NeurotransmitterEffect Effect
        {
            get => this.effect;
            set => this.RaiseAndSetIfChanged(ref this.effect, value);
        }

        [ReadOnly(true)]
        [Category(NeuronViewModelBase.TerminalCategory)]
        public float Strength
        {
            get => this.strength;
            set => this.RaiseAndSetIfChanged(ref this.strength, value);
        }

        [Category(NeuronViewModelBase.MiscCategory)]
        [DisplayName("Last Modified")]
        public LastModifiedInfo LastModified
        {
            get => this.lastModified;
            set => this.RaiseAndSetIfChanged(ref this.lastModified, value);
        }

        [Category(NeuronViewModelBase.NeuronCategory)]
        public LayerInfo Layer
        {
            get => this.layerInfo;
            set => this.RaiseAndSetIfChanged(ref this.layerInfo, value);
        }

        [ParenthesizePropertyName(true)]
        [ReadOnly(true)]
        [Category(NeuronViewModelBase.TerminalCategory)]
        public string TerminalId
        {
            get => this.terminalId;
            set => this.RaiseAndSetIfChanged(ref this.terminalId, value);
        }

        [Browsable(false)]
        public bool IsExpanded
        {
            get => this.isExpanded;
            set => this.RaiseAndSetIfChanged(ref isExpanded, value);
        }

        [Browsable(false)]
        public bool IsSelected
        {
            get => this.isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }

        [Browsable(false)]
        public bool IsHighlighted
        {
            get => this.isHighlighted;
            set => this.RaiseAndSetIfChanged(ref this.isHighlighted, value);
        }

        [Browsable(false)]
        public ReactiveCommand<Unit, Task> ReloadCommand { get; }

        [Browsable(false)]
        public ReactiveCommand<Unit, Task> ReloadExpandCommand { get; }

        [Browsable(false)]
        public ReactiveCommand<object, Unit> AddPostsynapticCommand { get; }

        [Browsable(false)]
        public ReactiveCommand<object, Unit> AddPresynapticCommand { get; }

        [Browsable(false)]
        public ReactiveCommand<object, Unit> DeleteCommand { get; }

        [Browsable(false)]
        public abstract object ViewModel { get; }

        [Browsable(false)]
        public ReadOnlyObservableCollection<NeuronViewModelBase> Children => this.children;

        [Browsable(false)]
        public Optional<NeuronViewModelBase> Parent { get; }

        [Browsable(false)]
        public new IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing => base.Changing;
        [Browsable(false)]
        public new IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed => base.Changed;
        [Browsable(false)]
        public new IObservable<Exception> ThrownExceptions => base.ThrownExceptions;

        public override string ToString()
        {
            return $"{this.NeuronId}:Neuron '{this.Tag}'";
        }

        public void Dispose()
        {
            this.cleanUp.Dispose();
        }

        #region Equality Members

        public bool Equals(NeuronViewModelBase other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NeuronViewModelBase) obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(NeuronViewModelBase left, NeuronViewModelBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NeuronViewModelBase left, NeuronViewModelBase right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
