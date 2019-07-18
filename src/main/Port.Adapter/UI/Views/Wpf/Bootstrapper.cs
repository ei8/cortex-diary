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
using Splat;
using works.ei8.Cortex.Diary.Application.Dependency;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Application.OpenUrl;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Origin;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Origins;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.RequestProvider;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Dialogs;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf.Dialogs;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    public class Bootstrapper
    {
        public Bootstrapper()
        {
            this.ConfigureServices();
        }

        private void ConfigureServices()
        {
            Locator.CurrentMutable.Register(() => new MainWindow(), typeof(IViewFor<Workspace>));
            Locator.CurrentMutable.Register(() => new OpenUrlService(), typeof(IOpenUrlService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new DialogService(), typeof(IDialogService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new SettingsServiceImplementation(), typeof(ISettingsServiceImplementation));
            Locator.CurrentMutable.RegisterLazySingleton(() => new DependencyService(), typeof(IDependencyService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new SettingsService(), typeof(ISettingsService));            
            Locator.CurrentMutable.RegisterLazySingleton(() => new RequestProvider(), typeof(IRequestProvider));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronGraphQueryClient(), typeof(INeuronGraphQueryClient));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronQueryService(), typeof(INeuronQueryService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronClient(), typeof(INeuronClient));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronApplicationService(), typeof(INeuronApplicationService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new TerminalClient(), typeof(ITerminalClient));
            Locator.CurrentMutable.RegisterLazySingleton(() => new TerminalApplicationService(), typeof(ITerminalApplicationService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new OriginsCacheService(), typeof(IOriginsCacheService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new OriginService(), typeof(IOriginService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new StatusService(), typeof(IStatusService));

            // TODO: Locator.CurrentMutable.Register(() => new NeuronGraphView(), typeof(IViewFor<NeuronGraphPaneViewModel>));
            //Locator.CurrentMutable.Register(() => new PresynapticView(), typeof(IViewFor<PresynapticViewModel>));
            //Locator.CurrentMutable.Register(() => new PostsynapticView(), typeof(IViewFor<PostsynapticViewModel>));            
            Locator.CurrentMutable.RegisterLazySingleton(() => new SelectionService(), typeof(IExtendedSelectionService), SelectionContract.Select.ToString());
            Locator.CurrentMutable.RegisterLazySingleton(() => new SelectionService(), typeof(IExtendedSelectionService), SelectionContract.Highlight.ToString());
        }

        internal void Run()
        {
            var viewModel = new Workspace();
            Locator.CurrentMutable.RegisterConstant(viewModel);
            var view = ViewLocator.Current.ResolveView(viewModel);
            view.ViewModel = viewModel;
            ((MainWindow)view).Show();
        }
    }
}
