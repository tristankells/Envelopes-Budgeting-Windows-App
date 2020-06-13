#nullable enable
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Models;


namespace Envelopes.Pages.TransactionsPage.AccountsPane {
    public interface IAccountsPanePresenter {
        public AccountsPaneView GetView();
    }

    public class AccountsPanePresenter : Presenter, IAccountsPanePresenter {
        private readonly AccountsPaneView view;
        private readonly IAccountsPaneViewModel viewModel;
        private readonly IDataService dataService;
        private INotificationService notificationService;

        public AccountsPanePresenter(AccountsPaneView view,
            IAccountsPaneViewModel viewModel,
            IDataService dataService,
            INotificationService notificationService) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.dataService = dataService;
            this.notificationService = notificationService;

            BindEvents();
            BindCommands();
        }

        public AccountsPaneView GetView() => view;

        private void BindEvents() {
            view.Loaded += OnViewLoaded;
            view.accountsDataGrid.CellEditEnding += OnAccountsDataGridCellEditEnding;
            view.Unloaded += OnViewUnloaded;
            viewModel.ActiveAccountChanged += OnActiveAccountChanged;
            notificationService.OnTransactionBalanceChanged += OnTransactionBalanceChanged;
        }

        private void BindCommands() {
            viewModel.AddItemCommand = new DelegateCommand(ExecuteAddAccount, CanExecuteAddAccount);
            viewModel.DeleteItemCommand = new DelegateCommand(ExecuteDeleteAccount, CanExecuteDeleteAccount);
        }

        private void OnTransactionBalanceChanged(object? sender, EventArgs e) {
            viewModel.AccountsTotalBalance = dataService.GetTotalAccountBalance();
        }

        private void OnActiveAccountChanged(object? sender, System.EventArgs e) {
            if (sender is Account account) {
                dataService.SetActiveAccount(account);
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
            var newAccountName = editedTextBox.Text;
            if (!IsAccountNameUnique(newAccountName)) {
                editedTextBox.Text = (e.Row.Item as Account)?.Name ?? string.Empty;
            }
        }

        private bool IsAccountNameUnique(string newName) {
            var existingNames = viewModel.ItemList.Select(account => account.Name);
            return !existingNames.Contains(newName);
        }


        private bool CanExecuteAddAccount() => true;

        private void ExecuteAddAccount() {
            var newAccount = dataService.AddAccount();
            viewModel.AddItem(newAccount);
        }

        private bool CanExecuteDeleteAccount() => true;

        private void ExecuteDeleteAccount() {
            //Confirm the user would like to delete the account
            MessageBoxResult result =
                MessageBox.Show(
                    "Are you sure you would like to delete your account? This will remove all transactions attaches to this account?",
                    "Delete Account", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            switch (result) {
                case MessageBoxResult.No:
                case MessageBoxResult.Cancel:
                    // Don't delete account
                    break;
                case MessageBoxResult.Yes: // Attempt to delete account
                    DeleteAccount();
                    break;
            }
        }

        private void DeleteAccount() {
            var selectedAccount = viewModel.SelectedItem;
            dataService.RemoveAccount(selectedAccount);
            viewModel.RemoveItem(selectedAccount);
        }

        private void PopulateAccountsList() {
            var accounts = dataService.GetAccounts();
            foreach (var account in accounts) {
                viewModel.AddItem(account);
            }
        }

        private void UpdateAccountsTotal() {
            viewModel.AccountsTotalBalance = dataService.GetTotalAccountBalance();
        }
    }
}