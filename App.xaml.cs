using System.Reflection;
using System.Windows;
using Envelopes.Data;
using Ninject;

namespace Envelopes {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        protected override async void OnStartup(StartupEventArgs e) {
            await DataService.Instance.LoadApplicationData(); // Not worth doing anything till our data is loaded...for now

            var kernel = new StandardKernel();
            new Bootstrapper(kernel).Load();
            kernel.Load(Assembly.GetExecutingAssembly());
            var mainWindowPresenter = kernel.Get<IMainWindowPresenter>();
            mainWindowPresenter.MainWindow.Show();
        }
    }
}