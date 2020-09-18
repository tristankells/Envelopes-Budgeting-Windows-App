using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Data.Persistence;
using Envelopes.Models;

namespace Envelopes.Data {
    public interface IDataService {
        public bool IgnoreApplicationSaveEvents { get; set; }

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
        public Task<AccountTransaction> AddAccountTransaction(Account activeAccountId);
        public bool AddAccountTransaction(AccountTransaction transaction);
        public bool RemoveAccountTransaction(AccountTransaction selectedAccount);

        public Task LoadApplicationData();
        public decimal GetRemainingAccountBalanceToBudget();
        public decimal GetTotalBudgeted();
        public decimal GetTotalInflow();
        decimal GetTotalBalance();
        public Task SaveBudget();
    }

    public class DataService : IDataService {
        private readonly ObservableCollection<Account> accounts = new ObservableCollection<Account>();

        private readonly ObservableCollection<AccountTransaction> accountTransactions =
            new ObservableCollection<AccountTransaction>();

        private readonly ObservableCollection<Category> categories = new ObservableCollection<Category>();
        private readonly IIdentifierService identifierService;
        private readonly INotificationService notificationService;
        private readonly IPersistenceService persistenceService;

        public DataService(IPersistenceService persistenceService,
            IIdentifierService identifierService,
            INotificationService notificationService) {
            this.notificationService = notificationService;
            this.persistenceService = persistenceService;
            this.identifierService = identifierService;
        }

        public bool IgnoreApplicationSaveEvents { get; set; }

        public async Task LoadApplicationData() {
            IgnoreApplicationSaveEvents = true;

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

            IgnoreApplicationSaveEvents = false;
        }

        public decimal GetTotalBalance() => accountTransactions.Sum(accountTransaction => accountTransaction.Inflow) - accountTransactions.Sum(accountTransaction => accountTransaction.Outflow);

        public decimal GetTotalInflow() => accountTransactions.Sum(accountTransaction => accountTransaction.Inflow);

        public decimal GetTotalBudgeted() => categories.Sum(category => category.Budgeted);

        public decimal GetRemainingAccountBalanceToBudget() =>
            accountTransactions.Sum(accountTransaction => accountTransaction.Inflow) - categories.Sum(category => category.Budgeted);


        /// <summary>
        ///     Attempts to save data to specified persistence service.
        /// </summary>
        public async Task SaveBudget() {
            if (IgnoreApplicationSaveEvents) {
                return;
            }

            var applicationData = new ApplicationData {
                Accounts = accounts.ToList(),
                Categories = categories.ToList(),
                AccountTransactions = accountTransactions.ToList()
            };
            await persistenceService.SaveApplicationData(applicationData);
        }

        private void SetAccountsTotals() {
            foreach (Account account in accounts) {
                account.Total = accountTransactions.Where(transaction => transaction.AccountId == account.Id)
                    .Select(transaction => transaction.Inflow - transaction.Outflow).Sum();
            }
        }

        private void SetCategoriesActivityAmount() {
            foreach (Category category in categories) {
                category.Activity = accountTransactions.Where(transaction => transaction.CategoryId == category.Id)
                    .Select(transaction => transaction.Inflow - transaction.Outflow).Sum();
            }
        }

        private void LoadCategories(IList<Category> categoryList) {
            foreach (Category category in categoryList) {
                category.PropertyChanged += OnCategoryPropertyChanged;
                categories.Add(category);
            }

            categories.CollectionChanged += OnCategoriesCollectionChanged;
        }

        private void LoadAccount(IList<Account> accountList) {
            foreach (Account account in accountList) {
                account.PropertyChanged += Account_PropertyChanged;
                accounts.Add(account);
            }

            accounts.CollectionChanged += Accounts_CollectionChanged;
        }

        private void LoadAccountTransaction(IList<AccountTransaction> accountTransactionList) {
            foreach (AccountTransaction transaction in accountTransactionList) {
                transaction.PropertyChanged += OnTransactionPropertyChanged;
                accountTransactions.Add(transaction);
            }

            accountTransactions.CollectionChanged += OnAccountTransactionsCollectionChanged;
        }

        private async void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            await SaveBudget();
        }

        private async void Account_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(Account.Name):
                case nameof(Account.Id):
                    await SaveBudget();
                    break;
            }
        }

        #region Accounts

        public IEnumerable<Account> Accounts() => accounts;

        public Account AddAccount() {
            var account = new Account {
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
            List<AccountTransaction> accountTransactionToRemove = accountTransactions.Where(accountTransaction => accountTransaction.AccountId == accountId).ToList();
            foreach (AccountTransaction accountTransaction in accountTransactionToRemove) {
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
            var category = new Category {
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
            IEnumerable<AccountTransaction> categoriesAccountTransactions = accountTransactions.Where(at => at.CategoryId == categoryId);
            foreach (AccountTransaction accountTransaction in categoriesAccountTransactions) {
                accountTransaction.CategoryId = 0;
            }
        }

        #endregion


        #region Account Transactions

        public IEnumerable<AccountTransaction> AccountTransactions() {
            return accountTransactions.OrderByDescending(a => a.Date);
        }

        public async Task<AccountTransaction> AddAccountTransaction(Account activeAccount) {
            var transaction = new AccountTransaction {
                AccountId = activeAccount.Id,
                AccountName = activeAccount.Name,
                Date = DateTime.Now
            };
            transaction.PropertyChanged += OnTransactionPropertyChanged;
            accountTransactions.Add(transaction);
            await SaveBudget();
            return transaction;
        }

        public bool AddAccountTransaction(AccountTransaction transaction) {
            bool isTransactionDuplicate = accountTransactions.Any(at => at.AccountId == transaction.AccountId
                                                                        && at.Date.Date == transaction.Date.Date && at.Total == transaction.Total);
            if (isTransactionDuplicate) {
                return false;
            }

            transaction.PropertyChanged += OnTransactionPropertyChanged;
            accountTransactions.Add(transaction);
            return true;
        }

        private async void OnTransactionPropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e is PropertyChangedExtendedEventArgs<decimal> pcExtendedEventArgs) {
                decimal difference = pcExtendedEventArgs.NewValue - pcExtendedEventArgs.OldValue;
                var transaction = sender as AccountTransaction;

                Account account = accounts.FirstOrDefault(a => a.Id == transaction?.AccountId);
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

        private void OnAccountTransactionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            switch (e.Action) {
                // Deduct the transaction total from the transaction account balance.
                case NotifyCollectionChangedAction.Remove: {
                    AccountTransaction updatedTransaction = e.OldItems.OfType<AccountTransaction>().FirstOrDefault();
                    if (updatedTransaction == null) return;

                    Account account = Accounts().FirstOrDefault(a => a.Id == updatedTransaction.AccountId);
                    if (account == null) return;
                    account.Total -= updatedTransaction.Total;

                    notificationService.NotifyTransactionBalanceChanged();
                    break;
                }

                case NotifyCollectionChangedAction.Add: {
                    AccountTransaction updatedTransaction = e.NewItems.OfType<AccountTransaction>().FirstOrDefault();
                    if (updatedTransaction == null) {
                        return;
                    }

                    Account account = Accounts().FirstOrDefault(a => a.Id == updatedTransaction.AccountId);
                    if (account == null) {
                        return;
                    }

                    account.Total += updatedTransaction.Total;

                    notificationService.NotifyTransactionBalanceChanged();
                    break;
                }
            }
        }

        public bool RemoveAccountTransaction(AccountTransaction transaction) => accountTransactions.Remove(transaction);

        #endregion
    }
}