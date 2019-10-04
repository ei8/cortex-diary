using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Application.Notifications;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral
{
    public class EditorToolViewModel : ToolViewModel, IValidatableViewModel
    {
        public enum ProcessType
        {
            Idle,
            Saving,
            Cancelling
        }

        private readonly IDialogService dialogService;
        private readonly INeuronApplicationService neuronApplicationService;
        private readonly INeuronQueryService neuronQueryService;
        private readonly INotificationApplicationService notificationApplicationService;
        private readonly ITerminalApplicationService terminalApplicationService;
        private readonly IStatusService statusService;
        private readonly IExtendedSelectionService selectionService;
        private readonly Workspace workspace;
        
        public EditorToolViewModel(Workspace workspace = null, INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, INotificationApplicationService notificationApplicationService = null, ITerminalApplicationService terminalApplicationService = null,
            IStatusService statusService = null, IDialogService dialogService = null, IExtendedSelectionService selectionService = null) : base("Editor")
        {
            this.dialogService = dialogService ?? Locator.Current.GetService<IDialogService>();
            this.neuronApplicationService = neuronApplicationService ?? Locator.Current.GetService<INeuronApplicationService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.notificationApplicationService = notificationApplicationService ?? Locator.Current.GetService<INotificationApplicationService>();
            this.terminalApplicationService = terminalApplicationService ?? Locator.Current.GetService<ITerminalApplicationService>();

            this.statusService = statusService ?? Locator.Current.GetService<IStatusService>();
            this.selectionService = selectionService ?? Locator.Current.GetService<IExtendedSelectionService>(SelectionContract.Select.ToString());
            this.workspace = workspace ?? Locator.Current.GetService<Workspace>();

            this.EditorState = EditorStateValue.Browse;
            this.InitDetailsSection();

            this.NewCommand = ReactiveCommand.Create(
                () => this.OnNewClicked(),
                this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.AvatarUrl,
                    vm => vm.LayerName,
                    (esv, au, ln) => esv == EditorStateValue.Browse && !string.IsNullOrEmpty(au) && !string.IsNullOrEmpty(ln)
                    )
                );

            this.SaveCommand = ReactiveCommand.Create<object>(
                async (parameter) => await this.OnSaveClicked(parameter),
                Observable.CombineLatest(
                    this.WhenAnyValue(
                        vm => vm.EditorState,
                        es => es == EditorStateValue.New || es == EditorStateValue.Edit
                    ),
                    this.IsValid(),
                    (esValid, dataValid) => esValid && dataValid
                    )
                );

            this.CancelCommand = ReactiveCommand.Create(
                () => this.OnCancelClicked(),
                this.WhenAnyValue(
                    vm => vm.EditorState,
                    es => es == EditorStateValue.New || es == EditorStateValue.Edit
                    )
                );

            this.EditCommand = ReactiveCommand.Create(
                () => this.OnEditClicked(),
                this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.AvatarUrl,
                    vm => vm.LayerName,
                    vm => vm.Target,
                    (esv, au, ln, t) => 
                        esv == EditorStateValue.Browse && 
                        !string.IsNullOrEmpty(au) && 
                        !string.IsNullOrEmpty(ln) && 
                        t != null
                    )
                );

            this.SelectCommand = ReactiveCommand.Create<object>(
                async (parameter) => await this.OnSelectClicked(parameter),
                this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    (es, nm) => es == EditorStateValue.New && nm == NewModeValue.Link
                    )
                );

            this.WhenAnyValue(vm => vm.selectionService.SelectedComponents)
                .Where(sc => sc != null && sc.Count > 0 && sc.OfType<object>().FirstOrDefault() is NeuronViewModelBase)
                .Subscribe(sc =>
                {
                    if (this.EditorState == EditorStateValue.Browse)
                    {
                        this.Target = (NeuronViewModelBase)sc.OfType<object>().FirstOrDefault();
                    }
                });

            this.WhenAnyValue(vm => vm.statusService.Message)
                .Subscribe(m => this.StatusMessage = m);

            this.WhenAnyValue(vm => vm.workspace.ActiveDocument)
                .Where(ad => ad is IAvatarViewer)
                .Subscribe(ad => this.AvatarViewer = (IAvatarViewer)ad);

            this.WhenAnyValue(vm => vm.AvatarViewer)
                .Subscribe(av =>
                {
                    if (this.EditorState == EditorStateValue.Browse)
                    {
                        this.AvatarUrl = av.AvatarUrl;
                        this.LayerId = av.LayerId;
                        this.LayerName = av.LayerName;
                        // TODO: make Target property of Avatar Viewer, which will store last selected item in viewer
                        this.Target = null;
                    }
                });

            (this).WhenAnyValue(vm => vm.Target)
                .Subscribe(t =>
                {
                    if (t == null)
                    {
                        this.InitTagRelativeType();
                    }
                    else
                    {
                        this.Tag = t.Tag;
                        this.RelativeType = t.Neuron.Type == Domain.Model.Neurons.RelativeType.NotSet ? (Domain.Model.Neurons.RelativeType?)null : t.Neuron.Type;
                        this.Effect = t.Effect == Domain.Model.Neurons.NeurotransmitterEffect.NotSet ? (NeurotransmitterEffect?)null : t.Effect;
                        this.Strength = t.Neuron.Type == Domain.Model.Neurons.RelativeType.NotSet ? (float?)null : t.Strength;
                    }
                });

            this.WhenAnyValue(vm => vm.RelativeType)
                .Subscribe(rt =>
                {
                    if (rt == null)
                    {
                        this.Effect = null;
                        this.Strength = null;
                    }
                });

            this.WhenAnyValue(vm => vm.AvatarViewer.AvatarUrl)
                .Subscribe(au =>
                {
                    if (this.EditorState == EditorStateValue.Browse)
                        this.AvatarUrl = au;
                });

            this.WhenAnyValue(vm => vm.AvatarViewer.LayerId)
                .Subscribe(li =>
                {
                    if (this.EditorState == EditorStateValue.Browse)
                        this.LayerId = li;
                });

            this.WhenAnyValue(vm => vm.AvatarViewer.LayerName)
                .Subscribe(ln =>
                {
                    if (this.EditorState == EditorStateValue.Browse)
                        this.LayerName = ln;
                });

            this.WhenAnyValue(vm => vm.EditorState, vm => vm.RelativeType)
                .Select(x => x.Item1 == EditorStateValue.New && x.Item2 != null)
                .ToPropertyEx(this, vm => vm.AreTerminalParametersEditable);

            this.WhenAnyValue(vm => vm.EditorState, vm => vm.NewMode)
                .Select(x => x.Item2 != ViewModels.NewModeValue.NotSet && x.Item2 != ViewModels.NewModeValue.Neuron && x.Item1 == ViewModels.EditorStateValue.New)
                .ToPropertyEx(this, vm => vm.IsRelativeTypeEditable);

            this.WhenAnyValue(vm => vm.EditorState)
                .Subscribe(es =>
                {
                    if (es == EditorStateValue.Browse)
                    {
                        this.NewModes = new NewModeValue[0];
                        this.NewMode = NewModeValue.NotSet;

                        switch (this.ProcessState)
                        {
                            case ProcessType.Cancelling:
                                if (this.Target == null)
                                    this.InitTagRelativeType();
                                else
                                {
                                    var target = this.Target;
                                    this.Target = null;
                                    this.Target = target;
                                }
                                break;
                            case ProcessType.Saving:
                                // TODO: reload Target and try to select created item
                                switch (this.lastEditorState)
                                {
                                    case EditorStateValue.New:
                                        // TODO: do this, or find newly created Neuron and assign to Target
                                        this.Target = null;
                                        break;
                                }
                                break;
                        }
                    }
                    this.lastEditorState = es;
                });

            this.WhenAnyValue(vm => vm.NewMode)
                .Subscribe(nm =>
                {
                    if (nm == NewModeValue.Neuron)
                        this.RelativeType = null;
                    if (nm != NewModeValue.Link)
                        this.LinkCandidates = null;
                    else 
                        this.Tag = string.Empty;
                });

            this.NewEditTagRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.Tag,
                    (es, nm, t) => es == EditorStateValue.Browse || nm == NewModeValue.Link || !string.IsNullOrWhiteSpace(t)
                    ),
                (vm, state) => !state ? "Tag must neither be null, empty, nor consist of white-space characters." : string.Empty
                );

            this.NewLinkLinkCandidatesRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.LinkCandidates,
                    (es, nm, lcs) => es != EditorStateValue.New || nm != NewModeValue.Link || (lcs != null && lcs.Count() > 0)
                    ),
                (vm, state) => !state ? "Must select at least one Link Candidate." : string.Empty
                );

            this.NewRelativeTypeRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.RelativeType,
                    (es, nm, rt) => es != EditorStateValue.New || nm == NewModeValue.Neuron || (rt != null && rt != Domain.Model.Neurons.RelativeType.NotSet)
                    ),
                (vm, state) => !state ? "Relative Type must be either Postsynaptic or Presynaptic." : string.Empty                    
                );

            this.RelativeEffectRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.Effect,
                    (es, nm, e) => es != EditorStateValue.New || nm == NewModeValue.Neuron || (e != null && e != Domain.Model.Neurons.NeurotransmitterEffect.NotSet)
                    ),
                (vm, state) => !state ? "Effect must be either Excite or Inhibit." : string.Empty
                );

            this.RelativeStrengthRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.Strength,
                    (es, nm, s) => es != EditorStateValue.New || nm == NewModeValue.Neuron || (s != null && s > 0)
                    ),
                (vm, state) => !state ? "Strength must be greater than zero." : string.Empty
                );

            // TODO: this.IconSourcePath = @"pack://application:,,,/Dasz;component/images/wrench.ico";
        }

        private void InitTagRelativeType()
        {
            this.Tag = string.Empty;
            this.RelativeType = null;
        }

        private void InitDetailsSection()
        {
            this.Tag = string.Empty;
            this.RelativeTypes = Enum.GetValues(typeof(RelativeType)).OfType<RelativeType>().Where(rt => rt != Domain.Model.Neurons.RelativeType.NotSet);
            this.Effects = Enum.GetValues(typeof(NeurotransmitterEffect)).OfType<NeurotransmitterEffect>().Where(ne => ne != NeurotransmitterEffect.NotSet);
            this.RelativeType = null;
        }

        private void OnCancelClicked()
        {
            this.ProcessState = ProcessType.Cancelling;

            // TODO: repopulate details section with target data
            this.EditorState = EditorStateValue.Browse;

            this.ProcessState = ProcessType.Idle;
        }

        private void OnEditClicked()
        {
            this.EditorState = EditorStateValue.Edit;
            this.InitModes();
        }

        private async Task OnSelectClicked(object parameter)
        {
            if ((await this.dialogService.ShowDialogSelectNeurons("Select Neuron(s)", this.AvatarUrl, parameter, true, out IEnumerable<Neuron> result)).GetValueOrDefault())
            {
                this.LinkCandidates = result;
            }
        }

        private void OnNewClicked()
        {
            this.EditorState = EditorStateValue.New;
            this.InitModes();

            this.NewMode = NewModeValue.Neuron;

            // Initialize only and don't lose Target (set it to null)
            this.InitDetailsSection();
        }

        private void InitModes()
        {
            this.NewModes = Enum.GetValues(typeof(NewModeValue)).OfType<NewModeValue>().Where(
                am => am != NewModeValue.NotSet && 
                    (
                        this.Target != null || 
                        (am != NewModeValue.Relative && am != NewModeValue.Link)
                    )
                );
        }

        private async Task OnSaveClicked(object owner)
        {
            this.ProcessState = ProcessType.Saving;
            
            switch (this.EditorState)
            {
                case EditorStateValue.New:
                    switch (this.NewMode)
                    {
                        case NewModeValue.Neuron:
                            await ViewModels.Helper.CreateNeuron(
                                () => Task.FromResult(this.Tag),
                                owner,
                                this.dialogService,
                                this.neuronQueryService,
                                this.neuronApplicationService,
                                this.notificationApplicationService,
                                this.statusService,
                                this.AvatarUrl,
                                this.LayerId
                                );
                            break;
                        case NewModeValue.Relative:
                            await ViewModels.Helper.CreateRelative(
                                () => Task.FromResult(this.Tag),
                                (o) => Task.FromResult(new string[] {
                            ((int)this.Effect).ToString(),
                            this.Strength.ToString()
                                }),
                                owner,
                                this.dialogService,
                                this.neuronQueryService,
                                this.neuronApplicationService,
                                this.terminalApplicationService,
                                this.statusService,
                                this.AvatarUrl,
                                this.LayerId,
                                this.Target.NeuronId,
                                this.RelativeType.Value
                                );
                            break;
                        case NewModeValue.Link:
                            await ViewModels.Helper.LinkRelative(
                                () => Task.FromResult(this.LinkCandidates),
                                (o) => Task.FromResult(new string[] {
                            ((int)this.Effect).ToString(),
                            this.Strength.ToString()
                                }),
                                owner,
                                this.terminalApplicationService,
                                this.statusService,
                                this.AvatarUrl,
                                this.Target.NeuronId,
                                this.RelativeType.Value
                                );
                            break;
                    }
                    break;
                case EditorStateValue.Edit:
                    await ViewModels.Helper.ChangeNeuronTag(
                        this.Tag,
                        this.neuronApplicationService,
                        this.statusService,
                        this.AvatarUrl,
                        this.Target.NeuronId,
                        this.Target.Neuron.Version
                        );
                    break;
            }
            
            this.EditorState = EditorStateValue.Browse;
            this.ProcessState = ProcessType.Idle;
        }

        [Reactive]
        public IEnumerable<NewModeValue> NewModes { get; set; }

        [Reactive]
        public NewModeValue NewMode { get; set; }

        [Reactive]
        public IAvatarViewer AvatarViewer { get; set; }

        [Reactive]
        public string Tag { get; set; }

        [Reactive]
        public string AvatarUrl { get; set; }

        [Reactive]
        public string LayerId { get; set; }

        [Reactive]
        public string LayerName { get; set; }

        // TODO: change to an EditableNeuronViewModel
        [Reactive]
        public NeuronViewModelBase Target { get; set; }

        public ReactiveCommand<Unit, Unit> NewCommand { get; }

        public ReactiveCommand<object, Unit> SaveCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<object, Unit> SelectCommand { get; }

        public ReactiveCommand<Unit, Unit> EditCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        [Reactive]
        public string StatusMessage { get; set; }

        private EditorStateValue lastEditorState = EditorStateValue.Browse;

        [Reactive]
        public ProcessType ProcessState { get; set; }

        [Reactive]
        public EditorStateValue EditorState { get; set; }

        [Reactive]
        public IEnumerable<Neuron> LinkCandidates { get; set; }

        [Reactive]
        public RelativeType? RelativeType { get; set; }

        [Reactive]
        public IEnumerable<RelativeType> RelativeTypes { get; set; }

        [Reactive]
        public NeurotransmitterEffect? Effect { get; set; }

        [Reactive]
        public IEnumerable<NeurotransmitterEffect> Effects { get; set; }

        [Reactive]
        public float? Strength { get; set; }

        public bool AreTerminalParametersEditable { [ObservableAsProperty] get; }
        
        public bool IsRelativeTypeEditable { [ObservableAsProperty] get; }

        public ValidationContext ValidationContext { get; } = new ValidationContext();

        public ValidationHelper NewEditTagRule { get; }

        public ValidationHelper NewRelativeTypeRule { get; }

        public ValidationHelper NewLinkLinkCandidatesRule { get; }

        public ValidationHelper RelativeEffectRule { get; }

        public ValidationHelper RelativeStrengthRule { get; }
    }
}
