using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Models;

namespace Envelopes.Data {
    public sealed class DataService {
        private IList<Account> listOfAccounts;

        private readonly IPersistenceService persistenceService;

        private static readonly Lazy<DataService> Lazy = new Lazy<DataService>(() => new DataService());

        private int idCounter;

        public static DataService Instance => Lazy.Value;

        /// <summary>
        /// Until I can figure out how to get the singleton pattern working with Ninject, injecting dependencies in here.
        /// </summary>
        private DataService() {
            persistenceService = new JsonPersistenceService();
        }

        public async Task LoadApplicationData() {
            listOfAccounts = await persistenceService.LoadAccounts();
            // Finding the largest ID, make our ID counter that number + 1
            if (listOfAccounts.Any()) {
                idCounter = listOfAccounts.Select(account => account.Id).Max() + 1;
            }
        }

        /// <summary>
        /// Save new account to to Account list. Save 
        /// </summary>
        public Account AddAccount() {
            var account = new Account() {
                Id = idCounter
            };
            idCounter++;
            listOfAccounts.Add(account);
            return account;
        }


        /// <summary>
        /// Save new account to to Account list. Save 
        /// </summary>
        public Account RemoveAccount(Account account) {
            listOfAccounts.Remove(account);
            return account;
        }

        /// <summary>
        /// Attempts to save data to specified persistence service.
        /// </summary>
        public async Task SaveBudget() {
            await persistenceService.SaveAccounts(listOfAccounts.ToList());
        }

        public IEnumerable<Account> GetAccounts() {
            return listOfAccounts;
        }
    }
}