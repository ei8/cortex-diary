using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public partial class NeuronGraphView : UserControl, IViewFor<NeuronGraphPaneViewModel>
    {
        public NeuronGraphView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.DataContext)
                .Where(x => x != null)
                .Subscribe(x => this.ViewModel = (NeuronGraphPaneViewModel)x);

            this.WhenActivated(d =>
           {
               d(this.Bind(this.ViewModel, vm => vm.AvatarUrl, v => v.AvatarUrl.Text));
               d(this.Bind(this.ViewModel, vm => vm.AuthorId, v => v.AuthorId.Text));
               d(this.Bind(this.ViewModel, vm => vm.StatusMessage, v => v.StatusMessage.Content));

               d(this.BindCommand(this.ViewModel, vm => vm.ReloadCommand, v => v.Reload));
               d(this.BindCommand(this.ViewModel, vm => vm.AddCommand, v => v.Add));
               d(this.BindCommand(this.ViewModel, vm => vm.SetAuthorIdCommand, v => v.SetAuthorId));
           });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (NeuronGraphPaneViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(NeuronGraphPaneViewModel), typeof(NeuronGraphView), new PropertyMetadata(default(NeuronGraphPaneViewModel)));

        public NeuronGraphPaneViewModel ViewModel
        {
            get { return (NeuronGraphPaneViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
