using System.Linq;

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
}