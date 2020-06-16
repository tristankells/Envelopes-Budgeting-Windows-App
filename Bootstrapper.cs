using Envelopes.Data;
using Envelopes.Data.Persistence;
using Envelopes.Helpers;
using Envelopes.Pages.BudgetPage;
using Envelopes.Pages.BudgetPage.CategoriesGrid;
using Envelopes.Pages.TransactionsPage;
using Envelopes.Pages.TransactionsPage.AccountsPane;
using Envelopes.Pages.TransactionsPage.TransactionsGrid;
using Ninject.Modules;
using Ninject;

namespace Envelopes {
    public class Bootstrapper : NinjectModule {
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
            kernel.Bind<ITransactionsPageViewModelBase>().To<TransactionsPageViewModelBase>();
            kernel.Bind<IAccountsPaneView>().To<AccountsPaneView>();
            kernel.Bind<IAccountsPanePresenter>().To<AccountsPanePresenter>();
            kernel.Bind<IAccountsPaneViewModel>().To<AccountsPaneViewModel>();
            kernel.Bind<ITransactionsGridPresenter>().To<TransactionsGridPresenter>();
            kernel.Bind<ITransactionsGridViewModel>().To<TransactionsGridViewModel>();

            // Budget Page
            kernel.Bind<IBudgetPagePresenter>().To<BudgetPagePresenter>();
            kernel.Bind<ICategoriesGridViewModelBase>().To<CategoriesGridViewModel>();
            kernel.Bind<ICategoriesGridPresenter>().To<CategoriesGridPresenter>();

            // Data
            kernel.Bind<IPersistenceService>().To<ExcelPersistenceService>();
            kernel.Bind<IGridValidator>().To<GridValidator>();
            kernel.Bind<IIdentifierService>().To<IdentifierService>().InSingletonScope();
            kernel.Bind<IDataService>().To<DataService>().InSingletonScope();
            kernel.Bind<INotificationService>().To<NotificationService>().InSingletonScope();
            kernel.Bind<IMessageBoxWrapper>().To<MessageBoxWrapper>().InSingletonScope();
        }
    }
}