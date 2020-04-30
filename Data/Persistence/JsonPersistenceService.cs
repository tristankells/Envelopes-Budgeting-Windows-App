using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Envelopes.Models;

namespace Envelopes.Data.Persistence {
    public class JsonPersistenceService : IPersistenceService {
        private const string DefaultBudgetJsonPath = "Envelopes.json";
        private readonly string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public async Task SaveApplicationData(IList<Account> accounts) {
            await SaveApplicationData(accounts, DefaultBudgetJsonPath);
        }

        public async Task SaveApplicationData(IList<Account> accounts, string fileName) {
            // Turn list in to json object
            var json = JsonSerializer.Serialize(accounts);

            // Write the specified text asynchronously to a new file named "YouNeedABudget.json".
            await using var outputFile = new StreamWriter(Path.Combine(docPath, fileName));
            await outputFile.WriteAsync(json);
        }

        public async Task<IList<Account>> LoadAccounts() {
            return await LoadAccounts(DefaultBudgetJsonPath);
        }

        public async Task<IList<Account>> LoadAccounts(string path) {
            IList<Account> accounts = new List<Account>();

            using (StreamReader sr = new StreamReader(Path.Combine(docPath, path))) {
                string line = await sr.ReadToEndAsync();
                accounts = JsonSerializer.Deserialize<List<Account>>(line);
            }

            return accounts;
        }


        public Task SaveApplicationData(ApplicationData data, string fileName) {
            throw new NotImplementedException();
        }

        public Task SaveApplicationData(ApplicationData data) {
            throw new NotImplementedException();
        }

        public Task<ApplicationData> GetApplicationData(string fileName) {
            throw new NotImplementedException();
        }

        public async Task<ApplicationData> GetApplicationData() {
            return await GetApplicationData(DefaultBudgetJsonPath);
        }
    }
}