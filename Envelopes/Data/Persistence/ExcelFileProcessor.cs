using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Envelopes.Data.Persistence {
    public interface IExcelFileProcessor {
        public Task SaveAs(ExcelPackage package);
        public ExcelPackage LoadExcelPackageFromFile();
    }

    public class ExcelFileProcessor : IExcelFileProcessor {
        private readonly string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private readonly string fileName = ConfigurationManager.AppSettings.Get("BudgetPath");

        public async Task SaveAs(ExcelPackage package) {
            var filePath = new FileInfo(Path.Combine(directoryPath, fileName));
            try {
                await package.SaveAsAsync(filePath);
            }
            catch (IOException e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public ExcelPackage LoadExcelPackageFromFile() {
            var filePath = new FileInfo(Path.Combine(directoryPath, fileName));
            return new ExcelPackage(filePath);
        }
    }
}