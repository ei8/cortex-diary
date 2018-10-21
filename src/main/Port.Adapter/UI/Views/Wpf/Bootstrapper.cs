using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Docking;
using works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons;

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
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronService(), typeof(INeuronService));
            // TODO: Locator.CurrentMutable.Register(() => new NeuronGraphView(), typeof(IViewFor<NeuronGraphPaneViewModel>));
            //Locator.CurrentMutable.Register(() => new PresynapticView(), typeof(IViewFor<PresynapticViewModel>));
            //Locator.CurrentMutable.Register(() => new PostsynapticView(), typeof(IViewFor<PostsynapticViewModel>));
            Locator.CurrentMutable.RegisterLazySingleton(() => new SelectionService(), typeof(IExtendedSelectionService));
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
