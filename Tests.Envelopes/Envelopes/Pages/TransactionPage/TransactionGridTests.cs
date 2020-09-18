using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Envelopes.Data;
using Envelopes.Data.Persistence;
using Envelopes.Models;
using Envelopes.Pages.TransactionsPage.TransactionsGrid;
using Envelopes.Persistence.Importer;
using Moq;
using NUnit.Framework;

namespace Tests.Envelopes.Envelopes.Pages.TransactionPage {
    [Apartment(ApartmentState.STA)]
    internal class TransactionGridTests : PageTestBase {
        private TransactionsGridPresenter transactionsGridPresenter;
        private Mock<ITransactionsGridView> transactionsGridView;
        private TransactionsGridViewModel transactionsGridViewModel;

        [SetUp]
        public void Setup() {
            SetupServices();

            transactionsGridViewModel = new TransactionsGridViewModel();
            transactionsGridView = new Mock<ITransactionsGridView>();
            transactionsGridPresenter = new TransactionsGridPresenter(transactionsGridView.Object, transactionsGridViewModel, DataService, NotificationService, TransactionsImporter.Object);
        }

        [Test]
        public async Task OnTransactionImport_AddsCorrectNumberOfTransactionsToGrid() {
            TransactionsImporter.Setup(importer => importer.Import(It.IsAny<string>(), It.IsAny<AccountTransactionColumnMap>())).ReturnsAsync(
                new List<AccountTransaction> {
                    new AccountTransaction {
                        AccountId = 1,
                        Date = DateTime.Today,
                        Outflow = 20
                    },
                    new AccountTransaction {
                        AccountId = 2,
                        Date = DateTime.Today,
                        Outflow = 20
                    },
                    new AccountTransaction {
                        AccountId = 3,
                        Date = DateTime.Today,
                        Outflow = 20
                    }
                }
            );
            await transactionsGridViewModel.ImportTransactionsCommand.ExecuteAsync();
            Assert.AreEqual(3, transactionsGridViewModel.AccountTransactions.Count);
        }

        [Test]
        public async Task OnTransactionImport_WillNotDuplicateTransactions_WithMatchingAmountsAndAccountsAndDates() {
            transactionsGridViewModel.AccountTransactions.Add(new AccountTransaction {
                AccountId = 1,
                Date = DateTime.Today,
                Outflow = 20
            });

            TransactionsImporter.Setup(importer => importer.Import(It.IsAny<string>(), It.IsAny<AccountTransactionColumnMap>())).ReturnsAsync(
                new List<AccountTransaction> {
                    new AccountTransaction {
                        AccountId = 1,
                        Date = DateTime.Today,
                        Outflow = 20
                    },
                    new AccountTransaction {
                        AccountId = 2,
                        Date = DateTime.Today,
                        Outflow = 20
                    },
                    new AccountTransaction {
                        AccountId = 1,
                        Date = DateTime.Today.AddDays(1),
                        Outflow = 20
                    },
                    new AccountTransaction {
                        AccountId = 1,
                        Date = DateTime.Today,
                        Outflow = 30
                    }
                }
            );

            await transactionsGridViewModel.ImportTransactionsCommand.ExecuteAsync();
            Assert.AreEqual(4, transactionsGridViewModel.AccountTransactions.Count);
        }
    }

    // Setups all the shared services that will be used by multiple classes.
    // This way if we make big changes to how are services are consumed should have less places to make changes.
    public class PageTestBase {
        protected IDataService DataService;

        protected Mock<IFileProcessor> FileProcessor;
        protected IIdentifierService IdentifierService;
        protected INotificationService NotificationService;
        protected IPersistenceService PersistenceService;
        protected Mock<ITransactionsImporter> TransactionsImporter;

        protected void SetupServices() {
            FileProcessor = new Mock<IFileProcessor>();
            TransactionsImporter = new Mock<ITransactionsImporter>();
            PersistenceService = new ExcelPersistenceService(FileProcessor.Object);
            NotificationService = new NotificationService();
            IdentifierService = new IdentifierService();
            DataService = new DataService(PersistenceService, IdentifierService, NotificationService);
        }
    }
}