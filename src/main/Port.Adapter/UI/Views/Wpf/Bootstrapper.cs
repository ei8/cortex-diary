using ReactiveUI;
using Splat;
using works.ei8.Cortex.Diary.Application.Dependency;
using works.ei8.Cortex.Diary.Application.Neurons;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;
using works.ei8.Cortex.Diary.Domain.Model.Origin;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Origins;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.RequestProvider;
using works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Settings;
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
            Locator.CurrentMutable.RegisterLazySingleton(() => new SettingsServiceImplementation(), typeof(ISettingsServiceImplementation));
            Locator.CurrentMutable.RegisterLazySingleton(() => new DependencyService(), typeof(IDependencyService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new SettingsService(), typeof(ISettingsService));            
            Locator.CurrentMutable.RegisterLazySingleton(() => new RequestProvider(), typeof(IRequestProvider));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronGraphQueryClient(), typeof(INeuronGraphQueryClient));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronQueryService(), typeof(INeuronQueryService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronService(), typeof(INeuronService)); // DEL:
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronClient(), typeof(INeuronClient));
            Locator.CurrentMutable.RegisterLazySingleton(() => new NeuronApplicationService(), typeof(INeuronApplicationService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new OriginsCacheService(), typeof(IOriginsCacheService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new OriginService(), typeof(IOriginService));
            Locator.CurrentMutable.RegisterLazySingleton(() => new StatusService(), typeof(IStatusService));

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
