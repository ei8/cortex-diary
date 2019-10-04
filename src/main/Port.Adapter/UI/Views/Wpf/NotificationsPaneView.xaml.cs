using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Notifications;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    /// <summary>
    /// Interaction logic for NotificationPaneView.xaml
    /// </summary>
    public partial class NotificationsPaneView : UserControl, IViewFor<NotificationsPaneViewModel>
    {
        public NotificationsPaneView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .Where(x => x != null)
                    .Subscribe(x => this.ViewModel = (NotificationsPaneViewModel)x);

                d(this.Bind(this.ViewModel, vm => vm.AvatarUrl, v => v.AvatarUrl.Text));
                d(this.Bind(this.ViewModel, vm => vm.LayerName, v => v.LayerName.Content));
                d(this.Bind(this.ViewModel, vm => vm.StatusMessage, v => v.StatusMessage.Content));
                d(this.OneWayBind(this.ViewModel, vm => vm.Notifications, v => v.NotificationsListBox.ItemsSource));

                d(this.Bind(this.ViewModel, vm => vm.SelectedNotification, v => v.NotificationsListBox.SelectedItem));

                d(this.BindCommand(this.ViewModel, vm => vm.LoadCommand, v => v.Load));
                d(this.BindCommand(this.ViewModel, vm => vm.MoreCommand, v => v.More));
                d(this.BindCommand(this.ViewModel, vm => vm.SetLayerCommand, v => v.SetLayer));

                d(this.OneWayBind(this.ViewModel, vm => vm.Loading, v => v.Progress.Visibility, l => l ? Visibility.Visible : Visibility.Collapsed));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (NotificationsPaneViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(NotificationsPaneViewModel), typeof(NotificationsPaneView), new PropertyMetadata(default(NotificationsPaneViewModel)));

        public NotificationsPaneViewModel ViewModel
        {
            get { return (NotificationsPaneViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
