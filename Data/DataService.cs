using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Data.Persistence;
using Envelopes.Models;

namespace Envelopes.Data {
    public interface IDataService {
        public IEnumerable<Account> GetAccounts();
        public Task LoadApplicationData();
        public Account RemoveAccount(Account account);
        public Task SaveBudget();
        public Account AddAccount();
    }

    public class DataService : IDataService {
        // Ugly solution, have added this for integration tests, to change the file.
        public string FileName { get; set; }

        private IList<Account> accounts;
        private readonly IPersistenceService persistenceService;

        private int accountIdCounter;

        /// <summary>
        /// Until I can figure out how to get the singleton pattern working with Ninject, injecting dependencies in here.
        /// </summary>
        public DataService(IPersistenceService persistenceService) {
            accounts = new List<Account>();
            this.persistenceService = new ExcelPersistenceService();
        }

        public async Task LoadApplicationData() {
            var applicationData = await (string.IsNullOrEmpty(FileName)
                ? persistenceService.GetApplicationData()
                : persistenceService.GetApplicationData(FileName));

            accounts = applicationData.Accounts;

            // Finding the largest ID, make our ID counter that number + 1
            if (accounts.Any()) {
                accountIdCounter = accounts.Select(account => account.Id).Max() + 1;
            }
        }

        /// <summary>
        /// Save new account to to Account list. Save 
        /// </summary>
        public Account AddAccount() {
            var account = new Account() {
                Id = accountIdCounter
            };
            accountIdCounter++;
            accounts.Add(account);
            return account;
        }

        /// <summary>
        /// Save new account to to Account list. Save 
        /// </summary>
        public Account RemoveAccount(Account account) {
            accounts.Remove(account);
            return account;
        }

        /// <summary>
        /// Attempts to save data to specified persistence service.
        /// </summary>
        public async Task SaveBudget() {
            await (string.IsNullOrEmpty(FileName)
                ? persistenceService.SaveAccounts(accounts.ToList())
                : persistenceService.SaveAccounts(accounts.ToList(), FileName));
        }

        public IEnumerable<Account> GetAccounts() {
            return accounts;
        }
    }
}