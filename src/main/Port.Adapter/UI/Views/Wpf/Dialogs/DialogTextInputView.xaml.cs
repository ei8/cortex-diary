using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    public partial class DialogTextInputView : UserControl, IViewFor<DialogTextInputViewModel>
    {
        public DialogTextInputView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.DataContext)
                .Where(x => x != null)
                .Subscribe(x => this.ViewModel = (DialogTextInputViewModel) x);

            this.WhenActivated(d =>
            {
                d(this.BindCommand(this.ViewModel, vm => vm.OkCommand, v => v.OkButton));
                d(this.BindCommand(this.ViewModel, vm => vm.CancelCommand, v => v.CancelButton));
                d(this.Bind(this.ViewModel, vm => vm.TextInput, v => v.InputBox.Text));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DialogTextInputViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DialogTextInputViewModel), typeof(DialogTextInputView), new PropertyMetadata(default(DialogTextInputViewModel)));

        public DialogTextInputViewModel ViewModel
        {
            get { return (DialogTextInputViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
