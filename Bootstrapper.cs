using Envelopes.Data;
using Envelopes.TransactionsPage;
using Envelopes.TransactionsPage.AccountsPane;
using Ninject.Modules;
using Ninject;

namespace Envelopes
{
    public class Bootstrapper : NinjectModule
    {
        private readonly StandardKernel kernel;
        public Bootstrapper(StandardKernel kernel) {
            this.kernel = kernel;
        }

        public override void Load() {
            // Main Window
            kernel.Bind<IMainWindowPresenter>().To<MainWindowPresenter>();
            kernel.Bind<IMainWindowViewModel>().To<MainWindowViewModel>();

            // Transactions Page
            kernel.Bind<ITransactionsPagePresenter>().To<TransactionsPagePresenter>();
            kernel.Bind<ITransactionsPageViewModel>().To<TransactionsPageViewModel>();
            kernel.Bind<IAccountsPanePresenter>().To<AccountsPanePresenter>();
            kernel.Bind<IAccountsPaneViewModel>().To<AccountsPaneViewModel>();
        }
    }
}
