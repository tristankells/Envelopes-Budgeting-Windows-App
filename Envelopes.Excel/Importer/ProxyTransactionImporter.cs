using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Models;
using Envelopes.Persistence.Helpers;

namespace Envelopes.Persistence.Importer {
    /// <summary>
    ///     Most likely chuck away. This class is here so I can use my ITransactionsImporter interface, but hard code in the
    ///     retrieval of three different files for my personal use.
    /// </summary>
    public class ProxyTransactionImporter : ITransactionsImporter {
        private TransactionsImporter TransactionsImporter { get; } = new TransactionsImporter();

        public async Task<IEnumerable<AccountTransaction>> Import(string fileLocation, AccountTransactionColumnMap map) {
            var transactions = new List<AccountTransaction>();

            IEnumerable<AccountTransaction> kiwibankTransactions = await TransactionsImporter.Import(ImportHelper.KiwiBankLocation, ImportHelper.KiwiBankMap);
            transactions.AddRange(kiwibankTransactions.Select(transaction => {
                transaction.AccountId = 1;
                return transaction;
            }).ToList());

            IEnumerable<AccountTransaction> purpleTransactions = await TransactionsImporter.Import(ImportHelper.PurpleVisaBankLocation, ImportHelper.PurpleVisaBankMap);
            transactions.AddRange(purpleTransactions.Select(transaction => {
                transaction.AccountId = 2;
                return transaction;
            }).ToList());

            IEnumerable<AccountTransaction> amexTransactions = await TransactionsImporter.Import(ImportHelper.AmexBankLocation, ImportHelper.AmexBankMap);
            transactions.AddRange(amexTransactions.Select(transaction => {
                transaction.AccountId = 3;
                return transaction;
            }).ToList());

            return transactions;
        }
    }
}