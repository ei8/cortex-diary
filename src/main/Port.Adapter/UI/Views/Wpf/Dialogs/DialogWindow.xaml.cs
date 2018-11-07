using ReactiveUI;
using System.Windows;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogWindow.
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public partial class DialogWindow : Window, IViewFor<DialogViewModelBase>
    {
        public DialogWindow()
        {
            InitializeComponent();
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DialogViewModelBase)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DialogViewModelBase), typeof(DialogWindow), new PropertyMetadata(default(DialogViewModelBase)));

        public DialogViewModelBase ViewModel
        {
            get { return (DialogViewModelBase)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
