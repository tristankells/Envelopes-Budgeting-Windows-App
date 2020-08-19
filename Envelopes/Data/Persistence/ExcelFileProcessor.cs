using System;
using System.IO;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Envelopes.Data.Persistence {
    public interface IExcelFileProcessor {
        public Task SaveAs(ExcelPackage package);
        public ExcelPackage LoadExcelPackageFromFile();
    }

    public class ExcelFileProcessor : IExcelFileProcessor {
        private const string FileName = "Envelopes_Dev.xlsx";
        private readonly string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public async Task SaveAs(ExcelPackage package) {
            var filePath = new FileInfo(Path.Combine(directoryPath, FileName));
            try {
                await package.SaveAsAsync(filePath);
            }
            catch (IOException e) {
                Console.WriteLine(e);
                throw;
            }
        }

        public ExcelPackage LoadExcelPackageFromFile() {
            var filePath = new FileInfo(Path.Combine(directoryPath, FileName));
            return new ExcelPackage(filePath);
        }
    }
}