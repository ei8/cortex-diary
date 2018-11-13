﻿using DynamicData.Binding;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogYesNoView.xaml
    /// https://www.c-sharpcorner.com/article/dialogs-in-wpf-mvvm/
    /// </summary>
    public partial class DialogYesNoView : UserControl, IViewFor<DialogYesNoViewModel>
    {
        public DialogYesNoView()
        {
            InitializeComponent();

            this.WhenAnyValue(x => x.DataContext)
                .Where(x => x != null)
                .Subscribe(x => this.ViewModel = (DialogYesNoViewModel) x);

            this.WhenActivated(d =>
            {
                d(this.BindCommand(this.ViewModel, vm => vm.YesCommand, v => v.YesButton));
                d(this.BindCommand(this.ViewModel, vm => vm.NoCommand, v => v.NoButton));
                d(this.Bind(this.ViewModel, vm => vm.Message, v => v.MessageLabel.Text));
            });
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DialogYesNoViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DialogYesNoViewModel), typeof(DialogYesNoView), new PropertyMetadata(default(DialogYesNoViewModel)));

        public DialogYesNoViewModel ViewModel
        {
            get { return (DialogYesNoViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
