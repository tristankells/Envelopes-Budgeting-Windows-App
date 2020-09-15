using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Envelopes.Models.Models;
using OfficeOpenXml;

namespace Envelopes.Persistence.Importer {
    public class TransactionsImporter : ITransactionsImporter {
        public async Task<IEnumerable<AccountTransaction>> Import(string fileLocation, AccountTransactionColumnMap map) {
            var file = new FileInfo(fileLocation);
            var transactions = new List<AccountTransaction>();

            if (file.Extension.Equals(".csv", StringComparison.InvariantCultureIgnoreCase)) {
                transactions.AddRange(await ImportTransactionsFromCsv(fileLocation, map));
            }

            if (file.Extension.Equals(".xlsx", StringComparison.InvariantCultureIgnoreCase)) {
                transactions.AddRange(await ImportTransactionsFromExcel(file, map));
            }

            return transactions;
        }

        private async Task<List<AccountTransaction>> ImportTransactionsFromCsv(string file, AccountTransactionColumnMap map) {
            return await Task.Factory.StartNew(() => {
                using var reader = new StreamReader(file);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var transactions = new List<AccountTransaction>();

                if (!map.IncludeFirstRow) {
                    csv.Read();
                    csv.ReadHeader();
                }

                while (csv.Read()) {
                    var transaction = new AccountTransaction() {
                        Payee = csv.GetField<string>(map.PayeeColumnIndex),
                    };

                    string dateAsString = csv.GetField<string>(map.DateColumnIndex);
                    SetTransactionDate(transaction, dateAsString);

                    string amountAsString = csv.GetField<string>(map.AmountColumnIndex);
                    SetTransactionAmount(transaction, amountAsString, map.IsMinusOutflow);

                    transactions.Add(transaction);
                }

                return transactions;
            });
        }

        private static async Task<List<AccountTransaction>> ImportTransactionsFromExcel(FileInfo file, AccountTransactionColumnMap map) {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            return await Task.Factory.StartNew(() => {
                var transactions = new List<AccountTransaction>();

                using var package = new ExcelPackage(file);

                ExcelWorksheet worksheet = package.Workbook.Worksheets.First(); //Requirement, the transactions are on the first worksheet of the excel
                if (worksheet != null) {
                    transactions.AddRange(ParseTransactionFromExcelWorkSheet(worksheet.Cells, map));
                }

                return transactions;
            });
        }

        private static IEnumerable<AccountTransaction> ParseTransactionFromExcelWorkSheet(ExcelRange cells, AccountTransactionColumnMap map) {
            var accountTransactions = new List<AccountTransaction>();
            bool isTransactionRowValid = true;
            for (int row = 2; isTransactionRowValid; row++) {
                if (cells[row, map.DateColumnIndex+1].Value != null) { // If row does not have a date, it is invalid. This will likely indicate the end transactions.
                    accountTransactions.Add(ParseTransactionFromExcelWorksheetRow(cells, row, map));
                } else {
                    isTransactionRowValid = false;
                }
            }

            return accountTransactions;
        }

        private static AccountTransaction ParseTransactionFromExcelWorksheetRow(ExcelRange cells, int row, AccountTransactionColumnMap map) {
            var transaction = new AccountTransaction {
                Date = cells[row, map.DateColumnIndex + 1].GetValue<DateTime>(),
                Payee = cells[row, map.PayeeColumnIndex + 1].GetValue<string>()
            };

            string amountAsString = cells[row, map.AmountColumnIndex + 1].GetValue<string>();
            SetTransactionAmount(transaction, amountAsString, map.IsMinusOutflow);
            return transaction;
        }

        private static void SetTransactionAmount(AccountTransaction transaction, string amountAsString, bool isMinusOutflow) {
            decimal.TryParse(new string(amountAsString.Where(c => char.IsDigit(c) || char.IsPunctuation(c)).ToArray()), out decimal amount);

            if (isMinusOutflow) {
                amount = -amount;
            }

            if (amount >= 0) {
                transaction.Outflow = amount;
            } else {
                transaction.Inflow = -amount;
            }
        }

        private static void SetTransactionDate(AccountTransaction transaction, string dateAsString) {
            DateTime.TryParse(dateAsString, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out DateTime date);
            transaction.Date = date;
        }
    }
}