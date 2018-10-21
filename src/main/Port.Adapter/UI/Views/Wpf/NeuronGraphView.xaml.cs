using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using Splat;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public partial class NeuronGraphView : UserControl, IViewFor<NeuronGraphPaneViewModel> // TODO: Is IViewFor unnecessary?
    {
        public NeuronGraphView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.DataContext)
                .Subscribe(x =>
                {
                    if (x != null)
                    {
                        this.ViewModel = (NeuronGraphPaneViewModel) x;
                        this.DataContext = this.ViewModel;
                    }
                });

           this.WhenActivated(d =>
           {
               //DEL: d(this.Bind(ViewModel, vm => vm.PresynapticName, v => v.PresynapticName.Text));
               //d(this.BindCommand(ViewModel, vm => vm.AddPresynaptic, v => v.AddPresynaptic));
               //d(this.Bind(ViewModel, vm => vm.PostsynapticName, v => v.PostsynapticName.Text));
               //d(this.BindCommand(ViewModel, vm => vm.AddPostsynaptic, v => v.AddPostsynaptic));
               //d(this.WhenAnyValue(x => x.FamilyTree.SelectedItem).BindTo(this, x => x.ViewModel.SelectedItem));
               //d(this.BindCommand(ViewModel, vm => vm.Collapse, v => v.Collapse));

               //d(this.Bind(this.ViewModel, vm => vm.ModName, v => v.ModName.Text));
               //d(this.BindCommand(ViewModel, vm => vm.ChangeName, v => v.ChangeName));

               d(this.BindCommand(this.ViewModel, vm => vm.ReloadCommand, v => v.Reload));
               d(this.BindCommand(this.ViewModel, vm => vm.AddCommand, v => v.Add));
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
