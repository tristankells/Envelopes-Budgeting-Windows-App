using System.Windows;
using Envelopes.BudgetPage;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.TransactionsPage;

namespace Envelopes {
    public interface IMainWindowPresenter {
        public Window MainWindow { get; }
    }

    public class MainWindowPresenter : Presenter, IMainWindowPresenter {
        private readonly MainWindow view;
        private readonly IMainWindowViewModel viewModel;
        private readonly IDataService dataService;
        private readonly ITransactionsPagePresenter transactionsPagePresenter;
        private IBudgetPagePresenter budgetPagePresenter;

        public MainWindowPresenter(MainWindow view, 
            IMainWindowViewModel viewModel,
            ITransactionsPagePresenter transactionsPagePresenter,
            IBudgetPagePresenter budgetPagePresenter,
            IDataService dataService) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.transactionsPagePresenter = transactionsPagePresenter;
            this.budgetPagePresenter = budgetPagePresenter;
            this.dataService = dataService;

            BindEvents();
            BindCommands();
        }

        private void BindCommands() {
            viewModel.SaveBudgetCommand = new DelegateCommand(ExecuteSaveBudget, CanSaveBudget);
            viewModel.NavigateToBudgetPageCommand = new DelegateCommand(ExecuteNavigateToBudgetPage, CanNavigateToBudgetPage);
            viewModel.NavigateToTransactionsPageCommand = new DelegateCommand(ExecuteNavigateTransactionsPage, CanNavigateToTransactionsPage);
        }

        private bool CanNavigateToTransactionsPage() => !(viewModel.CurrentPage is TransactionsPageView);
    

        private void ExecuteNavigateTransactionsPage() {
            viewModel.CurrentPage = transactionsPagePresenter.GetPageView();
        }

        private bool CanNavigateToBudgetPage() => !(viewModel.CurrentPage is BudgetPageView);

        private void ExecuteNavigateToBudgetPage() {
            viewModel.CurrentPage = budgetPagePresenter.GetPageView();
        }
    

        private bool CanSaveBudget() => true;

        private async void ExecuteSaveBudget() {
            await dataService.SaveBudget();
        }

        private void BindEvents() {
            view.Loaded += View_Loaded;
            view.Closing += View_Closing;
        }

        private async void View_Closing(object sender, System.EventArgs e) {
             await dataService.SaveBudget();
        }

        private async void View_Loaded(object sender, RoutedEventArgs e) {
            await dataService.LoadApplicationData();
            viewModel.CurrentPage = transactionsPagePresenter.GetPageView();
        }

        public Window MainWindow => view;
    }
}