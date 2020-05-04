using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Data.Persistence;
using Envelopes.Models;

namespace Envelopes.Data {
    public interface IIdentifierService {
        public void Setup(ApplicationData applicationData);
        public int GetNewAccountId();
        public int GetNewCategoryId();
        int GetNewTransactionId();

    }

    public class IdentifierService : IIdentifierService {
        private int accountIdCounter;
        private int categoryIdCounter;
        private int accountTransactionsIdCounter;

        public void Setup(ApplicationData applicationData) {
            accountIdCounter = applicationData.Accounts.Any()
                ? applicationData.Accounts.Select(account => account.Id).Max()
                : 0;
            categoryIdCounter = applicationData.Categories.Any()
                ? applicationData.Categories.Select(account => account.Id).Max()
                : 0;
            accountTransactionsIdCounter = applicationData.AccountTransactions.Any()
                ? applicationData.AccountTransactions.Select(account => account.Id).Max()
                : 0;
        }

        public int GetNewAccountId() {
            accountIdCounter++;
            return accountIdCounter;
        }

        public int GetNewCategoryId() {
            categoryIdCounter++;
            return categoryIdCounter;
        }

        public int GetNewTransactionId() {
            accountTransactionsIdCounter++;
            return accountTransactionsIdCounter;
        }
    }

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

            accounts = applicationData.Accounts;
            categories = applicationData.Categories;
            accountTransactions = applicationData.AccountTransactions;

            if (accounts.Any()) {
                activeAccount = accounts.First();
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

        // Accounts
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

        // Categories
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
            return categories.Remove(category);
        }

        // Account Transactions
        public IEnumerable<AccountTransaction> GetAccountTransactions() {
            return accountTransactions;
        }

        public AccountTransaction AddAccountTransaction() {
            var transaction = new AccountTransaction() {
                Id = identifierService.GetNewTransactionId(),
                AccountId = activeAccount.Id,
                AccountName = activeAccount.Name
            };
            accountTransactions.Add(transaction);
            return transaction;
        }

        public bool RemoveAccountTransaction(AccountTransaction transaction) {
            return accountTransactions.Remove(transaction);
        }

        public void SetActiveAccount(Account account) {
            activeAccount = account;
        }
    }
}