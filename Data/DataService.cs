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
        public Task LoadApplicationData();
        public Task SaveBudget();

        public void SetActiveAccount(Account account);

        //Accounts
        public IEnumerable<Account> GetAccounts();
        public Account AddAccount();
        public bool RemoveAccount(Account account);

        // Categories
        public IEnumerable<Category> GetCategories();
        public bool RemoveCategory(Category selectedAccount);
        public Category AddCategory();

        // Account Transactions
        public IEnumerable<AccountTransaction> GetAccountTransactions();
        public bool RemoveAccountTransaction(AccountTransaction selectedAccount);
        public AccountTransaction AddAccountTransaction();

        public decimal GetRemainingBalanceToBudget();


        public event EventHandler BudgetAmountUpdated;
    }

    public class DataService : IDataService {
        // Ugly solution, have added this for integration tests, to change the file.

        public event EventHandler BudgetAmountUpdated;
        private readonly IPersistenceService persistenceService;
        private readonly IIdentifierService identifierService;

        public string FileName { get; set; }

        private readonly ObservableCollection<Account> accounts = new ObservableCollection<Account>();
        private readonly ObservableCollection<Category> categories = new ObservableCollection<Category>();
        private readonly ObservableCollection<AccountTransaction> accountTransactions = new ObservableCollection<AccountTransaction>();

        private Account activeAccount;

        public DataService(IPersistenceService persistenceService, IIdentifierService identifierService) {
            this.persistenceService = persistenceService;
            this.identifierService = identifierService;
        }

        public async Task LoadApplicationData() {
            var applicationData = await (string.IsNullOrEmpty(FileName)
                ? persistenceService.GetApplicationData()
                : persistenceService.GetApplicationData(FileName));

            identifierService.Setup(applicationData);

            foreach (var accountTransaction in applicationData.AccountTransactions) {
                accountTransaction.PropertyChanged += Transaction_PropertyChanged;
                accountTransactions.Add(accountTransaction);
            }
            accountTransactions.CollectionChanged += OnAccountTransactionsCollectionChanged;

            foreach (var category in applicationData.Categories) {
                category.PropertyChanged += Category_PropertyChanged;
                categories.Add(category);
            }
            categories.CollectionChanged += Categories_CollectionChanged;

            foreach (var account in applicationData.Accounts) {
                account.PropertyChanged += Account_PropertyChanged;
                accounts.Add(account);
            }
            accounts.CollectionChanged += Accounts_CollectionChanged; ;

            if (accounts.Any()) {
                activeAccount = accounts.First();

                foreach (var account in accounts) {
                    account.Total = accountTransactions.Where(transaction => transaction.AccountId == account.Id)
                        .Select(transaction => transaction.Inflow - transaction.Outflow).Sum();
                }
            }

            if (categories.Any()) {
                foreach (var category in categories) {
                    category.Activity = accountTransactions.Where(transaction => transaction.CategoryId == category.Id)
                        .Select(transaction => transaction.Inflow - transaction.Outflow).Sum();
                }
            }

        }

        private void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            //throw new NotImplementedException();
        }

        private void Account_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            //throw new NotImplementedException();
        }



        /// <summary>
        /// Attempts to save data to specified persistence service.
        /// </summary>
        public async Task SaveBudget() {
            var applicationData = new ApplicationData() {
                Accounts = accounts.ToList(),
                Categories = categories.ToList(),
                AccountTransactions = accountTransactions.ToList()
            };
            await (string.IsNullOrEmpty(FileName)
                ? persistenceService.SaveApplicationData(applicationData)
                : persistenceService.SaveApplicationData(applicationData, FileName));
        }

        #region Accounts

        public IEnumerable<Account> GetAccounts() {
            return accounts;
        }

        /// <summary>
        /// Save new account to to Account list. Save 
        /// </summary>
        public Account AddAccount() {
            var account = new Account() {
                Id = identifierService.GetNewAccountId()
            };
            accounts.Add(account);
            return account;
        }

        /// <summary>
        /// Save new account to to Account list. Save 
        /// </summary>
        public bool RemoveAccount(Account account) {
            return accounts.Remove(account);
        }


        public void SetActiveAccount(Account account) {
            activeAccount = account;
        }

        #endregion


        #region Categories

        public IEnumerable<Category> GetCategories() {
            return categories;
        }

        public Category AddCategory() {
            var category = new Category() {
                Id = identifierService.GetNewCategoryId()
            };
            category.PropertyChanged += Category_PropertyChanged;
            categories.Add(category);
            return category;
        }

        private void Category_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(Category.Budgeted):
                    BudgetAmountUpdated?.Invoke(sender, e);
                    break;
            }
        }

        private void Categories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                BudgetAmountUpdated?.Invoke(sender, e);
            }
        }

        public bool RemoveCategory(Category category) {

            // For all account transactions with this category ID, reset their ID to 0.
            foreach (var accountTransaction in accountTransactions) {
                if (accountTransaction.CategoryId == category.Id) {
                    accountTransaction.CategoryId = 0;
                }
            }

            return categories.Remove(category);
        }

        #endregion


        #region Account Transactions
        public IEnumerable<AccountTransaction> GetAccountTransactions() {
            return accountTransactions;
        }

        public AccountTransaction AddAccountTransaction() {
            var transaction = new AccountTransaction() {
                Id = identifierService.GetNewTransactionId(),
                AccountId = activeAccount.Id,
                AccountName = activeAccount.Name,
                Date = DateTime.Now
            };
            transaction.PropertyChanged += Transaction_PropertyChanged;
            accountTransactions.Add(transaction);
            return transaction;
        }

        private void Transaction_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e is PropertyChangedExtendedEventArgs<decimal> pcExtendedEventArgs) {
                var difference = pcExtendedEventArgs.NewValue - pcExtendedEventArgs.OldValue;
                var transaction = sender as AccountTransaction;

                var account = accounts.FirstOrDefault(a => a.Id == transaction?.AccountId);
                if (account == null) { return;}

                switch (e.PropertyName) {
                    case nameof(AccountTransaction.Outflow):
                        account.Total -= difference;
                        break;
                    case nameof(AccountTransaction.Inflow):
                        account.Total += difference;
                        break;
                }
            }
        }

        private void OnAccountTransactionsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            // Deduct the transaction total from the transaction account balance.
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                var updatedTransaction = e.OldItems.OfType<AccountTransaction>().FirstOrDefault();
                if (updatedTransaction == null) return;

                var account = accounts.FirstOrDefault(a => a.Id == updatedTransaction?.AccountId);
                if (account == null) return;

                account.Total -= updatedTransaction.Total;
            }
        }

        public bool RemoveAccountTransaction(AccountTransaction transaction) {
            return accountTransactions.Remove(transaction);
        }

        #endregion

        public decimal GetRemainingBalanceToBudget() {
            return accounts.Sum(account => account.Total) - categories.Sum(category => category.Budgeted);
        }

    }

}