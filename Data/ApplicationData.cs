using System;
using System.Collections.Generic;
using System.Text;
using Envelopes.Models;

namespace Envelopes.Data
{
    /// <summary>
    /// Data wrapper for passing application entities loaded from persistence service. 
    /// </summary>
    public class ApplicationData {
        public IList<Account> Accounts;
        public IList<AccountTransaction> AccountTransactions;
        public IList<Category> Categories;
        public IList<CategoryGroup> CategoryGroups;
        public IList<Payee> Payees;

        public ApplicationData() {
            Accounts = new List<Account>();
            AccountTransactions = new List<AccountTransaction>();
            Categories = new List<Category>();
            CategoryGroups = new List<CategoryGroup>();
            Payees = new List<Payee>();
        }
    }
}
