using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogSelectNeuronView.xaml
    /// </summary>
    public partial class DialogSelectNeuronsView : UserControl, IViewFor<DialogSelectNeuronsViewModel>
    {
        public DialogSelectNeuronsView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .Where(x => x != null)
                    .Subscribe(x => this.ViewModel = (DialogSelectNeuronsViewModel)x);

                this.WhenAnyValue(view => view.ViewModel)
                    .Select(cmd => Unit.Default)
                    .InvokeCommand(this.ViewModel.ReloadCommand);

                d(this.Bind(this.ViewModel, vm => vm.StatusMessage, v => v.StatusMessageLabel.Content));
                d(this.BindCommand(this.ViewModel, vm => vm.ReloadCommand, v => v.ReloadButton));
                d(this.BindCommand(this.ViewModel, vm => vm.SelectCommand, v => v.SelectButton));
                d(this.BindCommand(this.ViewModel, vm => vm.CancelCommand, v => v.CancelButton));
                d(this.OneWayBind(this.ViewModel, vm => vm.Neurons, v => v.NeuronList.ItemsSource));
                d(this.OneWayBind(this.ViewModel, vm => vm.AllowMultiSelect, v => v.NeuronList.SelectionMode, p => p ? SelectionMode.Extended : SelectionMode.Single));

                Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                    ev => this.NeuronList.SelectionChanged += ev,
                    ev => this.NeuronList.SelectionChanged -= ev
                    ).Subscribe(ep => this.ViewModel.SelectedNeurons = ((ListView)ep.Sender).SelectedItems);
            });
        }
        
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DialogSelectNeuronsViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DialogSelectNeuronsViewModel), typeof(DialogSelectNeuronsView), new PropertyMetadata(default(DialogSelectNeuronsViewModel)));

        public DialogSelectNeuronsViewModel ViewModel
        {
            get { return (DialogSelectNeuronsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
