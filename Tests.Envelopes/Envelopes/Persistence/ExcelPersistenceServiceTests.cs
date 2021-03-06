﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Data;
using Envelopes.Data.Persistence;
using Envelopes.Models;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;

namespace Tests.Envelopes.Envelopes.Persistence {
    public class ExcelPersistenceServiceTests {
        private const string CategoriesWorksheetName = "Categories";
        private const string AccountTransactionsWorksheetName = "Account Transactions";
        private const string AccountsWorksheetName = "Accounts";
        private Mock<IFileProcessor> excelFileProcessor;

        private ExcelPersistenceService excelPersistenceService;

        [SetUp]
        public void Setup() {
            excelFileProcessor = new Mock<IFileProcessor>();
            excelPersistenceService = new ExcelPersistenceService(excelFileProcessor.Object);
        }

        [Test]
        public async Task SaveApplicationData_TriesToSaveTheCorrectDataToFile() {
            var appData = new ApplicationData();
            Account account = TestDataSetup.CreateAccount();
            Category category = TestDataSetup.CreateCategory();
            AccountTransaction accountTransaction = TestDataSetup.CreateAccountTransaction();
            appData.Accounts.Add(account);
            appData.Categories.Add(category);
            appData.AccountTransactions.Add(accountTransaction);

            excelFileProcessor.Setup(efp => efp.SaveAs(It.IsAny<ExcelPackage>()))
                .Callback<ExcelPackage>(ep => {
                    ValidateExcelPackageContainsAccount(ep, account);
                    ValidateExcelPackageContainsCategory(ep, category);
                    ValidateExcelPackageContainsAccountTransaction(ep, accountTransaction);
                });

            await excelPersistenceService.SaveApplicationData(appData);

            excelFileProcessor.Verify(efp => efp.SaveAs(It.IsAny<ExcelPackage>()), Times.Once);
        }

        private static void ValidateExcelPackageContainsAccount(ExcelPackage excelPackage, Account account) {
            Assert.AreEqual(account.Id, excelPackage.Workbook.Worksheets[AccountsWorksheetName].Cells[2, 1].GetValue<int>());
            Assert.AreEqual(account.Name, excelPackage.Workbook.Worksheets[AccountsWorksheetName].Cells[2, 2].GetValue<string>());
        }

        private static void ValidateExcelPackageContainsCategory(ExcelPackage excelPackage, Category account) {
            Assert.AreEqual(account.Id, excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 1].GetValue<int>());
            Assert.AreEqual(account.Name, excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 2].GetValue<string>());
            Assert.AreEqual(account.Budgeted, excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 3].GetValue<decimal>());
        }

        private static void ValidateExcelPackageContainsAccountTransaction(ExcelPackage excelPackage, AccountTransaction account) {
            Assert.AreEqual(account.AccountId, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 2].GetValue<int>());
            Assert.AreEqual(account.Date, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 3].GetValue<DateTime>());
            Assert.AreEqual(account.Payee, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 4].GetValue<string>());
            Assert.AreEqual(account.CategoryId, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 5].GetValue<int>());
            Assert.AreEqual(account.Memo, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 6].GetValue<string>());
            Assert.AreEqual(account.Outflow, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 7].GetValue<decimal>());
            Assert.AreEqual(account.Inflow, excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 8].GetValue<decimal>());
        }

        [Test]
        public async Task GetApplicationData_ConvertsExcelToTheCorrectApplicationData() {
            Account account = TestDataSetup.CreateAccount();
            Category category = TestDataSetup.CreateCategory();
            AccountTransaction accountTransaction = TestDataSetup.CreateAccountTransaction();

            var excelPackage = new ExcelPackage();
            excelPackage.Workbook.Worksheets.Add(AccountsWorksheetName);
            excelPackage.Workbook.Worksheets[AccountsWorksheetName].Cells[2, 1].Value = account.Id;
            excelPackage.Workbook.Worksheets[AccountsWorksheetName].Cells[2, 2].Value = account.Name;

            excelPackage.Workbook.Worksheets.Add(CategoriesWorksheetName);
            excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 1].Value = category.Id;
            excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 2].Value = category.Name;
            excelPackage.Workbook.Worksheets[CategoriesWorksheetName].Cells[2, 3].Value = category.Budgeted;

            excelPackage.Workbook.Worksheets.Add(AccountTransactionsWorksheetName);
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 2].Value = accountTransaction.AccountId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 3].Value = accountTransaction.Date;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 4].Value = accountTransaction.Payee;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 5].Value = accountTransaction.CategoryId;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 6].Value = accountTransaction.Memo;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 7].Value = accountTransaction.Outflow;
            excelPackage.Workbook.Worksheets[AccountTransactionsWorksheetName].Cells[2, 8].Value = accountTransaction.Inflow;

            excelFileProcessor.Setup(efp => efp.LoadExcelPackageFromFile())
                .Returns(() => excelPackage);

            ApplicationData applicationData = await excelPersistenceService.GetApplicationData();

            Assert.AreEqual(1, applicationData.Accounts.Count);
            Assert.AreEqual(1, applicationData.Categories.Count);
            Assert.AreEqual(1, applicationData.AccountTransactions.Count);

            TestValidationHelper.ValidateAccountsAreEqual(account, applicationData.Accounts.FirstOrDefault());
            TestValidationHelper.ValidateCategoriesAreEqual(category, applicationData.Categories.FirstOrDefault());
            TestValidationHelper.ValidateAccountTransactionsAreEqual(accountTransaction, applicationData.AccountTransactions.FirstOrDefault());
        }
    }
}