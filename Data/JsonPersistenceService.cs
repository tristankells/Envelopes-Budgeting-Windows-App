using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Envelopes.Models;

namespace Envelopes.Data {
    interface IPersistenceService {

        Task SaveAccounts(IList<Account> accounts, string path);
        Task SaveAccounts(IList<Account> accounts);

        Task<IList<Account>> LoadAccounts(string path);
        Task<IList<Account>> LoadAccounts();
    }

    public class JsonPersistenceService : IPersistenceService {
        private const string DefaultBudgetJsonPath = "Envelopes.json";
        private readonly string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public async Task SaveAccounts(IList<Account> accounts) {
            await SaveAccounts(accounts, DefaultBudgetJsonPath);
        }

        public async Task SaveAccounts(IList<Account> accounts, string path) {
            // Turn list in to json object
            var json = JsonSerializer.Serialize(accounts);

            // Write the specified text asynchronously to a new file named "YouNeedABudget.json".
            await using var outputFile = new StreamWriter(Path.Combine(docPath, path));
            await outputFile.WriteAsync(json);
        }

        public async Task<IList<Account>> LoadAccounts(string path) {
            IList<Account> accounts = new List<Account>();

            using (StreamReader sr = new StreamReader(Path.Combine(docPath, path))) {
                string line = await sr.ReadToEndAsync();
                accounts = JsonSerializer.Deserialize<List<Account>>(line);
            }

            return accounts;
        }

        public async Task<IList<Account>> LoadAccounts() {
            return await LoadAccounts(DefaultBudgetJsonPath);
        }
    }
}