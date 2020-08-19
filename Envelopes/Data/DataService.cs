using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Common;
using Envelopes.Data.Persistence;
using Envelopes.Models;

namespace Envelopes.Data {
    public interface IDataService {
        // Accounts
        public IEnumerable<Account> Accounts();
        public Account AddAccount();
        public bool RemoveAccount(Account account);

        // Categories
        public IEnumerable<Category> Categories();
        public Category AddCategory();
        public bool RemoveCategory(Category selectedAccount);

        // Account Transactions
        public IEnumerable<AccountTransaction> AccountTransactions();
        AccountTransaction AddAccountTransaction(Account activeAccountId);
        public bool RemoveAccountTransaction(AccountTransaction selectedAccount);

        public Task LoadApplicationData();
        public Task SaveBudget();
        public decimal GetRemainingAccountBalanceToBudget();
        public decimal GetTotalAccountBalance();

    }

    public class DataService : IDataService {
        private readonly INotificationService notificationService;
        private readonly IPersistenceService persistenceService;
        private readonly IIdentifierService identifierService;
        private bool ignoreApplicationSaveEvents = false;

        public string FileName { get; set; }

        private readonly ObservableCollection<Account> accounts = new ObservableCollection<Account>();
        private readonly ObservableCollection<Category> categories = new ObservableCollection<Category>();
        private readonly ObservableCollection<AccountTransaction> accountTransactions =
            new ObservableCollection<AccountTransaction>();

        public DataService(IPersistenceService persistenceService,
            IIdentifierService identifierService,
            INotificationService notificationService) {
            this.notificationService = notificationService;
            this.persistenceService = persistenceService;
            this.identifierService = identifierService;
        }

        public async Task LoadApplicationData() {
            ignoreApplicationSaveEvents = true;

            ApplicationData applicationData = await persistenceService.GetApplicationData();
            
            identifierService.Setup(applicationData);

            LoadAccountTransaction(applicationData.AccountTransactions);
            LoadCategories(applicationData.Categories);
            LoadAccount(applicationData.Accounts);

            if (accounts.Any()) {
                SetAccountsTotals();
            }

            if (categories.Any()) {
                SetCategoriesActivityAmount();
            }

            ignoreApplicationSaveEvents = false;
        }

        private void SetAccountsTotals() {
            foreach (var account in accounts) {
                account.Total = accountTransactions.Where(transaction => transaction.AccountId == account.Id)
                    .Select(transaction => transaction.Inflow - transaction.Outflow).Sum();
            }
        }

        private void SetCategoriesActivityAmount() {
            foreach (var category in categories) {
                category.Activity = accountTransactions.Where(transaction => transaction.CategoryId == category.Id)
                    .Select(transaction => transaction.Inflow - transaction.Outflow).Sum();
            }
        }

        private void LoadCategories(IList<Category> categoryList) {
            foreach (var category in categoryList) {
                category.PropertyChanged += OnCategoryPropertyChanged;
                categories.Add(category);
            }

            categories.CollectionChanged += OnCategoriesCollectionChanged;
        }

        private void LoadAccount(IList<Account> accountList) {
            foreach (var account in accountList) {
                account.PropertyChanged += Account_PropertyChanged;
                accounts.Add(account);
            }

            accounts.CollectionChanged += Accounts_CollectionChanged;
        }

        private void LoadAccountTransaction(IList<AccountTransaction> accountTransactionList) {
            foreach (var transaction in accountTransactionList) {
                transaction.PropertyChanged += OnTransactionPropertyChanged;
                accountTransactions.Add(transaction);
            }

            accountTransactions.CollectionChanged += OnAccountTransactionsCollectionChanged;
        }

        private async void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            await SaveBudget();
        }

        private async void Account_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            await SaveBudget();
        }


        /// <summary>
        /// Attempts to save data to specified persistence service.
        /// </summary>
        public async Task SaveBudget() {
            if (ignoreApplicationSaveEvents) {
                return;
            }

            var applicationData = new ApplicationData() {
                Accounts = accounts.ToList(),
                Categories = categories.ToList(),
                AccountTransactions = accountTransactions.ToList()
            };
            await persistenceService.SaveApplicationData(applicationData);
        }

        #region Accounts

        public IEnumerable<Account> Accounts() {
            return accounts;
        }

        public Account AddAccount() {
            var account = new Account() {
                Id = identifierService.GetNewAccountId()
            };
            accounts.Add(account);
            return account;
        }

        public bool RemoveAccount(Account account) {
            //bool isCurrentlyActiveAccount = account == activeAccount;
            //bool isMoreThanOneAccount = accounts.Count > 1;
            //if (isCurrentlyActiveAccount && isMoreThanOneAccount) {
            //    var indexOfCurrentAccount = accounts.IndexOf(account);
            //    SetActiveAccount(accounts[indexOfCurrentAccount + 1]);
            //
            RemoveAllAccountTransactionsForAccount(account.Id);



            return accounts.Remove(account);
        }

        private void RemoveAllAccountTransactionsForAccount(int accountId) {
            var accountTransactionToRemove = accountTransactions.Where(accountTransaction => accountTransaction.AccountId == accountId).ToList();
            foreach (var accountTransaction in accountTransactionToRemove) {
                accountTransactions.Remove(accountTransaction);
            }
        }

        //public void SetActiveAccount(Account account) {
        //    if (activeAccount == account) return;
        //    activeAccount = account;
        //    notificationService.NotifyActiveAccountChanged(account);
        //}

        #endregion


        #region Categories

        public IEnumerable<Category> Categories() {
            SetCategoriesActivityAmount();
            return categories;
        }

        public Category AddCategory() {
            var category = new Category() {
                Id = identifierService.GetNewCategoryId()
            };
            category.PropertyChanged += OnCategoryPropertyChanged;
            categories.Add(category);
            return category;
        }

        private async void OnCategoryPropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(Category.Budgeted):
                    notificationService.NotifyCategoryBudgetedChanged();
                    break;
            }

            await SaveBudget();
        }

        private async void OnCategoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                notificationService.NotifyCategoryBudgetedChanged();
            }

            await SaveBudget();
        }

        public bool RemoveCategory(Category category) {
            ResetCategoryForCategoriesAccountTransactions(category.Id); // For all account transactions with this category ID, reset their ID to 0.
            return categories.Remove(category);
        }

        private void ResetCategoryForCategoriesAccountTransactions(int categoryId) {
            var categoriesAccountTransactions = accountTransactions.Where(at => at.CategoryId == categoryId);
            foreach (var accountTransaction in categoriesAccountTransactions) {
                accountTransaction.CategoryId = 0;
            }
        }

        #endregion


        #region Account Transactions

        public IEnumerable<AccountTransaction> AccountTransactions() {
            return accountTransactions;
        }

        public AccountTransaction AddAccountTransaction(Account activeAccount) {
            var transaction = new AccountTransaction() {
                Id = identifierService.GetNewTransactionId(),
                AccountId = activeAccount.Id,
                AccountName = activeAccount.Name,
                Date = DateTime.Now
            };
            transaction.PropertyChanged += OnTransactionPropertyChanged;
            accountTransactions.Add(transaction);
            return transaction;
        }

        private async void OnTransactionPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e is PropertyChangedExtendedEventArgs<decimal> pcExtendedEventArgs) {
                var difference = pcExtendedEventArgs.NewValue - pcExtendedEventArgs.OldValue;
                var transaction = sender as AccountTransaction;

                var account = accounts.FirstOrDefault(a => a.Id == transaction?.AccountId);
                if (account == null) {
                    return;
                }

                switch (e.PropertyName) {
                    case nameof(AccountTransaction.Outflow):
                        account.Total -= difference;
                        notificationService.NotifyTransactionBalanceChanged();
                        break;
                    case nameof(AccountTransaction.Inflow):
                        account.Total += difference;
                        notificationService.NotifyTransactionBalanceChanged();
                        break;
                }
            }

            await SaveBudget();
        }

        private async void OnAccountTransactionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            // Deduct the transaction total from the transaction account balance.
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                var updatedTransaction = e.OldItems.OfType<AccountTransaction>().FirstOrDefault();
                if (updatedTransaction == null) return;

                Account account = Accounts().FirstOrDefault(a => a.Id == updatedTransaction?.AccountId);
                if (account == null) return;
                account.Total -= updatedTransaction.Total;

                notificationService.NotifyTransactionBalanceChanged();
            }

            await SaveBudget();
        }

        public bool RemoveAccountTransaction(AccountTransaction transaction) {
            return accountTransactions.Remove(transaction);
        }

        #endregion

        public decimal GetTotalAccountBalance() => accounts.Sum(account => account.Total);

        public decimal GetRemainingAccountBalanceToBudget() =>
            accounts.Sum(account => account.Total) - categories.Sum(category => category.Budgeted);
    }
}