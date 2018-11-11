using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogSelectNeuronView.xaml
    /// </summary>
    public partial class DialogSelectNeuronView : UserControl, IViewFor<DialogSelectNeuronViewModel>
    {
        public DialogSelectNeuronView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.DataContext)
                .Where(x => x != null)
                .Subscribe(x => this.ViewModel = (DialogSelectNeuronViewModel)x);

            this.WhenActivated(d =>
            {
                d(this.Bind(this.ViewModel, vm => vm.StatusMessage, v => v.StatusMessageLabel.Content));
                d(this.BindCommand(this.ViewModel, vm => vm.ReloadCommand, v => v.ReloadButton));
                d(this.BindCommand(this.ViewModel, vm => vm.SelectCommand, v => v.SelectButton));
                d(this.BindCommand(this.ViewModel, vm => vm.CancelCommand, v => v.CancelButton));
                d(this.OneWayBind(this.ViewModel, vm => vm.Neurons, v => v.NeuronList.ItemsSource));
                d(this.Bind(this.ViewModel, vm => vm.SelectedNeuron, v => v.NeuronList.SelectedItem));
            });
        }
        
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DialogSelectNeuronViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DialogSelectNeuronViewModel), typeof(DialogSelectNeuronView), new PropertyMetadata(default(DialogSelectNeuronViewModel)));

        public DialogSelectNeuronViewModel ViewModel
        {
            get { return (DialogSelectNeuronViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
