#nullable enable
using System;
using Envelopes.Common;
using Envelopes.Data;
using System.Windows;
using Envelopes.Pages.BudgetPage;
using Envelopes.Pages.TransactionsPage;

namespace Envelopes {
    public interface IMainWindowPresenter {
        public Window MainWindow { get; }
    }

    public class MainWindowPresenter : Presenter, IMainWindowPresenter {
        private readonly MainWindow view;
        private readonly IMainWindowViewModel viewModel;
        private readonly IDataService dataService;
        private readonly ITransactionsPagePresenter transactionsPagePresenter;
        private readonly IBudgetPagePresenter budgetPagePresenter;
        private INotificationService notificationService;

        public MainWindowPresenter(MainWindow view,
            IMainWindowViewModel viewModel,
            ITransactionsPagePresenter transactionsPagePresenter,
            IBudgetPagePresenter budgetPagePresenter,
            IDataService dataService,
            INotificationService notificationService) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.transactionsPagePresenter = transactionsPagePresenter;
            this.budgetPagePresenter = budgetPagePresenter;
            this.dataService = dataService;
            this.notificationService = notificationService;

            BindEvents();
            BindCommands();
        }

        private void UpdateRemainingBalanceToBudget() {
            viewModel.RemainingBalanceToBudget = dataService.GetRemainingAccountBalanceToBudget();
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
            view.Loaded += OnViewLoaded;
            notificationService.OnCategoryBudgetedChanged += OnCategoryBudgetedChanged;
            notificationService.OnTransactionBalanceChanged += OnTransactionsBalanceChanged;
        }

        private void OnTransactionsBalanceChanged(object? sender, EventArgs e) {
            UpdateRemainingBalanceToBudget();
        }

        private void OnCategoryBudgetedChanged(object? sender, System.EventArgs e) {
            UpdateRemainingBalanceToBudget();
        }

        private async void OnViewLoaded(object sender, RoutedEventArgs e) {
            await dataService.LoadApplicationData();
            viewModel.CurrentPage = transactionsPagePresenter.GetPageView();
            UpdateRemainingBalanceToBudget();
        }

        public Window MainWindow => view;
    }
}