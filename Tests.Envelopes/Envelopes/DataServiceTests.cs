using System.Collections.Generic;
using System.Threading.Tasks;
using Envelopes.Data;
using Envelopes.Data.Persistence;
using Envelopes.Models;
using Envelopes.Models.Models;
using Moq;
using NUnit.Framework;

namespace Tests.Envelopes {
    internal class DataServiceTests {
        private DataService dataService;
        private Mock<IIdentifierService> identifierService;
        private Mock<INotificationService> notificationService;
        private Mock<IPersistenceService> persistenceService;

        [SetUp]
        public void Setup() {
            persistenceService = new Mock<IPersistenceService>();
            identifierService = new Mock<IIdentifierService>();
            notificationService = new Mock<INotificationService>();
            dataService = new DataService(persistenceService.Object, identifierService.Object, notificationService.Object);
        }

        [Test]
        public async Task RemainingToBudget_IsCorrect_PositiveBalance() {
            persistenceService.Setup(ps => ps.GetApplicationData()).Returns(
                Task.Factory.StartNew(() => {
                    var applicationData = new ApplicationData {
                        Categories = new List<Category> {
                            new Category {
                                Id = 1,
                                Name = "Savings",
                                Budgeted = 10.0M
                            }
                        },
                        Accounts = new List<Account> {
                            new Account {
                                Id = 1,
                                Name = "Kiwibank"
                            }
                        },
                        AccountTransactions = new List<AccountTransaction> {
                            new AccountTransaction {
                                Id = 1,
                                AccountId = 1,
                                Inflow = 15.0M,
                                Memo = "Starting Balance"
                            }
                        }
                    };

                    return applicationData;
                }));

            await dataService.LoadApplicationData();

            decimal remainingAccountBalance = dataService.GetRemainingAccountBalanceToBudget();

            Assert.AreEqual(5, remainingAccountBalance);
        }

        [Test]
        public async Task RemainingToBudget_IsCorrect_NegativeBalance() {
            persistenceService.Setup(ps => ps.GetApplicationData()).Returns(
                Task.Factory.StartNew(() => {
                    var applicationData = new ApplicationData {
                        Categories = new List<Category> {
                            new Category {
                                Id = 1,
                                Name = "Savings",
                                Budgeted = 10.0M
                            }
                        },
                        Accounts = new List<Account> {
                            new Account {
                                Id = 1,
                                Name = "Kiwibank"
                            }
                        },
                        AccountTransactions = new List<AccountTransaction> {
                            new AccountTransaction {
                                Id = 1,
                                AccountId = 1,
                                Inflow = 5.0M,
                                Memo = "Starting Balance"
                            }
                        }
                    };

                    return applicationData;
                }));

            await dataService.LoadApplicationData();

            decimal remainingAccountBalance = dataService.GetRemainingAccountBalanceToBudget();

            Assert.AreEqual(-5, remainingAccountBalance);
        }

        [Test]
        public async Task RemainingToBudget_IsCorrect_ZeroBalance() {
            persistenceService.Setup(ps => ps.GetApplicationData()).Returns(
                Task.Factory.StartNew(() => {
                    var applicationData = new ApplicationData {
                        Categories = new List<Category> {
                            new Category {
                                Id = 1,
                                Name = "Savings",
                                Budgeted = 10.0M
                            }
                        },
                        Accounts = new List<Account> {
                            new Account {
                                Id = 1,
                                Name = "Kiwibank"
                            }
                        },
                        AccountTransactions = new List<AccountTransaction> {
                            new AccountTransaction {
                                Id = 1,
                                AccountId = 1,
                                Inflow = 10.0M,
                                Memo = "Starting Balance"
                            }
                        }
                    };

                    return applicationData;
                }));

            await dataService.LoadApplicationData();

            decimal remainingAccountBalance = dataService.GetRemainingAccountBalanceToBudget();

            Assert.AreEqual(0, remainingAccountBalance);
        }
    }
}