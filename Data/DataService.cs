using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
    }

    public class DataService : IDataService {
        // Ugly solution, have added this for integration tests, to change the file.
        public string FileName { get; set; }

        private IList<Account> accounts;
        private IList<Category> categories;
        private IList<AccountTransaction> accountTransactions;

        private Account activeAccount;

        private readonly IPersistenceService persistenceService;
        private readonly IIdentifierService identifierService;

        public DataService(IPersistenceService persistenceService, IIdentifierService identifierService) {
            accounts = new List<Account>();
            categories = new List<Category>();
            accountTransactions = new List<AccountTransaction>();

            this.persistenceService = persistenceService;
            this.identifierService = identifierService;
        }

        public async Task LoadApplicationData() {
            var applicationData = await (string.IsNullOrEmpty(FileName)
                ? persistenceService.GetApplicationData()
                : persistenceService.GetApplicationData(FileName));

            identifierService.Setup(applicationData);

            accountTransactions = applicationData.AccountTransactions;

            categories = applicationData.Categories;

            accounts = applicationData.Accounts;

            if (accounts.Any()) {
                activeAccount = accounts.First();

                foreach (var account in accounts) {
                    account.Total = accountTransactions.Where(transaction => transaction.AccountId == account.Id)
                        .Select(transaction => transaction.Inflow - transaction.Outflow).Sum();
                }
            }
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
            categories.Add(category);
            return category;
        }

        public bool RemoveCategory(Category category) {
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
            accountTransactions.Add(transaction);
            return transaction;
        }

        public bool RemoveAccountTransaction(AccountTransaction transaction) {
            if (accountTransactions.Remove(transaction)) {
                return true;
            }
            return false;
        }

        #endregion

    }

}