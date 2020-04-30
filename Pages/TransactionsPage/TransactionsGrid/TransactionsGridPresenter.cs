﻿using System;
using System.Windows;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Models;

namespace Envelopes.TransactionsPage.TransactionsGrid {
    public interface ITransactionsGridPresenter {
        TransactionsGridView GetView();
    }

    public class TransactionsGridPresenter : Presenter, ITransactionsGridPresenter {
        private readonly TransactionsGridView view;
        private readonly ITransactionsGridViewModel viewModel;
        private readonly IDataService dataService;

        public TransactionsGridPresenter(TransactionsGridView view, ITransactionsGridViewModel viewModel,
            IDataService dataService) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.dataService = dataService;

            BindEvents();
            BindCommands();
        }

        #region Events

        private void BindEvents() {
            view.Loaded += View_Loaded;
            view.Unloaded += View_Unloaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            PopulateTransactionsList();
        }


        private void View_Unloaded(object sender, RoutedEventArgs e) {
            view.Loaded -= View_Loaded;
            view.Unloaded -= View_Unloaded;
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
            var accounts = dataService.GetAccountTransactions();
            foreach (var account in accounts) {
                viewModel.AddItem(account);
            }
        }
    }
}