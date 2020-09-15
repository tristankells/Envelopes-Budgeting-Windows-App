using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Envelopes.Models;
using Envelopes.Models.Models;
using OfficeOpenXml;

namespace Envelopes.Data.Persistence {
    public interface IPersistenceService {
        Task SaveApplicationData(ApplicationData data);
        Task<ApplicationData> GetApplicationData();
    }

    public class ExcelPersistenceService : IPersistenceService {
        private readonly IFileProcessor fileProcessor;
        private bool saveInProgress;

        public ExcelPersistenceService(IFileProcessor fileProcessor) {
            this.fileProcessor = fileProcessor;
        }

        public async Task SaveApplicationData(ApplicationData data) {
            if (saveInProgress) {
                return;
            }

            saveInProgress = true;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            await Task.Factory.StartNew(async () => {
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
                await fileProcessor.SaveAs(package);
            });

            saveInProgress = false;
        }

        public async Task<ApplicationData> GetApplicationData() {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            return await Task.Factory.StartNew(() => {
                var applicationData = new ApplicationData();
                using ExcelPackage package = fileProcessor.LoadExcelPackageFromFile();

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
                    applicationData.AccountTransactions = ParseAccountTransactionFromExcelWorkSheet(accountTransactionsWorksheet);
                }

                return applicationData;
            });
        }

        private void AddApplicationDataToExcelPackage(ExcelPackage package, ApplicationData data) {
            AddAccountsWorksheetToExcelPackage(package, data.Accounts);
            AddCategoriesWorksheetToExcelPackage(package, data.Categories);
            AddAccountTransactionsWorksheetToExcelPackage(package, data.AccountTransactions);
        }

        private static void AddAccountsWorksheetToExcelPackage(ExcelPackage package, IList<Account> accounts) {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Accounts");
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Name";

            for (int i = 0; i < accounts.Count; i++) {
                worksheet.Cells["A" + (i + 2)].Value = accounts[i].Id;
                worksheet.Cells["B" + (i + 2)].Value = accounts[i].Name;
            }

            SetDefaultWorksheetValues(worksheet);
        }

        private static void AddCategoriesWorksheetToExcelPackage(ExcelPackage package, IList<Category> categories) {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Categories");

            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Budgeted";

            for (int i = 0; i < categories.Count; i++) {
                worksheet.Cells["A" + (i + 2)].Value = categories[i].Id;
                worksheet.Cells["B" + (i + 2)].Value = categories[i].Name;
                worksheet.Cells["c" + (i + 2)].Value = categories[i].Budgeted;
            }

            SetDefaultWorksheetValues(worksheet);
        }

        private static void AddAccountTransactionsWorksheetToExcelPackage(ExcelPackage package, IList<AccountTransaction> transactions) {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Account Transactions");

            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Account Id";
            worksheet.Cells[1, 3].Value = "Date";
            worksheet.Cells[1, 4].Value = "Payee Id";
            worksheet.Cells[1, 5].Value = "Category Id";
            worksheet.Cells[1, 6].Value = "Memo";
            worksheet.Cells[1, 7].Value = "Outflow";
            worksheet.Cells[1, 8].Value = "Inflow";

            for (int i = 0; i < transactions.Count; i++) {
                worksheet.Cells["A" + (i + 2)].Value = transactions[i].Id;
                worksheet.Cells["B" + (i + 2)].Value = transactions[i].AccountId;
                worksheet.Cells["C" + (i + 2)].Value = transactions[i].Date;
                worksheet.Cells["D" + (i + 2)].Value = transactions[i].Payee;
                worksheet.Cells["E" + (i + 2)].Value = transactions[i].CategoryId;
                worksheet.Cells["F" + (i + 2)].Value = transactions[i].Memo;
                worksheet.Cells["G" + (i + 2)].Value = transactions[i].Outflow;
                worksheet.Cells["H" + (i + 2)].Value = transactions[i].Inflow;
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

        private List<Account> ParseAccountsFromExcelWorkSheet(ExcelWorksheet worksheet) {
            var accounts = new List<Account>();
            bool isAccountRowValid = true;
            for (int row = 2; isAccountRowValid; row++) {
                if (worksheet.Cells[row, 1].Value != null) {
                    // If current row, does not have an Id (col = 1), then not a valid row.
                    accounts.Add(new Account {
                        Id = worksheet.Cells[row, 1].GetValue<int>(),
                        Name = worksheet.Cells[row, 2].GetValue<string>()
                    });
                } else {
                    isAccountRowValid = false;
                }
            }

            return accounts;
        }

        private List<Category> ParseCategoriesFromExcelWorkSheet(ExcelWorksheet worksheet) {
            var categories = new List<Category>();
            bool isCategoryRowValid = true;
            for (int row = 2; isCategoryRowValid; row++) {
                if (worksheet.Cells[row, 1].Value != null) {
                    // If current row, does not have an Id (col = 1), then not a valid row.
                    categories.Add(new Category {
                        Id = worksheet.Cells[row, 1].GetValue<int>(),
                        Name = worksheet.Cells[row, 2].GetValue<string>(),
                        Budgeted = worksheet.Cells[row, 3].GetValue<decimal>()
                    });
                } else {
                    isCategoryRowValid = false;
                }
            }

            return categories;
        }

        private List<AccountTransaction> ParseAccountTransactionFromExcelWorkSheet(ExcelWorksheet worksheet) {
            var accountTransactions = new List<AccountTransaction>();
            bool isAccountTransactionRowValid = true;
            for (int row = 2; isAccountTransactionRowValid; row++) {
                if (worksheet.Cells[row, 1].Value != null) {
                    // If current row, does not have an Id (col = 1), then not a valid row.
                    accountTransactions.Add(new AccountTransaction {
                        Id = worksheet.Cells[row, 1].GetValue<int>(),
                        AccountId = worksheet.Cells[row, 2].GetValue<int>(),
                        Date = worksheet.Cells[row, 3].GetValue<DateTime>(),
                        Payee = worksheet.Cells[row, 4].GetValue<string>(),
                        CategoryId = worksheet.Cells[row, 5].GetValue<int>(),
                        Memo = worksheet.Cells[row, 6].GetValue<string>(),
                        Outflow = worksheet.Cells[row, 7].GetValue<decimal>(),
                        Inflow = worksheet.Cells[row, 8].GetValue<decimal>()
                    });
                } else {
                    isAccountTransactionRowValid = false;
                }
            }

            return accountTransactions;
        }
    }
}