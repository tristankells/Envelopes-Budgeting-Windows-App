using System;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Data;
using Envelopes.Data.Persistence;
using Envelopes.Models;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;

namespace Tests.Envelopes.Data.Persistence {
    public class ExcelPersistenceServiceTests {
        private const string CategoriesWorksheetName = "Categories";
        private const string AccountTransactionsWorksheetName = "Account Transactions";
        private const string AccountsWorksheetName = "Accounts";

        private ExcelPersistenceService excelPersistenceService;
        private Mock<IExcelFileProcessor> excelFileProcessor;

        [SetUp]
        public void Setup() {
            excelFileProcessor = new Mock<IExcelFileProcessor>();
            excelPersistenceService = new ExcelPersistenceService(excelFileProcessor.Object);
        }


        [Test]
        public async Task SaveApplicationData_TriesToSaveTheCorrectDataToFile_WithCategoryTransactions() {
            var appData = new ApplicationData();
            var account = TestDataSetup.CreateAccount();
            var category = TestDataSetup.CreateCategoryOne();
            var accountTransactionOne = TestDataSetup.CreateAccountTransaction_WithSingleCategoryTransaction();
            var accountTransactionTwo = TestDataSetup.CreateAccountTransaction_WithMultipleCategoryTransactions();
            appData.Accounts.Add(account);
            appData.Categories.Add(category);
            appData.AccountTransactions.Add(accountTransactionOne);
            appData.AccountTransactions.Add(accountTransactionTwo);

            excelFileProcessor.Setup(efp => efp.SaveAs(It.IsAny<ExcelPackage>()))
                .Callback<ExcelPackage>((ep) => {
                    ValidateExcelPackageContainsAccount(ep, account);
                    ValidateExcelPackageContainsCategory(ep, category);
                    ValidateExcelPackageContainsAccountTransaction(ep, accountTransactionOne);
                    ValidateExcelPackageContainsAccountTransaction(ep, accountTransactionTwo, 2);
                }).Verifiable();

            await excelPersistenceService.SaveApplicationData(appData);

            excelFileProcessor.Verify();
        }

        private static void ValidateExcelPackageContainsAccount(ExcelPackage excelPackage, Account account, int rowNumber = 1) {
            var rowNumberIncludingHeader = rowNumber + 1;
            Assert.AreEqual(account.Id, excelPackage.Workbook.Worksheets[AccountsWorksheetName].Cells[rowNumberIncludingHeader, 1].GetValue<int>());
            Assert.AreEqual(account.Name, excelPackage.Workbook.Worksheets[AccountsWorksheetName].Cells[rowNumberIncludingHeader, 2].GetValue<string>());
        }

        private static void ValidateExcelPackageContainsCategory(ExcelPackage excelPackage, Category account, int rowNumber = 1) {
            var rowNumberIncludingHeader = rowNumber + 1;
            Assert.AreEqual(account.Id, excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[rowNumberIncludingHeader, 1].GetValue<int>());
            Assert.AreEqual(account.Name, excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[rowNumberIncludingHeader, 2].GetValue<string>());
            Assert.AreEqual(account.Budgeted, excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[rowNumberIncludingHeader, 3].GetValue<decimal>());
        }

        private static void ValidateExcelPackageContainsAccountTransaction(ExcelPackage excelPackage, AccountTransaction account, int rowNumber = 1) {
            var rowNumberIncludingHeader = rowNumber + 1;
 
            Assert.AreEqual(account.Id, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[rowNumberIncludingHeader, 1].GetValue<int>());
            Assert.AreEqual(account.AccountId, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[rowNumberIncludingHeader, 2].GetValue<int>());
            Assert.AreEqual(account.Date, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[rowNumberIncludingHeader, 3].GetValue<DateTime>());
            Assert.AreEqual(account.PayeeId, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[rowNumberIncludingHeader, 4].GetValue<int>());
            Assert.AreEqual(account.Memo, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[rowNumberIncludingHeader, 5].GetValue<string>());

            for (var i = 0; i < account.CategoryTransactions.Count; i++) {
                var categoryTransactionRowNumber = rowNumberIncludingHeader + i;
                Assert.AreEqual(account.CategoryTransactions[i].CategoryId, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[categoryTransactionRowNumber, 6].GetValue<int>());
                Assert.AreEqual(account.CategoryTransactions[i].Outflow, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[categoryTransactionRowNumber, 7].GetValue<decimal>());
                Assert.AreEqual(account.CategoryTransactions[i].Inflow, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[categoryTransactionRowNumber, 8].GetValue<decimal>());
            }
        }

        [Test]
        public async Task GetApplicationData_ConvertsExcelToTheCorrectApplicationData() {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var account = TestDataSetup.CreateAccount();
            var category = TestDataSetup.CreateCategoryOne();
            var accountTransactionOne = TestDataSetup.CreateAccountTransaction_WithSingleCategoryTransaction();
            var accountTransactionTwo = TestDataSetup.CreateAccountTransaction_WithMultipleCategoryTransactions();

            var excelPackage = new ExcelPackage();
            excelPackage.Workbook.Worksheets.Add(AccountsWorksheetName);
            excelPackage.Workbook.Worksheets[AccountsWorksheetName].Cells[2, 1].Value = account.Id;
            excelPackage.Workbook.Worksheets[AccountsWorksheetName].Cells[2, 2].Value = account.Name;

            excelPackage.Workbook.Worksheets.Add(CategoriesWorksheetName);
            excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 1].Value = category.Id;
            excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 2].Value = category.Name;
            excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 3].Value = category.Budgeted;

            excelPackage.Workbook.Worksheets.Add(AccountTransactionsWorksheetName);
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 1].Value = accountTransactionOne.Id;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 2].Value = accountTransactionOne.AccountId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 3].Value = accountTransactionOne.Date;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 4].Value = accountTransactionOne.PayeeId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 5].Value = accountTransactionOne.Memo;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 6].Value = accountTransactionOne.CategoryTransactions[0].CategoryId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 7].Value = accountTransactionOne.CategoryTransactions[0].Outflow;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 8].Value = accountTransactionOne.CategoryTransactions[0].Inflow;

            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[3, 1].Value = accountTransactionTwo.Id;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[3, 2].Value = accountTransactionTwo.AccountId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[3, 3].Value = accountTransactionTwo.Date;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[3, 4].Value = accountTransactionTwo.PayeeId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[3, 5].Value = accountTransactionTwo.Memo;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[3, 6].Value = accountTransactionTwo.CategoryTransactions[0].CategoryId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[3, 7].Value = accountTransactionTwo.CategoryTransactions[0].Outflow;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[3, 8].Value = accountTransactionTwo.CategoryTransactions[0].Inflow;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[4, 6].Value = accountTransactionTwo.CategoryTransactions[1].CategoryId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[4, 7].Value = accountTransactionTwo.CategoryTransactions[1].Outflow;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[4, 8].Value = accountTransactionTwo.CategoryTransactions[1].Inflow;


            excelFileProcessor.Setup(efp => efp.LoadExcelPackageFromFile())
                .Returns(() => excelPackage);
            
            var applicationData = await excelPersistenceService.GetApplicationData();

            Assert.AreEqual(1, applicationData.Accounts.Count);
            Assert.AreEqual(1, applicationData.Categories.Count);
            Assert.AreEqual(2, applicationData.AccountTransactions.Count);

            ValidateAccountsAreEqual(account, applicationData.Accounts.FirstOrDefault());
            ValidateCategoriesAreEqual(category, applicationData.Categories.FirstOrDefault());
            ValidateAccountTransactionsAreEqual(accountTransactionOne, applicationData.AccountTransactions[0]);
            ValidateAccountTransactionsAreEqual(accountTransactionTwo, applicationData.AccountTransactions[1]);
        }

        private static void ValidateAccountsAreEqual(Account expected, Account actual) {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
        }
        private static void ValidateCategoriesAreEqual(Category expected, Category actual) {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Budgeted, actual.Budgeted);
        }

        private static void ValidateAccountTransactionsAreEqual(AccountTransaction expected, AccountTransaction actual) {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.AccountId, actual.AccountId);
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.PayeeId, actual.PayeeId);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Memo, actual.Memo);

            for (var i = 0; i < expected.CategoryTransactions.Count; i++) {
                Assert.AreEqual(expected.CategoryTransactions[i].CategoryId, actual.CategoryTransactions[i].CategoryId);
                Assert.AreEqual(expected.CategoryTransactions[i].Outflow, actual.CategoryTransactions[i].Outflow);
                Assert.AreEqual(expected.CategoryTransactions[i].Inflow, actual.CategoryTransactions[i].Inflow);
            }
        }
    }
}
