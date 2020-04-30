using ReactiveUI;
using System.Windows;
using System.Windows.Controls;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Notifications;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    /// <summary>
    /// Interaction logic for NotificationView.xaml
    /// </summary>
    public partial class NotificationView : UserControl, IViewFor<NotificationViewModel>
    {
        public NotificationView()
        {
            InitializeComponent();

            this.OneWayBind(this.ViewModel, vm => vm.Timestamp, v => v.TimestampText.Text);
            this.OneWayBind(this.ViewModel, vm => vm.Author, v => v.AuthorText.Text);
            this.OneWayBind(this.ViewModel, vm => vm.Type, v => v.TypeText.Text);
            this.OneWayBind(this.ViewModel, vm => vm.Tag, v => v.TagText.Text);
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (NotificationViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(NotificationViewModel), typeof(NotificationView), new PropertyMetadata(default(NotificationViewModel)));

        public NotificationViewModel ViewModel
        {
            get { return (NotificationViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
