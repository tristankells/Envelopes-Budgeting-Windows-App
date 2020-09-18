using System.Collections.Generic;
using System.Threading.Tasks;
using Envelopes.Models;

namespace Envelopes.Persistence.Importer {
    public interface ITransactionsImporter {
        Task<IEnumerable<AccountTransaction>> Import(string fileLocation, AccountTransactionColumnMap map);
    }
}