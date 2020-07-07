using System.Threading.Tasks;
using Envelopes.Data;
using Envelopes.Data.Persistence;
using Moq;
using NUnit.Framework;


namespace Tests.Envelopes.Data {
    class DataServiceTests {
        private DataService dataService;
        private Mock<IPersistenceService> persistenceService;
        private Mock<IIdentifierService> identifierService;
        private Mock<INotificationService> notificationService;

        [SetUp]
        public void Setup() {
            persistenceService = new Mock<IPersistenceService>();
            identifierService = new Mock<IIdentifierService>();
            notificationService = new Mock<INotificationService>();
            dataService = new DataService(persistenceService.Object, identifierService.Object, notificationService.Object);
        }

        [Test]
        public async Task LoadApplicationData_SetAccountsTotals() {
            await dataService.LoadApplicationData();
        }

        [Test]
        public async Task LoadApplicationData_SetCategoriesActivityAmount() {
           await dataService.LoadApplicationData();
        }

        [Test]
        public void SaveBudget() {
            dataService.SaveBudget();
        }

        [Test]
        public void AddAccount() {
            dataService.AddAccount();
        }

        [Test]
        public void RemoveAccount() {
            //dataService.RemoveAccount();
        }

        [Test]
        public void AddCategory() {
            //dataService.AddCategory();
        }

        [Test]
        public void RemoveCategory() {
            //dataService.RemoveCategory();
        }

        [Test]
        public void AddAccountTransaction() {
            //dataService.AddAccountTransaction();

        }

        [Test]
        public void RemoveAccountTransaction() {
            //dataService.RemoveAccountTransaction();

        }

        [Test]
        public void GetRemainingAccountBalanceToBudget() {
            dataService.GetRemainingAccountBalanceToBudget();

        }

        [Test]
        public void GetTotalAccountBalance() {
            dataService.GetTotalAccountBalance();
        }
    }
}
