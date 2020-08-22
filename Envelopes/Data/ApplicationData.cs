using System.Collections.Generic;
using Envelopes.Models;

namespace Envelopes.Data {
    /// <summary>
    /// Data wrapper for passing application entities loaded from persistence service. 
    /// </summary>
    public class ApplicationData {
        public IList<Account> Accounts;
        public IList<AccountTransaction> AccountTransactions;
        public IList<Category> Categories;

        public ApplicationData() {
            Accounts = new List<Account>();
            AccountTransactions = new List<AccountTransaction>();
            Categories = new List<Category>();
        }
    }
}