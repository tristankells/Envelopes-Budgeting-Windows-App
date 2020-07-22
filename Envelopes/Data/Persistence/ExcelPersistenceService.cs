using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Envelopes.Models;
using OfficeOpenXml;

namespace Envelopes.Data.Persistence {
    public interface IExcelFileProcessor {
        public void SaveAs(ExcelPackage package);
        public ExcelPackage LoadExcelPackageFromFile();
    }

    public class ExcelFileProcessor : IExcelFileProcessor {
        private const string FileName = "Envelopes.xlsx";
        private readonly string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public void SaveAs(ExcelPackage package) {
            var filePath = new FileInfo(Path.Combine(directoryPath, FileName));
            package.SaveAs(filePath);
        }

        public ExcelPackage LoadExcelPackageFromFile() {
            var filePath = new FileInfo(Path.Combine(directoryPath, FileName));
            return new ExcelPackage(filePath);
        }
    }

    public class ExcelPersistenceService : IPersistenceService {
        private readonly IExcelFileProcessor excelFileProcessor;

        public ExcelPersistenceService(IExcelFileProcessor excelFileProcessor) {
            this.excelFileProcessor = excelFileProcessor;
        }

        public async Task SaveApplicationData(ApplicationData data) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            await Task.Factory.StartNew(() => {
                using var package = new ExcelPackage();
                // Add a new worksheet to the empty workbook
                AddApplicationDataToExcelPackage(package, data);

                // Set some document properties
                package.Workbook.Properties.Title = "Envelopes";
                package.Workbook.Properties.Author = "Tristan Kells";
                package.Workbook.Properties.Comments =
                    "This sample demonstrates how to create an Excel workbook using EPPlus";

                // Set some extended property values
                package.Workbook.Properties.Company = "EPPlus Software AB";

                // Set some custom property values
                package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Jan Källman");
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");

                // Save our new workbook in the output directory and we are done!
                excelFileProcessor.SaveAs(package);
            });
        }

        private void AddApplicationDataToExcelPackage(ExcelPackage package, ApplicationData data) {
            AddAccountsWorksheetToExcelPackage(package, data.Accounts);
            AddCategoriesWorksheetToExcelPackage(package, data.Categories);
            AddAccountTransactionsWorksheetToExcelPackage(package, data.AccountTransactions);
        }

        private static void AddAccountsWorksheetToExcelPackage(ExcelPackage package, IList<Account> accounts) {
            var worksheet = package.Workbook.Worksheets.Add("Accounts");
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Name";

            for (var i = 0; i < accounts.Count; i++) {
                worksheet.Cells["A" + (i + 2)].Value = accounts[i].Id;
                worksheet.Cells["B" + (i + 2)].Value = accounts[i].Name;
            }

            SetDefaultWorksheetValues(worksheet);
        }

        private static void AddCategoriesWorksheetToExcelPackage(ExcelPackage package, IList<Category> categories) {
            var worksheet = package.Workbook.Worksheets.Add("Categories");

            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Budgeted";

            for (var i = 0; i < categories.Count; i++) {
                worksheet.Cells["A" + (i + 2)].Value = categories[i].Id;
                worksheet.Cells["B" + (i + 2)].Value = categories[i].Name;
                worksheet.Cells["c" + (i + 2)].Value = categories[i].Budgeted;
            }

            SetDefaultWorksheetValues(worksheet);
        }

        private static void AddAccountTransactionsWorksheetToExcelPackage(ExcelPackage package,
            IList<AccountTransaction> transactions) {
            var worksheet = package.Workbook.Worksheets.Add("Account Transactions");
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Account Id";
            worksheet.Cells[1, 3].Value = "Date";
            worksheet.Cells[1, 4].Value = "Payee Id";
            worksheet.Cells[1, 5].Value = "Memo";
            worksheet.Cells[1, 6].Value = "Category Id";
            worksheet.Cells[1, 7].Value = "Outflow";
            worksheet.Cells[1, 8].Value = "Inflow";

            var startingRowAfterHeader = 2;
            for (var i = 0; i < transactions.Count; i++) {
                worksheet.Cells[i + startingRowAfterHeader, 1].Value = transactions[i].Id;
                worksheet.Cells[i + startingRowAfterHeader, 2].Value = transactions[i].AccountId;
                worksheet.Cells[i + startingRowAfterHeader, 3].Value = transactions[i].Date;
                worksheet.Cells[i + startingRowAfterHeader, 4].Value = transactions[i].PayeeId;
                worksheet.Cells[i + startingRowAfterHeader, 5].Value = transactions[i].Memo;

                for (var j = 0; j < transactions[i].CategoryTransactions.Count; j++) {
                    worksheet.Cells[i + j + startingRowAfterHeader, 6].Value =
                        transactions[i].CategoryTransactions[j].CategoryId;
                    worksheet.Cells[i + j + startingRowAfterHeader, 7].Value =
                        transactions[i].CategoryTransactions[j].Outflow;
                    worksheet.Cells[i + j + startingRowAfterHeader, 8].Value =
                        transactions[i].CategoryTransactions[j].Inflow;
                }
            }

            SetDefaultWorksheetValues(worksheet);
        }

        private static void SetDefaultWorksheetValues(ExcelWorksheet worksheet) {
            //Autofit columns for all cells
            worksheet.Cells.AutoFitColumns(0);

            // Lets set the header text 
            worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Accounts";

            // Add the page number to the footer plus the total number of pages
            worksheet.HeaderFooter.OddFooter.RightAlignedText =
                string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);

            // Add the sheet name to the footer
            worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;

            // Add the file fileName to the footer
            worksheet.HeaderFooter.OddFooter.LeftAlignedText =
                ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;
            worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
            worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

            // Change the sheet view to show it in page layout mode
            worksheet.View.PageLayoutView = true;
        }

        public async Task<ApplicationData> GetApplicationData() {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            return await Task.Factory.StartNew(() => {
                var applicationData = new ApplicationData();
                using ExcelPackage package = excelFileProcessor.LoadExcelPackageFromFile();

                ExcelWorksheet accountsWorksheet = package.Workbook.Worksheets["Accounts"];
                if (accountsWorksheet != null) {
                    applicationData.Accounts = ParseAccountsFromExcelWorkSheet(accountsWorksheet);
                }

                ExcelWorksheet categoriesWorksheet = package.Workbook.Worksheets["Categories"];
                if (accountsWorksheet != null) {
                    applicationData.Categories = ParseCategoriesFromExcelWorkSheet(categoriesWorksheet);
                }

                ExcelWorksheet accountTransactionsWorksheet = package.Workbook.Worksheets["Account Transactions"];
                if (accountsWorksheet != null) {
                    applicationData.AccountTransactions =
                        ParseAccountTransactionFromExcelWorkSheet(accountTransactionsWorksheet);
                }

                return applicationData;
            });
        }

        private List<Account> ParseAccountsFromExcelWorkSheet(ExcelWorksheet worksheet) {
            var accounts = new List<Account>();
            var isAccountRowValid = true;
            for (var row = 2; isAccountRowValid; row++) {
                if (worksheet.Cells[row, 1].Value != null) {
                    // If current row, does not have an Id (col = 1), then not a valid row.
                    accounts.Add(new Account() {
                        Id = worksheet.Cells[row, 1].GetValue<int>(),
                        Name = worksheet.Cells[row, 2].GetValue<string>()
                    });
                }
                else {
                    isAccountRowValid = false;
                }
            }

            return accounts;
        }

        private List<Category> ParseCategoriesFromExcelWorkSheet(ExcelWorksheet worksheet) {
            var categories = new List<Category>();
            var isCategoryRowValid = true;
            for (var row = 2; isCategoryRowValid; row++) {
                if (worksheet.Cells[row, 1].Value != null) {
                    // If current row, does not have an Id (col = 1), then not a valid row.
                    categories.Add(new Category() {
                        Id = worksheet.Cells[row, 1].GetValue<int>(),
                        Name = worksheet.Cells[row, 2].GetValue<string>(),
                        Budgeted = worksheet.Cells[row, 3].GetValue<decimal>(),
                    });
                } else {
                    isCategoryRowValid = false;
                }
            }

            return categories;
        }

        private List<AccountTransaction> ParseAccountTransactionFromExcelWorkSheet(ExcelWorksheet worksheet) {
            var accountTransactions = new List<AccountTransaction>();
            var isValidAccountTransaction = true;
            for (var row = 2; isValidAccountTransaction; row++) {
                if (worksheet.Cells[row, 1].Value != null) {
                    var accountTransaction = CreateAccountTransactionFromWorksheetRow(worksheet, row);
                    var isValidCategoryTransaction = true;
                    for (var i = 0; isValidCategoryTransaction; i++) {
                        var catergoryRowNumber = row + i + 1;
                        if (IsRowValidCategory(worksheet, catergoryRowNumber)) {
                            var categoryTransaction =
                                CreateCategoryTransactionFromWorksheetRow(worksheet, catergoryRowNumber);
                            accountTransaction.CategoryTransactions.Add(categoryTransaction);
                        }
                        else {
                            row += i;
                            isValidCategoryTransaction = false;
                        }
                    }

                    accountTransactions.Add(accountTransaction);
                }
                else {
                    isValidAccountTransaction = false;
                }
            }

            return accountTransactions;
        }

        private static AccountTransaction CreateAccountTransactionFromWorksheetRow(ExcelWorksheet worksheet,
            int rowNumber) {
            var accountTransaction = new AccountTransaction {
                Id = worksheet.Cells[rowNumber, 1].GetValue<int>(),
                AccountId = worksheet.Cells[rowNumber, 2].GetValue<int>(),
                Date = worksheet.Cells[rowNumber, 3].GetValue<DateTime>(),
                PayeeId = worksheet.Cells[rowNumber, 4].GetValue<int>(),
                Memo = worksheet.Cells[rowNumber, 5].GetValue<string>()
            };
            var categoryTransaction = CreateCategoryTransactionFromWorksheetRow(worksheet, rowNumber);
            accountTransaction.CategoryTransactions.Add(categoryTransaction);

            return accountTransaction;
        }

        private static CategoryTransaction CreateCategoryTransactionFromWorksheetRow(ExcelWorksheet worksheet,
            int rowNumber) {
            var categoryTransaction = new CategoryTransaction {
                CategoryId = worksheet.Cells[rowNumber, 6].GetValue<int>(),
                Outflow = worksheet.Cells[rowNumber, 7].GetValue<decimal>(),
                Inflow = worksheet.Cells[rowNumber, 8].GetValue<decimal>()
            };

            return categoryTransaction;
        }

        private static bool IsRowValidCategory(ExcelWorksheet worksheet, int rowNumber) {
            return worksheet.Cells[rowNumber, 1].Value == null && (worksheet.Cells[rowNumber, 6].Value != null ||
                                                                   worksheet.Cells[rowNumber, 7].Value != null ||
                                                                   worksheet.Cells[rowNumber, 8].Value != null);
        }
    }
}