using neurUL.Cortex.Common;
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
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Notifications;
using ei8.Cortex.Diary.Common;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using ei8.Cortex.Library.Common;
using ei8.Cortex.Diary.Port.Adapter.UI.Common;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral
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
        private readonly Workspace workspace;
        
        public EditorToolViewModel(Workspace workspace = null, INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, INotificationApplicationService notificationApplicationService = null, ITerminalApplicationService terminalApplicationService = null,
            IStatusService statusService = null, IDialogService dialogService = null) : base("Editor")
        {
            this.dialogService = dialogService ?? Locator.Current.GetService<IDialogService>();
            this.neuronApplicationService = neuronApplicationService ?? Locator.Current.GetService<INeuronApplicationService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.notificationApplicationService = notificationApplicationService ?? Locator.Current.GetService<INotificationApplicationService>();
            this.terminalApplicationService = terminalApplicationService ?? Locator.Current.GetService<ITerminalApplicationService>();

            this.statusService = statusService ?? Locator.Current.GetService<IStatusService>();
            this.workspace = workspace ?? Locator.Current.GetService<Workspace>();

            this.EditorState = EditorStateValue.Browse;
            this.TargetDraft = new EditorNeuronViewModel();
            this.Target = null;
            this.InitDetailsSection();

            this.NewCommand = ReactiveCommand.Create(
                () => this.OnNewClicked(),
                this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.AvatarUrl,
                    vm => vm.RegionName,
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
                    vm => vm.RegionName,
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

            this.WhenAnyValue(vm => vm.statusService.Message)
                .Subscribe(m => this.StatusMessage = m);

            this.WhenAnyValue(vm => vm.workspace.ActiveDocument)
                .Where(ad => ad is IAvatarViewer)
                .Subscribe(ad => this.AvatarViewer = (IAvatarViewer)ad);

            (this).WhenAnyValue(vm => vm.AvatarViewer)
                .Where(av => this.EditorState == EditorStateValue.Browse)
                .Subscribe(av => this.UpdateFromViewer(av));

            this.WhenAnyValue(vm => vm.AvatarViewer.Target)
                .Where(lt => this.EditorState == EditorStateValue.Browse)
                .Subscribe(lt => this.Target = lt);

            this.WhenAnyValue(vm => vm.Target)
                .Subscribe(t =>
                {
                    if (t == null)
                    {
                        this.TargetDraft.Init();
                    }
                    else
                    {
                        this.TargetDraft.Id = t.Id;
                        this.TargetDraft.Tag = t.Tag;
                        this.TargetDraft.Effect = t.Effect;
                        this.TargetDraft.Strength = t.Strength;
                        this.TargetDraft.RelativeType = t.RelativeType;
                        this.TargetDraft.RegionId = t.RegionId;
                        this.TargetDraft.RegionName = t.RegionName;
                        this.TargetDraft.Version = t.Version;
                    }
                });

            this.WhenAnyValue(vm => vm.TargetDraft.RelativeType)
                .Where(rt => rt == null)
                .Subscribe(rt => this.TargetDraft.Effect = (NeurotransmitterEffect?) (this.TargetDraft.Strength = null));

            this.WhenAnyValue(vm => vm.AvatarViewer.AvatarUrl)
                .Where(au => this.EditorState == EditorStateValue.Browse)
                .Subscribe(au => this.AvatarUrl = au);

            this.WhenAnyValue(vm => vm.AvatarViewer.RegionId)
                .Where(li => this.EditorState == EditorStateValue.Browse)
                .Subscribe(li => this.RegionId = li);

            this.WhenAnyValue(vm => vm.AvatarViewer.RegionName)
                .Where(ln => this.EditorState == EditorStateValue.Browse)
                .Subscribe(ln => this.RegionName = ln);

            this.WhenAnyValue(vm => vm.EditorState, vm => vm.TargetDraft.RelativeType)
                .Select(x => x.Item1 == EditorStateValue.New && x.Item2 != null)
                .ToPropertyEx(this, vm => vm.AreTerminalParametersEditable);

            this.WhenAnyValue(vm => vm.EditorState, vm => vm.NewMode)
                .Select(x => x.Item2 != ViewModels.NewModeValue.NotSet && x.Item2 != ViewModels.NewModeValue.Neuron && x.Item1 == ViewModels.EditorStateValue.New)
                .ToPropertyEx(this, vm => vm.IsRelativeTypeEditable);

            this.WhenAnyValue(vm => vm.EditorState)
                .Where(es => es == EditorStateValue.Browse)
                .Subscribe(es =>
                {                    
                    this.NewModes = new NewModeValue[0];
                    this.NewMode = NewModeValue.NotSet;
                    this.UpdateFromViewer(this.AvatarViewer);
                });

            this.WhenAnyValue(vm => vm.NewMode)
                .Subscribe(nm =>
                {
                    if (nm == NewModeValue.Neuron)
                        this.TargetDraft.RelativeType = null;
                    if (nm != NewModeValue.Link)
                        this.TargetDraft.LinkCandidates = null;
                    else 
                        this.TargetDraft.Tag = string.Empty;
                });

            this.NewEditTagRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.TargetDraft.Tag,
                    (es, nm, t) => es == EditorStateValue.Browse || nm == NewModeValue.Link || !string.IsNullOrWhiteSpace(t)
                    ),
                (vm, state) => !state ? "Tag must neither be null, empty, nor consist of white-space characters." : string.Empty
                );

            this.NewLinkLinkCandidatesRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.TargetDraft.LinkCandidates,
                    (es, nm, lcs) => es != EditorStateValue.New || nm != NewModeValue.Link || (lcs != null && lcs.Count() > 0)
                    ),
                (vm, state) => !state ? "Must select at least one Link Candidate." : string.Empty
                );

            this.NewRelativeTypeRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.TargetDraft.RelativeType,
                    (es, nm, rt) => es != EditorStateValue.New || nm == NewModeValue.Neuron || (rt != null && rt != RelativeType.NotSet)
                    ),
                (vm, state) => !state ? "Relative Type must be either Postsynaptic or Presynaptic." : string.Empty                    
                );

            this.RelativeEffectRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.TargetDraft.Effect,
                    (es, nm, e) => es != EditorStateValue.New || nm == NewModeValue.Neuron || (e != null && e != NeurotransmitterEffect.NotSet)
                    ),
                (vm, state) => !state ? "Effect must be either Excite or Inhibit." : string.Empty
                );

            this.RelativeStrengthRule = this.ValidationRule(
                _ => this.WhenAnyValue(
                    vm => vm.EditorState,
                    vm => vm.NewMode,
                    vm => vm.TargetDraft.Strength,
                    (es, nm, s) => es != EditorStateValue.New || nm == NewModeValue.Neuron || (s != null && s > 0)
                    ),
                (vm, state) => !state ? "Strength must be greater than zero." : string.Empty
                );

            // TODO: this.IconSourcePath = @"pack://application:,,,/d23-wpf;component/images/wrench.ico";
        }

        private void UpdateFromViewer(IAvatarViewer av)
        {
            this.AvatarUrl = av.AvatarUrl;
            this.RegionId = av.RegionId;
            this.RegionName = av.RegionName;
            this.Target = av.Target;
        }

        private void InitDetailsSection()
        {
            this.RelativeTypes = Enum.GetValues(typeof(RelativeType)).OfType<RelativeType>().Where(rt => rt != RelativeType.NotSet);
            this.Effects = Enum.GetValues(typeof(NeurotransmitterEffect)).OfType<NeurotransmitterEffect>().Where(ne => ne != NeurotransmitterEffect.NotSet);
        }

        private void OnCancelClicked()
        {
            this.ProcessState = ProcessType.Cancelling;

            this.Target = null;
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
            if ((await this.dialogService.ShowDialogSelectNeurons("Select Neuron(s)", this.AvatarUrl, parameter, true, out IEnumerable<UINeuron> result)).GetValueOrDefault())
            {
                this.TargetDraft.LinkCandidates = result;
            }
        }

        private void OnNewClicked()
        {
            this.EditorState = EditorStateValue.New;
            this.InitModes();

            this.NewMode = NewModeValue.Neuron;

            this.InitDetailsSection();
            // don't lose Target (set target.Id to string.empty)
            this.TargetDraft.Init(false);
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
            var previousEditorState = this.EditorState;

            switch (this.EditorState)
            {
                case EditorStateValue.New:
                    // TODO: switch (this.NewMode)
                    //{
                    //    case NewModeValue.Neuron:
                    //        await ViewModels.Helper.CreateNeuron(
                    //            () => Task.FromResult(this.TargetDraft.Tag),
                    //            owner,
                    //            this.dialogService,
                    //            this.neuronQueryService,
                    //            this.neuronApplicationService,
                    //            this.notificationApplicationService,
                    //            this.statusService,
                    //            this.AvatarUrl,
                    //            this.RegionId
                    //            );
                    //        break;
                    //    case NewModeValue.Relative:
                    //        await ViewModels.Helper.CreateRelative(
                    //            () => Task.FromResult(this.TargetDraft.Tag),
                    //            (o) => Task.FromResult(new string[] {
                    //                ((int)this.TargetDraft.Effect).ToString(),
                    //                this.TargetDraft.Strength.ToString()
                    //            }),
                    //            owner,
                    //            this.dialogService,
                    //            this.neuronQueryService,
                    //            this.neuronApplicationService,
                    //            this.terminalApplicationService,
                    //            this.statusService,
                    //            this.AvatarUrl,
                    //            this.RegionId,
                    //            this.TargetDraft.Id,
                    //            this.TargetDraft.RelativeType.Value
                    //            );
                    //        break;
                    //    case NewModeValue.Link:
                    //        await ViewModels.Helper.LinkRelative(
                    //            () => Task.FromResult(this.TargetDraft.LinkCandidates),
                    //            (o) => Task.FromResult(new string[] {
                    //                ((int)this.TargetDraft.Effect).ToString(),
                    //                this.TargetDraft.Strength.ToString()
                    //            }),
                    //            owner,
                    //            this.terminalApplicationService,
                    //            this.statusService,
                    //            this.AvatarUrl,
                    //            this.TargetDraft.Id,
                    //            this.TargetDraft.RelativeType.Value
                    //            );
                    //        break;
                    //}
                    // TODO: reload Target and try to select created item
                    // TODO: do this, or find newly created Neuron and assign to Target
                    this.Target = null;

                    break;
                case EditorStateValue.Edit:
                    // TODO: await ViewModels.Helper.ChangeNeuronTag(
                    //    this.TargetDraft.Tag,
                    //    this.neuronApplicationService,
                    //    this.statusService,
                    //    this.AvatarUrl,
                    //    this.TargetDraft.Id,
                    //    this.TargetDraft.Version
                    //    );
                    break;
            }
            this.EditorState = EditorStateValue.Browse;

            this.ProcessState = ProcessType.Idle;

            if (this.IsAutoNew && previousEditorState == EditorStateValue.New)
                this.OnNewClicked();
        }

        [Reactive]
        public bool IsAutoNew { get; set; }

        [Reactive]
        public IEnumerable<NewModeValue> NewModes { get; set; }

        [Reactive]
        public NewModeValue NewMode { get; set; }

        [Reactive]
        public IAvatarViewer AvatarViewer { get; set; }

        [Reactive]
        public string AvatarUrl { get; set; }

        // TODO: Rename to Default Region ID - to be used for New Neurons only, not ones that are being edited
        [Reactive]
        public string RegionId { get; set; }

        [Reactive]
        public string RegionName { get; set; }

        [Reactive]
        public EditorNeuronData Target { get; set; }

        [Reactive]
        public EditorNeuronViewModel TargetDraft { get; set; }

        public ReactiveCommand<Unit, Unit> NewCommand { get; }

        public ReactiveCommand<object, Unit> SaveCommand { get; }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public ReactiveCommand<object, Unit> SelectCommand { get; }

        public ReactiveCommand<Unit, Unit> EditCommand { get; }

        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }

        [Reactive]
        public string StatusMessage { get; set; }

        [Reactive]
        public ProcessType ProcessState { get; set; }

        [Reactive]
        public EditorStateValue EditorState { get; set; }
               
        [Reactive]
        public IEnumerable<RelativeType> RelativeTypes { get; set; }
               
        [Reactive]
        public IEnumerable<NeurotransmitterEffect> Effects { get; set; }
               
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
