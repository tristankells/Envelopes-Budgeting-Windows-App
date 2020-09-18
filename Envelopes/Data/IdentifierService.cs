using System.Linq;

namespace Envelopes.Data {
    public interface IIdentifierService {
        public void Setup(ApplicationData applicationData);
        public int GetNewAccountId();
        public int GetNewCategoryId();
    }

    public class IdentifierService : IIdentifierService {
        private int accountIdCounter;
        private int categoryIdCounter;

        public void Setup(ApplicationData applicationData) {
            accountIdCounter = applicationData.Accounts.Any()
                ? applicationData.Accounts.Select(account => account.Id).Max()
                : 0;
            categoryIdCounter = applicationData.Categories.Any()
                ? applicationData.Categories.Select(account => account.Id).Max()
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
    }
}