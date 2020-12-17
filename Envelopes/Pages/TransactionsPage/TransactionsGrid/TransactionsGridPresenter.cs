using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Helpers;
using Envelopes.Models;
using Envelopes.Persistence.Importer;

namespace Envelopes.Pages.TransactionsPage.TransactionsGrid {
    public interface ITransactionsGridPresenter {
        ITransactionsGridView GetView();
    }

    public class TransactionsGridPresenter : Presenter, ITransactionsGridPresenter {
        private readonly IDataService dataService;
        private readonly INotificationService notificationService;
        private readonly ITransactionsImporter transactionsImporter;
        private readonly ITransactionsGridView view;
        private readonly ITransactionsGridViewModel viewModel;
        private Account? activeAccount; //The currently selected Account in the AccountsPane


        public TransactionsGridPresenter(ITransactionsGridView view,
            ITransactionsGridViewModel viewModel,
            IDataService dataService,
            INotificationService notificationService,
            ITransactionsImporter transactionsImporter) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.dataService = dataService;
            this.notificationService = notificationService;
            this.transactionsImporter = transactionsImporter;

            BindEvents();
            BindCommands();
        }

        public ITransactionsGridView GetView() => view;

        private void BindCommands() {
            viewModel.AddItemCommand = new AsyncCommand(ExecuteAddTransaction, CanExecuteAddTransaction);
            viewModel.DeleteItemCommand = new DelegateCommand(ExecuteDeleteTransaction, CanExecuteDeleteTransaction);
            viewModel.ImportTransactionsCommand = new AsyncCommand(ExecuteImportTransactions, CanImportTransactions);
        }

        private bool CanExecuteAddTransaction() => true;

        private async Task ExecuteAddTransaction() {
            AccountTransaction? newTransaction = await dataService.AddAccountTransaction(activeAccount);
            viewModel.AddItem(newTransaction);
        }

        private bool CanExecuteDeleteTransaction() => true;

        private void ExecuteDeleteTransaction() {
            AccountTransaction? selectedTransaction = viewModel.SelectedItem;

            if (dataService.RemoveAccountTransaction(selectedTransaction)) {
                viewModel.RemoveItem(selectedTransaction);
            }
        }

        private bool CanImportTransactions() => true;

        private async Task ExecuteImportTransactions() {
            IEnumerable<AccountTransaction>? transactions = await transactionsImporter.Import("", new AccountTransactionColumnMap()); // Passing blank values as using a proxy for now that has hard coded bank maps and file locations

            foreach (AccountTransaction transaction in transactions) {
                if (dataService.AddAccountTransaction(transaction)) {
                    viewModel.AddItem(transaction);
                }
            }

            await dataService.SaveBudget();
        }

        private void PopulateTransactionsList(bool isFiltered) {
            IEnumerable<AccountTransaction>? transactions = isFiltered ? dataService.AccountTransactions().Where(at => at.AccountId == activeAccount?.Id) : dataService.AccountTransactions();
            viewModel.AddRange(transactions);
        }

        #region Events

        private void BindEvents() {
            view.Loaded += OnViewLoaded;
            view.Unloaded += OnViewUnloaded;
            view.DataGridCellEditEnding += OnTransactionsGridCellEditEnding;
            notificationService.OnActiveAccountChanged += OnActiveAccountChanged;
            notificationService.OnShowAllTransactionsExecuted += OnShowAllTransactionsExecuted;
        }

        private static void OnTransactionsGridCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e) {
            switch ((e.Column as DataGridTextColumn)?.SortMemberPath) {
                case nameof(AccountTransaction.Outflow):
                    OnOutflowCellEditEnding((TextBox)e.EditingElement);
                    break;

                case nameof(AccountTransaction.Inflow):
                    OnInflowCellEditEnding((TextBox)e.EditingElement);
                    break;
            }
        }

        private static void OnOutflowCellEditEnding(TextBox textBox) {
            GridValidator.ParseAmountFromString(textBox.Text, out decimal newBudgetedAmount);
            textBox.Text = newBudgetedAmount.ToString(CultureInfo.CurrentUICulture);
        }

        private static void OnInflowCellEditEnding(TextBox textBox) {
            GridValidator.ParseAmountFromString(textBox.Text, out decimal newBudgetedAmount);
            textBox.Text = newBudgetedAmount.ToString(CultureInfo.CurrentUICulture);
        }

        private void OnShowAllTransactionsExecuted(object? sender, EventArgs e) {
            viewModel.ItemList.Clear();
            PopulateTransactionsList(false);
        }

        private void OnActiveAccountChanged(object? sender, EventArgs e) {
            if (sender is Account account) {
                activeAccount = account;
            }

            viewModel.ItemList.Clear();
            PopulateTransactionsList(true);
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e) {
            PopulateTransactionsList(false);
            PopulateCategories();
            PopulateAccounts();
        }

        private void PopulateCategories() {
            IEnumerable<Category>? categories = dataService.Categories();
            foreach (var category in categories) {
                viewModel.Categories.Add(category);
            }

            viewModel.Categories.Add(new Category {
                Name = "Inflow",
                Id = 999
            });
        }

        private void PopulateAccounts() {
            IEnumerable<Account>? accounts = dataService.Accounts();
            foreach (var account in accounts) {
                viewModel.Accounts.Add(account);
            }
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e) {
            view.Loaded -= OnViewLoaded;
            view.Unloaded -= OnViewUnloaded;
        }

        #endregion
    }
}