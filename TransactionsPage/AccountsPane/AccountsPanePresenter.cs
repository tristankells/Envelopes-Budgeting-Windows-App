using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Models;

namespace Envelopes.TransactionsPage.AccountsPane {
    public interface IAccountsPanePresenter  {
        public AccountsPaneView GetView();
    }

    class AccountsPanePresenter : Presenter, IAccountsPanePresenter {
        #region Fields

        private readonly AccountsPaneView view;
        private readonly IAccountsPaneViewModel viewModel;

        #endregion

        public AccountsPanePresenter(AccountsPaneView view, IAccountsPaneViewModel viewModel) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;


            BindEvents();
            BindCommands();
        }

        private void BindEvents() {
            view.Loaded += View_Loaded; 
            //viewModel.OnAccountNameUpdated += ViewModel_OnAccountNameUpdated;
            view.AccountsDataGrid.CellEditEnding += AccountsDataGrid_CellEditEnding;
         
        }

        private void AccountsDataGrid_CellEditEnding(object sender, System.Windows.Controls.DataGridCellEditEndingEventArgs e) {
            var editedTextBox = (TextBox) e.EditingElement;
            if (editedTextBox == null) {
                return;
            }

            var newAccountName = editedTextBox.Text;
            var existingNames = viewModel.AccountsList.Select(account => account.Name);
            if (existingNames.Contains(newAccountName)) {
                view.AccountsDataGrid.CancelEdit();
            }
        }


        private void BindCommands() {
            viewModel.AddAccountCommand = new DelegateCommand(ExecuteAddAccount, CanExecuteAddAccount);
            viewModel.DeleteAccountCommand = new DelegateCommand(ExecuteDeleteAccount, CanExecuteDeleteAccount);
        }

        private bool CanExecuteAddAccount() => true;

        private void ExecuteAddAccount() {
            AddAccount();
        }

        private void AddAccount() {
            var newAccount = DataService.Instance.AddAccount();
            viewModel.AddAccount(newAccount);
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
            var selectedAccount = viewModel.SelectedAccount;
            DataService.Instance.RemoveAccount(selectedAccount);
            viewModel.RemoveAccount(selectedAccount);
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            PopulateAccountsList();
        }

        public AccountsPaneView GetView() => view;

        private void PopulateAccountsList() {
            var accounts = DataService.Instance.GetAccounts();
            foreach (var account in accounts) {
                viewModel.AddAccount(account);
            }
        }
    }
}