/*
    This file is part of the d# project.
    Copyright (c) 2016-2018 ei8
    Authors: ei8
     This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    EI8. EI8 DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS
     This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
    https://github.com/ei8/cortex-diary/blob/master/LICENSE
     The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
     You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the d# software without
    disclosing the source code of your own applications.
     For more information, please contact ei8 at this address: 
     support@ei8.works
 */

using ReactiveUI;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogSelectNeuronView.xaml
    /// </summary>
    public partial class DialogSelectNeuronsView : UserControl, IViewFor<DialogSelectNeuronsViewModel>
    {
        public DialogSelectNeuronsView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .Where(x => x != null)
                    .Subscribe(x => this.ViewModel = (DialogSelectNeuronsViewModel)x);

                this.WhenAnyValue(view => view.ViewModel)
                    .Select(cmd => Unit.Default)
                    .InvokeCommand(this.ViewModel.ReloadCommand);

                d(this.Bind(this.ViewModel, vm => vm.StatusMessage, v => v.StatusMessageLabel.Content));
                d(this.BindCommand(this.ViewModel, vm => vm.ReloadCommand, v => v.ReloadButton));
                d(this.BindCommand(this.ViewModel, vm => vm.SelectCommand, v => v.SelectButton));
                d(this.OneWayBind(this.ViewModel, vm => vm.Neurons, v => v.NeuronList.ItemsSource));
                d(this.OneWayBind(this.ViewModel, vm => vm.AllowMultiSelect, v => v.NeuronList.SelectionMode, p => p ? SelectionMode.Extended : SelectionMode.Single));

                Observable.FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(
                    ev => this.NeuronList.SelectionChanged += ev,
                    ev => this.NeuronList.SelectionChanged -= ev
                    ).Subscribe(ep => this.ViewModel.SelectedNeurons = ((ListView)ep.Sender).SelectedItems);
            });
        }
        
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (DialogSelectNeuronsViewModel)value; }
        }

        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(DialogSelectNeuronsViewModel), typeof(DialogSelectNeuronsView), new PropertyMetadata(default(DialogSelectNeuronsViewModel)));

        public DialogSelectNeuronsViewModel ViewModel
        {
            get { return (DialogSelectNeuronsViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
    }
}
