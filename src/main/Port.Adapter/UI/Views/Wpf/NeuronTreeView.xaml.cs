using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public partial class NeuronTreeView : UserControl, IViewFor<NeuronTreePaneViewModel>
    {
        public NeuronTreeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .Where(x => x != null)
                    .Subscribe(x => this.ViewModel = (NeuronTreePaneViewModel)x);

                d(this.Bind(this.ViewModel, vm => vm.AvatarUrl, v => v.AvatarUrl.Text));
                d(this.Bind(this.ViewModel, vm => vm.AuthorName, v => v.AuthorName.Content));
                d(this.Bind(this.ViewModel, vm => vm.StatusMessage, v => v.StatusMessage.Content));

                d(this.BindCommand(this.ViewModel, vm => vm.ReloadCommand, v => v.Reload));
                d(this.BindCommand(this.ViewModel, vm => vm.AddCommand, v => v.Add));
                d(this.BindCommand(this.ViewModel, vm => vm.SetAuthorCommand, v => v.SetAuthor));
           });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (NeuronTreePaneViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(NeuronTreePaneViewModel), typeof(NeuronTreeView), new PropertyMetadata(default(NeuronTreePaneViewModel)));

        public NeuronTreePaneViewModel ViewModel
        {
            get { return (NeuronTreePaneViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
