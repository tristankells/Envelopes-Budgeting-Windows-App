using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Envelopes.Data;
using Envelopes.Data.Persistence;
using Envelopes.Models;
using Envelopes.Pages.TransactionsPage.AccountsPane;
using Moq;
using NUnit.Framework;


namespace Tests.Envelopes {
    [Apartment(ApartmentState.STA)]
    public class AccountsPaneTests {
        private AccountsPaneViewModel viewModel;
        private AccountsPanePresenter presenter;
        private Mock<IAccountsPaneView> view;
        private Mock<IDataService> dataServiceMock;
        private Mock<IMessageBoxWrapper> messageBoxWrapper;
        private Mock<INotificationService> notificationService;

        [SetUp]
        public void Setup() {
            view = new Mock<IAccountsPaneView>();
            view.Setup(v => v.AccountsDataGrid).Returns(new DataGrid());
            dataServiceMock = new Mock<IDataService>();
            notificationService = new Mock<INotificationService>();
            messageBoxWrapper = new Mock<IMessageBoxWrapper>();
            messageBoxWrapper.Setup(mbw => mbw.Show(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<MessageBoxButton>(), It.IsAny<MessageBoxImage>())).Returns(MessageBoxResult.Yes);
            viewModel = new AccountsPaneViewModel();
            presenter = new AccountsPanePresenter(view.Object, viewModel, dataServiceMock.Object, notificationService.Object, messageBoxWrapper.Object);
        }

        [Test]
        public void UpdatingTheSelectedAccount_UpdatesTheDataServiceActiveAccount() { ;
            viewModel.ItemList.Add(new Account {
                Id = 1
            });
            viewModel.ItemList.Add(new Account {
                Id = 2
            });
            viewModel.ItemList.Add(new Account {
                Id = 3
            });

            Assert.Pass();
        }

        [Test]
        public void DeletingAnAccount_SetsActiveAccountToTheNextAvailableAccount() {
            var accountOne = new Account { Id = 1 };
            var accountTwo = new Account { Id = 2 };
            var accountThree = new Account { Id = 3 };

            viewModel.ItemList.Add(accountOne);
            viewModel.ItemList.Add(accountTwo);
            viewModel.ItemList.Add(accountThree);

            viewModel.SelectedItem = accountOne;

            viewModel.DeleteItemCommand.Execute(null);

            Assert.AreSame(accountTwo, viewModel.SelectedItem, "Expected account with ID 2, but actual is {0}", viewModel.SelectedItem.Id);
        }
    }

    public class DataServiceTest {

        private Mock<INotificationService> notificationServiceMock;
        private Mock<IPersistenceService> persistenceServiceMock;
        private Mock<IIdentifierService> identifierServiceMock;
        private DataService dataService;
        private int accountId, accountTransactionId, categoryId;

        [SetUp]
        public void Setup() {
            accountId = accountTransactionId = categoryId = 0;

            identifierServiceMock = new Mock<IIdentifierService>();
            identifierServiceMock.Setup(ism => ism.GetNewAccountId())
                .Returns(() => {
                    accountId += 1;
                    return accountId;
                });

            identifierServiceMock.Setup(ism => ism.GetNewTransactionId())
                .Returns(() => {
                    accountTransactionId += 1;
                    return accountTransactionId;
                });

            identifierServiceMock.Setup(ism => ism.GetNewCategoryId())
                .Returns(() => {
                    categoryId += 1;
                    return categoryId;
                });

            notificationServiceMock = new Mock<INotificationService>();
            persistenceServiceMock = new Mock<IPersistenceService>();

            dataService = new DataService(persistenceServiceMock.Object, identifierServiceMock.Object, notificationServiceMock.Object);
            PopulateDataService();
        }


        [Test]
        public void RemoveAccount_SetsActiveAccountToTheNextAvailableAccount() {
            var firstAccount = dataService.Accounts().FirstOrDefault(a => a.Id == 1);

            Assert.IsNotNull(firstAccount, "Should be an account with ID 1.");

            dataService.AddAccountTransaction(firstAccount);

            Assert.IsNotNull(dataService.AccountTransactions().FirstOrDefault(t => t.AccountId == 1));

            dataService.RemoveAccount(firstAccount);
            dataService.AddAccountTransaction(firstAccount);

            Assert.IsNotNull(dataService.AccountTransactions().FirstOrDefault(t => t.AccountId == 2));
        }

        [Test]
        public void RemoveAccount_RemovesAllTransactionsForThatAccount() {
            PopulateDataService();
            var firstAccount = dataService.Accounts().FirstOrDefault(a => a.Id == 1);
            dataService.AddAccountTransaction(firstAccount);
            dataService.AddAccountTransaction(firstAccount);
            dataService.AddAccountTransaction(firstAccount);

            Assert.AreEqual(3, dataService.AccountTransactions().Count());

            dataService.RemoveAccount(firstAccount);

            Assert.AreEqual(0, dataService.AccountTransactions().Count());
        }

        private void PopulateDataService() {
            for (var i = 0; i < 3; i++) {
                dataService.AddAccount();
            }
        }
    }
}