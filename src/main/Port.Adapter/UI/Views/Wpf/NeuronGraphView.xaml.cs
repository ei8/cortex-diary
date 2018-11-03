﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using Splat;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public partial class NeuronGraphView : UserControl, IViewFor<NeuronGraphPaneViewModel>
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
               d(this.Bind(this.ViewModel, vm => vm.AvatarUrl, v => v.AvatarUrl.Text));

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
