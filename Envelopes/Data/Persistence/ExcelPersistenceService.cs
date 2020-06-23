using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Envelopes.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Envelopes.Data.Persistence {
    class ExcelPersistenceService : IPersistenceService {
        private const string FileName = "Envelopes.xlsx";
        private readonly string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public async Task SaveApplicationData(ApplicationData accounts) {
            await SaveApplicationData(accounts, FileName);
        }

        public async Task SaveApplicationData(ApplicationData data, string fileName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            await Task.Factory.StartNew(() => {
                var accounts = data.Accounts;

                using var package = new ExcelPackage();
                // Add a new worksheet to the empty workbook

                AddAccountsWorksheetToPackage(package, data.Accounts);
                AddCategoriesWorksheetToPackage(package, data.Categories);
                AddAccountTransactionsWorksheetToPackage(package, data.AccountTransactions);

                // Set some document properties
                package.Workbook.Properties.Title = "Envelopes";
                package.Workbook.Properties.Author = "Jan Källman";
                package.Workbook.Properties.Comments =
                    "This sample demonstrates how to create an Excel workbook using EPPlus";

                // Set some extended property values
                package.Workbook.Properties.Company = "EPPlus Software AB";

                // Set some custom property values
                package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Jan Källman");
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");

                var filePath = new FileInfo(Path.Combine(directoryPath, fileName));

                // Save our new workbook in the output directory and we are done!
                package.SaveAs(filePath);
            });
        }

        private static void AddAccountsWorksheetToPackage(ExcelPackage package, IList<Account> accounts) {
            var worksheet = package.Workbook.Worksheets.Add("Accounts");
            // Add the headers
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Name";

            // For each account, add a corresponding row
            for (var i = 0; i < accounts.Count; i++) {
                worksheet.Cells["A" + (i + 2)].Value = accounts[i].Id;
                worksheet.Cells["B" + (i + 2)].Value = accounts[i].Name;
            }

            worksheet.Cells.AutoFitColumns(0); //Autofit columns for all cells

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

        private static void AddCategoriesWorksheetToPackage(ExcelPackage package, IList<Category> categories) {
            var worksheet = package.Workbook.Worksheets.Add("Categories");

            // Add the headers
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Budgeted";

            // For each account, add a corresponding row
            for (var i = 0; i < categories.Count; i++) {
                worksheet.Cells["A" + (i + 2)].Value = categories[i].Id;
                worksheet.Cells["B" + (i + 2)].Value = categories[i].Name;
                worksheet.Cells["c" + (i + 2)].Value = categories[i].Budgeted;
            }

            worksheet.Cells.AutoFitColumns(0); //Autofit columns for all cells

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

        private static void AddAccountTransactionsWorksheetToPackage(ExcelPackage package,
            IList<AccountTransaction> transactions) {
            var worksheet = package.Workbook.Worksheets.Add("Account Transactions");

            // Add the headers
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Account Id";
            worksheet.Cells[1, 3].Value = "Date";
            worksheet.Cells[1, 4].Value = "Payee Id";
            worksheet.Cells[1, 5].Value = "Category Id";
            worksheet.Cells[1, 6].Value = "Memo";
            worksheet.Cells[1, 7].Value = "Outflow";
            worksheet.Cells[1, 8].Value = "Inflow";

            // For each account, add a corresponding row
            for (var i = 0; i < transactions.Count; i++) {
                worksheet.Cells["A" + (i + 2)].Value = transactions[i].Id;
                worksheet.Cells["B" + (i + 2)].Value = transactions[i].AccountId;
                worksheet.Cells["C" + (i + 2)].Value = transactions[i].Date;
                worksheet.Cells["D" + (i + 2)].Value = transactions[i].PayeeId;
                worksheet.Cells["E" + (i + 2)].Value = transactions[i].CategoryId;
                worksheet.Cells["F" + (i + 2)].Value = transactions[i].Memo;
                worksheet.Cells["G" + (i + 2)].Value = transactions[i].Outflow;
                worksheet.Cells["H" + (i + 2)].Value = transactions[i].Inflow;
            }

            worksheet.Cells.AutoFitColumns(0); //Autofit columns for all cells

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
            return await GetApplicationData(FileName);
        }


        public async Task<ApplicationData> GetApplicationData(string fileName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            return await Task.Factory.StartNew(() => {
                var applicationData = new ApplicationData();

                var filePath = new FileInfo(Path.Combine(directoryPath, fileName));
                using ExcelPackage package = new ExcelPackage(filePath);

                //Get the first worksheet in the workbook
                ExcelWorksheet accountsWorksheet = package.Workbook.Worksheets["Accounts"];
                if (accountsWorksheet == null) return applicationData;

                var isAccountRowValid = true;
                for (var row = 2; isAccountRowValid; row++) {
                    if (accountsWorksheet.Cells[row, 1].Value != null) {
                        // If current row, does not have an Id (col = 1), then not a valid row.
                        applicationData.Accounts.Add(new Account() {
                            Id = accountsWorksheet.Cells[row, 1].GetValue<int>(),
                            Name = accountsWorksheet.Cells[row, 2].GetValue<string>()
                        });
                    }
                    else {
                        isAccountRowValid = false;
                    }
                }

                //Get the first worksheet in the workbook
                ExcelWorksheet categoriesWorksheet = package.Workbook.Worksheets["Categories"];
                if (categoriesWorksheet == null) return applicationData;

                var isCategoryRowValid = true;
                for (var row = 2; isCategoryRowValid; row++) {
                    if (categoriesWorksheet.Cells[row, 1].Value != null) {
                        // If current row, does not have an Id (col = 1), then not a valid row.
                        applicationData.Categories.Add(new Category() {
                            Id = categoriesWorksheet.Cells[row, 1].GetValue<int>(),
                            Name = categoriesWorksheet.Cells[row, 2].GetValue<string>(),
                            Budgeted = categoriesWorksheet.Cells[row, 3].GetValue<decimal>(),
                        });
                    }
                    else {
                        isCategoryRowValid = false;
                    }
                }

                //Get the first worksheet in the workbook
                ExcelWorksheet accountTransactionsWorksheet = package.Workbook.Worksheets["Account Transactions"];
                if (accountTransactionsWorksheet == null) return applicationData;

                var isAccountTransactionRowValid = true;
                for (var row = 2; isAccountTransactionRowValid; row++) {
                    if (accountTransactionsWorksheet.Cells[row, 1].Value != null) {
                        // If current row, does not have an Id (col = 1), then not a valid row.
                        applicationData.AccountTransactions.Add(new AccountTransaction() {
                            Id = accountTransactionsWorksheet.Cells[row, 1].GetValue<int>(),
                            AccountId = accountTransactionsWorksheet.Cells[row, 2].GetValue<int>(),
                            Date = accountTransactionsWorksheet.Cells[row, 3].GetValue<DateTime>(),
                            PayeeId = accountTransactionsWorksheet.Cells[row, 4].GetValue<int>(),
                            CategoryId = accountTransactionsWorksheet.Cells[row, 5].GetValue<int>(),
                            Memo = accountTransactionsWorksheet.Cells[row, 6].GetValue<string>(),
                            Outflow = accountTransactionsWorksheet.Cells[row, 7].GetValue<decimal>(),
                            Inflow = accountTransactionsWorksheet.Cells[row, 8].GetValue<decimal>(),
                        });
                    }
                    else {
                        isAccountTransactionRowValid = false;
                    }
                }

                return applicationData;
            });
        }
    }
}