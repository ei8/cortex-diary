using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Reactive.Views.Wpf
{
    public partial class PresynapticView : UserControl, IViewFor<PresynapticViewModel>
    {
        public PresynapticView()
        {
            InitializeComponent();
            this.OneWayBind(ViewModel, vm => vm.Data, v => v.PresynapticName.Text);
        }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (PresynapticViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(PresynapticViewModel), typeof(PresynapticView), new PropertyMetadata(default(PresynapticViewModel)));

        public PresynapticViewModel ViewModel
        {
            get { return (PresynapticViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
