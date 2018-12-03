using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.Views.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Process unhandled exception do stuff below
            File.AppendAllText("Exception.log", $"{DateTime.Now.ToString()}: {e.Exception.ToString()}");
            // Prevent default unhandled exception processing
            e.Handled = true;
        }
    }
}
