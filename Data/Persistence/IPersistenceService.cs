using System.Collections.Generic;
using System.Threading.Tasks;
using Envelopes.Models;

namespace Envelopes.Data {
    public interface IPersistenceService {

        Task SaveAccounts(IList<Account> accounts, string fileName);
        Task SaveAccounts(IList<Account> accounts);

        Task<IList<Account>> LoadAccounts(string path);
        Task<IList<Account>> LoadAccounts();

        Task<ApplicationData> GetApplicationData(string fileName);
        Task<ApplicationData> GetApplicationData();
    }
}