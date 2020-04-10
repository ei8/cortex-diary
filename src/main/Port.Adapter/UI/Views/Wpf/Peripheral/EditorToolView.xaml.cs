using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Peripheral
{
    /// <summary>
    /// Interaction logic for EditorToolView.xaml
    /// </summary>
    public partial class EditorToolView : UserControl, IViewFor<EditorToolViewModel>
    {
        public EditorToolView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(v => v.DataContext)
                    .Where(dc => dc != null)
                    .Subscribe(dc => this.ViewModel = (EditorToolViewModel)dc);

                this.WhenAnyValue(v => v.ViewModel.NewMode)
                    .Subscribe(nm => {
                        MaterialDesignThemes.Wpf.HintAssist.SetHint(
                            this.TagText,
                            nm == ViewModels.NewModeValue.Link ? "Link Candidates" : "Tag"
                            );
                        MaterialDesignThemes.Wpf.HintAssist.SetIsFloating(
                            this.TagText,
                            nm == ViewModels.NewModeValue.Link
                            );
                    });

                d(this.Bind(this.ViewModel, vm => vm.AvatarUrl, v => v.AvatarUrl.Text));
                d(this.Bind(this.ViewModel, vm => vm.RegionName, v => v.RegionName.Text));
                d(this.OneWayBind(this.ViewModel, vm => vm.Target, v => v.Target.Text, sn => sn == null ? string.Empty : sn.Tag));
                d(this.Bind(this.ViewModel, vm => vm.TargetDraft.Tag, v => v.TagText.Text));
                d(this.OneWayBind(this.ViewModel, vm => vm.EditorState, v => v.TagText.IsEnabled, es => es != ViewModels.EditorStateValue.Browse));

                d(this.BindCommand(this.ViewModel, vm => vm.SaveCommand, v => v.Save));
                d(this.BindCommand(this.ViewModel, vm => vm.CancelCommand, v => v.Cancel));
                d(this.BindCommand(this.ViewModel, vm => vm.NewCommand, v => v.New));
                d(this.BindCommand(this.ViewModel, vm => vm.EditCommand, v => v.Edit));
                d(this.BindCommand(this.ViewModel, vm => vm.SelectCommand, v => v.Select));

                d(this.OneWayBind(this.ViewModel, vm => vm.EditorState, v => v.New.IsChecked, es => es == ViewModels.EditorStateValue.New));
                d(this.OneWayBind(this.ViewModel, vm => vm.EditorState, v => v.Edit.IsChecked, es => es == ViewModels.EditorStateValue.Edit));
                d(this.OneWayBind(this.ViewModel, vm => vm.NewModes, v => v.NewMode.ItemsSource));
                d(this.OneWayBind(this.ViewModel, vm => vm.EditorState, v => v.NewMode.IsEnabled, es => es == ViewModels.EditorStateValue.New));
                d(this.OneWayBind(this.ViewModel, vm => vm.EditorState, v => v.AutoNew.IsEnabled, es => es == ViewModels.EditorStateValue.New));
                d(this.Bind(this.ViewModel, vm => vm.NewMode, v => v.NewMode.SelectedItem));
                d(this.Bind(this.ViewModel, vm => vm.IsAutoNew, v => v.AutoNew.IsChecked));
                d(this.OneWayBind(this.ViewModel, vm => vm.IsRelativeTypeEditable, v => v.RelativeType.IsEnabled));

                d(this.OneWayBind(this.ViewModel, vm => vm.RelativeTypes, v => v.RelativeType.ItemsSource));                
                d(this.Bind(this.ViewModel, vm => vm.TargetDraft.RelativeType, v => v.RelativeType.SelectedItem));

                d(this.OneWayBind(this.ViewModel, vm => vm.TargetDraft.LinkCandidates, v => v.TagText.Text, lns => lns != null && lns.Count() > 0 ? Environment.NewLine + string.Join(Environment.NewLine, lns.ToArray().Select(n => n.Tag)) : string.Empty));
                d(this.OneWayBind(this.ViewModel, vm => vm.NewMode, v => v.TagText.IsReadOnly, nm => nm == ViewModels.NewModeValue.Link)); 
                
                d(this.OneWayBind(this.ViewModel, vm => vm.Effects, v => v.Effect.ItemsSource));
                d(this.OneWayBind(this.ViewModel, vm => vm.AreTerminalParametersEditable, v => v.Effect.IsEnabled));
                d(this.Bind(this.ViewModel, vm => vm.TargetDraft.Effect, v => v.Effect.SelectedItem));

                d(this.OneWayBind(this.ViewModel, vm => vm.AreTerminalParametersEditable, v => v.Strength.IsEnabled));
                d(this.Bind(this.ViewModel, vm => vm.TargetDraft.Strength, v => v.Strength.Text));

                d(this.OneWayBind(this.ViewModel, vm => vm.ProcessState, v => v.Progress.Visibility, ps => ps != EditorToolViewModel.ProcessType.Idle ? Visibility.Visible : Visibility.Collapsed));
                d(this.OneWayBind(this.ViewModel, vm => vm.ProcessState, v => v.IsEnabled, ps => ps == EditorToolViewModel.ProcessType.Idle));

                d(this.OneWayBind(this.ViewModel, vm => vm.ValidationContext.Text, v => v.ErrorMessage.Text, t => t.ToSingleLine(Environment.NewLine)));
                d(this.OneWayBind(
                    this.ViewModel, 
                    vm => vm.ValidationContext.Text, 
                    v => v.ErrorMessage.Visibility, 
                    t => t.ToSingleLine(Environment.NewLine).Length > 0 ? Visibility.Visible : Visibility.Collapsed
                    ));
                d(this.Bind(this.ViewModel, vm => vm.StatusMessage, v => v.StatusMessage.Content));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (EditorToolViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", 
            typeof(EditorToolViewModel), 
            typeof(EditorToolView), 
            new PropertyMetadata(default(EditorToolViewModel))
            );

        public EditorToolViewModel ViewModel
        {
            get { return (EditorToolViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
