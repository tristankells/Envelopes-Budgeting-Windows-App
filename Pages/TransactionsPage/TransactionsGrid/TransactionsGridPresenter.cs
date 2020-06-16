#nullable enable
using System;
using System.Windows;
using Envelopes.Common;
using Envelopes.Data;

namespace Envelopes.Pages.TransactionsPage.TransactionsGrid {
    public interface ITransactionsGridPresenter {
        TransactionsGridView GetView();
    }

    public class TransactionsGridPresenter : Presenter, ITransactionsGridPresenter {
        private readonly TransactionsGridView view;
        private readonly ITransactionsGridViewModel viewModel;
        private readonly IDataService dataService;
        private readonly INotificationService notificationService;

        public TransactionsGridPresenter(TransactionsGridView view,
            ITransactionsGridViewModel viewModel,
            IDataService dataService,
            INotificationService notificationService) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.dataService = dataService;
            this.notificationService = notificationService;

            BindEvents();
            BindCommands();
        }

        #region Events

        private void BindEvents() {
            view.Loaded += OnViewLoaded;
            view.Unloaded += OnViewUnloaded;
            notificationService.OnActiveAccountChanged += OnActiveAccountChanged;
        }

        private void OnActiveAccountChanged(object? sender, EventArgs e) {
            viewModel.ItemList.Clear();
            PopulateTransactionsList();
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e) {
            PopulateTransactionsList();
            PopulateCategories();
            PopulateAccounts();
        }

        private void PopulateCategories() {
            var categories = dataService.GetCategories();
            foreach (var category in categories) {
                viewModel.Categories.Add(category);
            }
        }

        private void PopulateAccounts() {
            var accounts = dataService.GetAccounts();
            foreach (var account in accounts) {
                viewModel.Accounts.Add(account);
            }
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e) {
            view.Loaded -= OnViewLoaded;
            view.Unloaded -= OnViewUnloaded;
        }

        #endregion

        public TransactionsGridView GetView() => view;

        private void BindCommands() {
            viewModel.AddItemCommand = new DelegateCommand(ExecuteAddTransaction, CanExecuteAddTransaction);
            viewModel.DeleteItemCommand = new DelegateCommand(ExecuteDeleteTransaction, CanExecuteDeleteTransaction);
        }

        private bool CanExecuteAddTransaction() => true;

        private void ExecuteAddTransaction() {
            var newTransaction = dataService.AddAccountTransaction();
            viewModel.AddItem(newTransaction);
        }

        private bool CanExecuteDeleteTransaction() => true;

        private void ExecuteDeleteTransaction() {
            var selectedTransaction = viewModel.SelectedItem;

            if (dataService.RemoveAccountTransaction(selectedTransaction)) {
                viewModel.RemoveItem(selectedTransaction);
            }
        }

        private void PopulateTransactionsList() {
            var transactions = dataService.GetAccountTransactionsFilteredByActiveAccount();
            foreach (var account in transactions) {
                viewModel.AddItem(account);
            }
        }
    }
}