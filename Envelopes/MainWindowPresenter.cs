#nullable enable
using System;
using System.Windows;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Pages.BudgetPage;
using Envelopes.Pages.TransactionsPage;

namespace Envelopes {
    public interface IMainWindowPresenter {
        public Window MainWindow { get; }
    }

    public class MainWindowPresenter : Presenter, IMainWindowPresenter {
        private readonly IBudgetPagePresenter budgetPagePresenter;
        private readonly IDataService dataService;
        private readonly ITransactionsPagePresenter transactionsPagePresenter;
        private readonly MainWindow view;
        private readonly IMainWindowViewModel viewModel;
        private readonly INotificationService notificationService;

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

        public Window MainWindow => view;

        private void UpdateBalanceHeader() {
            viewModel.TotalBalance = dataService.GetTotalInflow();
            viewModel.TotalBudgeted = dataService.GetTotalBudgeted();
            viewModel.RemainingBalanceToBudget = dataService.GetRemainingAccountBalanceToBudget();
        }

        private void BindCommands() {
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

        private void BindEvents() {
            view.Loaded += OnViewLoaded;
            notificationService.OnCategoryBudgetedChanged += OnCategoryBudgetedChanged;
            notificationService.OnTransactionBalanceChanged += OnTransactionsBalanceChanged;
        }

        private void OnTransactionsBalanceChanged(object? sender, EventArgs e) {
            UpdateBalanceHeader();
        }

        private void OnCategoryBudgetedChanged(object? sender, EventArgs e) {
            UpdateBalanceHeader();
        }

        private async void OnViewLoaded(object sender, RoutedEventArgs e) {
            await dataService.LoadApplicationData();
            viewModel.CurrentPage = transactionsPagePresenter.GetPageView();
            UpdateBalanceHeader();
        }
    }
}