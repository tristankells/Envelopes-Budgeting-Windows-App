using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Models;
using Envelopes.Persistence.Helpers;
using Envelopes.Persistence.Importer;
using NUnit.Framework;

namespace Tests.Envelopes.Envelopes.Excel {
    internal class ImportTransactionsTests {
        private static readonly IReadOnlyList<AccountTransaction> ExpectedAccountTransactions = new List<AccountTransaction> {
            new AccountTransaction {
                Date = Convert.ToDateTime("01/08/2020"),
                Payee = "POS W/D AUTOBAHN CAFE-08:44 ;",
                Outflow = 13.40M,
                Inflow = 0M
            },
            new AccountTransaction {
                Date = Convert.ToDateTime("2/08/2020"),
                Payee = "PIZZA HUT WAIRAU GFLD AUCKLAND ;",
                Outflow = 5M,
                Inflow = 0M
            },
            new AccountTransaction {
                Date = Convert.ToDateTime("2/08/2020"),
                Payee = "BURGER FUEL GLENFIELD AUCKLAND ;",
                Outflow = 41.4M,
                Inflow = 0M
            },
            new AccountTransaction {
                Date = Convert.ToDateTime("6/08/2020"),
                Payee = "Direct Credit Aderant Expenses ADERANT LEGAL H ;Ref: Aderant Expenses ADERANT LEGAL H",
                Outflow = 0,
                Inflow = 156M
            }
        };

        private ITransactionsImporter transactionsImporter;

        [SetUp]
        public void Setup() {
            transactionsImporter = new TransactionsImporter();
        }

        [Test]
        public async Task TransactionsImporter_Import_ReturnsTheCorrectTransactions_KiwibankCsv() {
            const string fileLocation = "C:\\Projects\\Envelopes\\Tests.Envelopes\\Envelopes.Excel\\Data\\KiwiBankTestTransactions.csv";

            IEnumerable<AccountTransaction> actualTransactions = await transactionsImporter.Import(fileLocation, ImportHelper.KiwiBankMap);

            AssertExpectedTransactionsMatchActualTransactions(actualTransactions.ToList());
        }

        [Test]
        public async Task TransactionsImporter_Import_ReturnsTheTransactions_KiwibankExcel() {
            const string fileLocation = "C:\\Projects\\Envelopes\\Tests.Envelopes\\Envelopes.Excel\\Data\\KiwiBankTestTransactions.xlsx";

            IEnumerable<AccountTransaction> actualTransactions = await transactionsImporter.Import(fileLocation, ImportHelper.KiwiBankMap);

            AssertExpectedTransactionsMatchActualTransactions(actualTransactions.ToList());
        }

        [Test]
        public async Task TransactionsImporter_Import_ReturnsTheCorrectTransactions_AmexCsv() {
            const string fileLocation = "C:\\Projects\\Envelopes\\Tests.Envelopes\\Envelopes.Excel\\Data\\AmexTestTransactions.csv";

            IEnumerable<AccountTransaction> actualTransactions = await transactionsImporter.Import(fileLocation, ImportHelper.AmexBankMap);

            AssertExpectedTransactionsMatchActualTransactions(actualTransactions.ToList());
        }

        [Test]
        public async Task TransactionsImporter_Import_ReturnsTheCorrectTransactions_PurpleVisaCsv() {
            const string fileLocation = "C:\\Projects\\Envelopes\\Tests.Envelopes\\Envelopes.Excel\\Data\\PurpleVisaTestTransactions.csv";

            IEnumerable<AccountTransaction> actualTransactions = await transactionsImporter.Import(fileLocation, ImportHelper.PurpleVisaBankMap);

            AssertExpectedTransactionsMatchActualTransactions(actualTransactions.ToList());
        }

        [Test]
        public async Task ProxyTransactionsImporter_Import_TransactionsWithDifferentIds() {
            transactionsImporter = new ProxyTransactionImporter();
            IEnumerable<AccountTransaction> actualTransactions = await transactionsImporter.Import(string.Empty, new AccountTransactionColumnMap());
            Assert.IsTrue(actualTransactions.Count(t => t.AccountId == 1) > 0);
            Assert.IsTrue(actualTransactions.Count(t => t.AccountId == 2) > 0);
            Assert.IsTrue(actualTransactions.Count(t => t.AccountId == 3) > 0);
        }

        private static void AssertExpectedTransactionsMatchActualTransactions(IReadOnlyList<AccountTransaction> actualTransactions) {
            Assert.AreEqual(ExpectedAccountTransactions.Count, actualTransactions.Count());

            for (int i = 0; i < ExpectedAccountTransactions.Count; i++) {
                TestValidationHelper.ValidateAccountTransactionsAreEqual(ExpectedAccountTransactions[i], actualTransactions[i]);
            }
        }
    }
}