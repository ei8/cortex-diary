using ReactiveUI;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public partial class PostsynapticView : UserControl, IViewFor<PostsynapticViewModel>
    {
        public PostsynapticView()
        {
            InitializeComponent();
            this.OneWayBind(ViewModel, vm => vm.Tag, v => v.PostsynapticName.Text);
        }
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (PostsynapticViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(PostsynapticViewModel), typeof(PostsynapticView), new PropertyMetadata(default(PostsynapticViewModel)));

        public PostsynapticViewModel ViewModel
        {
            get { return (PostsynapticViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
