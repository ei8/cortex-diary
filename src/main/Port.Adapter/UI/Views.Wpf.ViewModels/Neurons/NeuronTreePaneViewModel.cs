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
using DynamicData.Binding;
using ei8.Cortex.Diary.Application.Neurons;
using ei8.Cortex.Diary.Application.Notifications;
// TODO: using ei8.Cortex.Diary.Domain.Model.Origin;
using ei8.Cortex.Diary.Port.Adapter.UI.Common;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Peripheral;
using ei8.Cortex.Library.Client;
using ei8.Cortex.Library.Common;
using Nancy.Helpers;
using neurUL.Common.Domain.Model;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    public class NeuronTreePaneViewModel : PaneViewModel, IAvatarViewer, IDisposable
    {
        private readonly ReadOnlyObservableCollection<NeuronViewModelBase> children;
        private readonly IDisposable cleanUp;
        private readonly IDialogService dialogService;
        private readonly INeuronApplicationService neuronApplicationService;
        private readonly INeuronQueryService neuronQueryService;
        private readonly INotificationApplicationService notificationApplicationService;
        private readonly IStatusService statusService;
        private string queryUrl;

        public NeuronTreePaneViewModel(INeuronApplicationService neuronApplicationService = null, INeuronQueryService neuronQueryService = null, INotificationApplicationService notificationApplicationService = null,
            IStatusService statusService = null, IDialogService dialogService = null)
        {
            this.dialogService = dialogService ?? Locator.Current.GetService<IDialogService>();
            this.neuronApplicationService = neuronApplicationService ?? Locator.Current.GetService<INeuronApplicationService>();
            this.neuronQueryService = neuronQueryService ?? Locator.Current.GetService<INeuronQueryService>();
            this.notificationApplicationService = notificationApplicationService ?? Locator.Current.GetService<INotificationApplicationService>();
            this.statusService = statusService ?? Locator.Current.GetService<IStatusService>();

            this.statusService.WhenPropertyChanged(s => s.Message)
                .Subscribe(s => this.StatusMessage = s.Sender.Message);

            bool DefaultPredicate(Node<UINeuron, int> node) => node.IsRoot;
            var cache = new SourceCache<UINeuron, int>(x => x.UIId);

            this.AddCommand = ReactiveCommand.Create<object>(async (parameter) => await this.OnAddClicked(cache, parameter));
            this.SetRegionCommand = ReactiveCommand.Create<object>(async (parameter) => await this.OnSetRegionIdClicked(parameter));
            this.ReloadCommand = ReactiveCommand.Create(async () => await this.OnReloadClicked(cache));

            this.cleanUp = cache.AsObservableCache().Connect()
                .TransformToTree(child => child.CentralUIId, Observable.Return((Func<Node<UINeuron, int>, bool>)DefaultPredicate))
                .Transform(e =>
                    e.Item.Type == RelativeType.Postsynaptic ?
                    (NeuronViewModelBase)(new PostsynapticViewModel(this, e.Item.Tag, e, cache)) :
                    (NeuronViewModelBase)(new PresynapticViewModel(this, e.Item.Tag, e, cache)))
                .Bind(out this.children)
                .DisposeMany()
                .Subscribe();

            this.Target = null;
            this.Loading = false;
            this.IconSourcePath = @"pack://application:,,,/d23-wpf;component/images/hierarchy.ico";
        }

        private async Task OnAddClicked(SourceCache<UINeuron, int> cache, object owner)
        {
        // TODO:   var n = await ViewModels.Helper.CreateNeuron(
        //        async () =>
        //        {
        //            string result = string.Empty;

        //            if (
        //                (await this.dialogService.ShowDialogTextInput(
        //                "Enter Neuron Tag: ",
        //                owner,
        //                out string r
        //                )).GetValueOrDefault()
        //                )
        //                result = r;

        //            return result;
        //        },
        //        owner,
        //        this.dialogService,
        //        this.neuronQueryService,
        //        this.neuronApplicationService,
        //        this.notificationApplicationService,
        //        this.statusService,
        //        this.AvatarUrl,
        //        this.RegionId
        //        );

        //    if (n != null)
        //        cache.AddOrUpdate(n);
        }

        private async Task OnSetRegionIdClicked(object parameter)
        {
            await Helper.SetStatusOnComplete(async () =>
                {
                    bool stat = false;

                    if ((await this.dialogService.ShowDialogSelectNeurons("Select Region Neuron", this.AvatarUrl, parameter, false, out IEnumerable<UINeuron> result)).GetValueOrDefault())
                    {
                        this.RegionName = result.First().Tag;
                        this.RegionId = result.First().Id;
                        stat = true;
                    }
                    else
                        this.InitRegion();

                    return stat;
                },
                "Region set successfully.",
                this.statusService,
                "Region set cancelled."
                );
        }

        private void InitRegion()
        {
            this.RegionName = "[Base]";
            this.RegionId = string.Empty;
        }

        private async Task OnReloadClicked(SourceCache<UINeuron, int> cache)
        {
            this.Loading = true;

            // TODO: await Helper.SetStatusOnComplete(async () =>
            //    {
            //        cache.Clear();
            //        var relatives = new List<UINeuron>();
            //        (await this.neuronQueryService.SendQuery(this.QueryUrl))
            //            .Neurons
            //            .ToList().ForEach(n => relatives.Add(new UINeuron(n))
            //        );
            //        this.originService.Save(this.AvatarUrl);
            //        relatives.FillUIIds(null);
            //        cache.AddOrUpdate(relatives);
            //        this.InitRegion();
            //        return true;
            //    },
            //    "Reload successful.",
            //    this.statusService
            //);

            this.Loading = false;
        }

        public ReactiveCommand<object, Unit> AddCommand { get; }

        [Reactive]
        public string RegionId { get; set; }

        [Reactive]
        public string RegionName { get; set; }

        public ReactiveCommand<object, Unit> SetRegionCommand { get; }

        public string QueryUrl
        {
            get => this.queryUrl;
            set
            {
                this.RaiseAndSetIfChanged(ref this.queryUrl, value);
                this.AvatarUrl = Library.Client.QueryUrl.TryParse(value, out QueryUrl rurl) ?
                    rurl.AvatarUrl : 
                    string.Empty;
            }
        }

        [Reactive]
        public string AvatarUrl { get; set; }
        
        [Reactive]
        public bool Loading { get; set; }

        [Reactive]
        public EditorNeuronData Target { get; set; }

        public ReadOnlyObservableCollection<NeuronViewModelBase> Children => this.children;

        public ReactiveCommand<Unit, Task> ReloadCommand { get; }

        [Reactive]
        public string StatusMessage { get; set; }
        
        public void Dispose()
        {
            this.cleanUp.Dispose();
        }
    }
}
