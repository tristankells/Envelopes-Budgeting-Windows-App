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

        public async Task SaveAccounts(IList<Account> accounts) {
            await SaveAccounts(accounts, FileName);
        }

        private WriteAccount() {

        }

        public async Task SaveAccount(Account account, string fileName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            await Task.Factory.StartNew(() => {
                using var package = new ExcelPackage();
                // Add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add("Accounts");
                // Add the headers
                worksheet.Cells[1, 1].Value = "ID";
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

        public async Task SaveAccounts(IList<Account> accounts, string fileName) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            await Task.Factory.StartNew(() => {
                using var package = new ExcelPackage();
                // Add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add("Accounts");
                // Add the headers
                worksheet.Cells[1, 1].Value = "ID";
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
                ExcelWorksheet worksheet = package.Workbook.Worksheets["Accounts"];
                if (worksheet == null) return applicationData;

                var isRowValid = true;
                for (var row = 2; isRowValid; row++) {
                    if (worksheet.Cells[row, 1].Value != null) { // If current row, does not have an ID (col = 1), then not a valid row.
                        applicationData.Accounts.Add(new Account() {
                            Id = worksheet.Cells[row, 1].GetValue<int>(),
                            Name = worksheet.Cells[row, 2].GetValue<string>()
                        });
                    } else {
                        isRowValid = false;
                    }
                }

                return applicationData;
            });
        }

        public Task<IList<Account>> LoadAccounts(string path) {
            throw new NotImplementedException();
        }

        public Task<IList<Account>> LoadAccounts() {
            throw new NotImplementedException();
        }

    }
}