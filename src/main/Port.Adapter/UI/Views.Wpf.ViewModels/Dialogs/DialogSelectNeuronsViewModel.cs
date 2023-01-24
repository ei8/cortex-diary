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

using DynamicData;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Port.Adapter.UI.Common;
using ei8.Cortex.Library.Common;
using ReactiveUI;
using Splat;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs
{
    public class DialogSelectNeuronsViewModel : DialogViewModelBase, IDisposable
    {
        private readonly string avatarUrl;
        private readonly INeuronQueryService neuronQueryService;
        private readonly ReadOnlyObservableCollection<UINeuron> neurons;
        private readonly IDisposable cleanUp;
        
        public DialogSelectNeuronsViewModel(string message, string avatarUrl, bool allowMultiSelect, INeuronQueryService neuronQueryService = null) : 
            base(message)
        {
            this.avatarUrl = avatarUrl;
            this.AllowMultiSelect = allowMultiSelect;
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            var list = new SourceList<UINeuron>();
            this.ReloadCommand = ReactiveCommand.Create(async() => await this.OnReloadClicked(list));
            this.SelectCommand = ReactiveCommand.Create(this.OnSelectedClicked);
            this.UserDialogResult = null;

            this.cleanUp = list.AsObservableList().Connect()
                .Bind(out this.neurons)
                .DisposeMany()
                .Subscribe();
        }

        private async Task OnReloadClicked(SourceList<UINeuron> list)
        {
            try
            {
                // TODO: list.Clear();
                //(await this.neuronQueryService.GetNeurons(this.avatarUrl, new NeuronQuery()))
                //    .Neurons.ToList().ForEach(n => list.Add(new UINeuron(n)));                
                //this.StatusMessage = "Reload successful.";
            }
            catch (Exception ex)
            {
                this.StatusMessage = ex.Message;
            }
        }

        private void OnSelectedClicked()
        {
            if (this.selectedNeurons != null)
            {
                this.UserDialogResult = this.selectedNeurons.Cast<UINeuron>();
                this.DialogResult = true;
            }
            else
                this.StatusMessage = "No Neuron selected.";
        }
        
        public ReadOnlyObservableCollection<UINeuron> Neurons => this.neurons;

        public ReactiveCommand<Unit, Task> ReloadCommand { get; }

        public ReactiveCommand<Unit, Unit> SelectCommand { get; }

        private IList selectedNeurons;

        public IList SelectedNeurons
        {
            get => this.selectedNeurons;
            set => this.RaiseAndSetIfChanged(ref this.selectedNeurons, value);
        }

        private bool allowMultiSelect;

        public bool AllowMultiSelect
        {
            get => this.allowMultiSelect;
            set => this.RaiseAndSetIfChanged(ref this.allowMultiSelect, value);
        }

        private string statusMessage;

        public string StatusMessage
        {
            get => this.statusMessage;
            set => this.RaiseAndSetIfChanged(ref this.statusMessage, value);
        }

        public void Dispose()
        {
            this.cleanUp.Dispose();
        }
    }
}
