#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Models;
using Envelopes.Presentation;
using OfficeOpenXml;

namespace Envelopes.Pages.TransactionsPage.AccountsPane {
    public interface IAccountsPanePresenter {
        public IAccountsPaneView GetView();
    }

    public class AccountsPanePresenter : Presenter, IAccountsPanePresenter {
        private readonly IDataService dataService;
        private readonly IMessageBoxWrapper messageBoxWrapper;
        private readonly INotificationService notificationService;
        private readonly IAccountsPaneView view;
        private readonly IAccountsPaneViewModel viewModel;

        public AccountsPanePresenter(IAccountsPaneView view,
            IAccountsPaneViewModel viewModel,
            IDataService dataService,
            INotificationService notificationService,
            IMessageBoxWrapper messageBoxWrapper) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.dataService = dataService;
            this.notificationService = notificationService;
            this.messageBoxWrapper = messageBoxWrapper;

            BindEvents();
            BindCommands();
        }

        public IAccountsPaneView GetView() => view;

        private void BindEvents() {
            view.Loaded += OnViewLoaded;
            view.AccountsDataGrid.CellEditEnding += OnAccountsDataGridCellEditEnding;
            view.Unloaded += OnViewUnloaded;
            viewModel.SelectedAccountChanged += OnActiveAccountChanged;
            notificationService.OnTransactionBalanceChanged += OnTransactionBalanceChanged;
        }

        private void BindCommands() {
            viewModel.AddItemCommand = new AsyncCommand(ExecuteAddAccount, CanExecuteAddAccount);
            viewModel.DeleteItemCommand = new AsyncCommand(ExecuteDeleteAccount, CanExecuteDeleteAccount);
            viewModel.ShowAllTransactionsCommand = new AsyncCommand(ExecuteShowAllTransactions, CanExecuteShowAllTransactions);
        }

        private void OnTransactionBalanceChanged(object? sender, EventArgs e) {
            viewModel.AccountsTotalBalance = dataService.GetTotalBalance();
        }

        private void OnActiveAccountChanged(object? sender, EventArgs e) {
            if (sender is Account account) {
                notificationService.NotifyActiveAccountChanged(account);
            }
        }

        private void OnAccountsDataGridCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e) {
            switch ((e.Column as DataGridTextColumn)?.SortMemberPath) {
                case nameof(Account.Name):
                    ValidateAccountNameTextBoxUpdate(e);
                    break;
            }
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e) {
            PopulateAccountsList();
            UpdateAccountsTotal();
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e) {
            viewModel.ItemList.Clear();
        }

        private void ValidateAccountNameTextBoxUpdate(DataGridCellEditEndingEventArgs e) {
            var editedTextBox = (TextBox) e.EditingElement;
            string? newAccountName = editedTextBox.Text;
            if (!IsAccountNameUnique(newAccountName)) {
                editedTextBox.Text = (e.Row.Item as Account)?.Name ?? string.Empty;
            }
        }

        private bool IsAccountNameUnique(string newName) {
            IEnumerable<string>? existingNames = viewModel.ItemList.Select(account => account.Name);
            return !existingNames.Contains(newName);
        }

        private bool CanExecuteAddAccount() => true;

        private async Task ExecuteAddAccount() {
            await Task.Factory.StartNew( () => {
                Account? newAccount = dataService.AddAccount();
                viewModel.AddItem(newAccount);
            });

        }

        private bool CanExecuteShowAllTransactions() => true;

        private async Task ExecuteShowAllTransactions() {

            notificationService.NotifyShowAllTransactionsExecuted();
        }

        private bool CanExecuteDeleteAccount() => true;

        private async Task ExecuteDeleteAccount() {
            await Task.Factory.StartNew( () => {
                //Confirm the user would like to delete the account
                MessageBoxResult result =
                    messageBoxWrapper.Show(
                        "Are you sure you would like to delete your account? This will remove all transactions attaches to this account?",
                        "Delete Account", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
                switch (result) {
                    case MessageBoxResult.No:
                    case MessageBoxResult.Cancel:
                        // Don't delete account
                        break;
                    case MessageBoxResult.Yes:
                        // Attempt to delete account
                        DeleteAccount();
                        break;
                }
            });
        }

        private void DeleteAccount() {
            Account? selectedAccount = viewModel.SelectedItem;
            dataService.RemoveAccount(selectedAccount);
            viewModel.RemoveItem(selectedAccount);
        }

        private void PopulateAccountsList() {
            IEnumerable<Account>? accounts = dataService.Accounts();

            foreach (var account in accounts) {
                viewModel.AddItem(account);
            }

            if (viewModel.ItemList.Any()) {
                viewModel.SelectedItem = viewModel.ItemList.FirstOrDefault();
            }
        }

        private void UpdateAccountsTotal() {
            viewModel.AccountsTotalBalance = dataService.GetTotalBalance();
        }
    }
}