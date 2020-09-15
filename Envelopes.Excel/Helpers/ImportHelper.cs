using Envelopes.Persistence.Importer;

namespace Envelopes.Persistence.Helpers {
    public static class ImportHelper {
            public static AccountTransactionColumnMap KiwiBankMap { get; } = new AccountTransactionColumnMap {
                DateColumnIndex = 0,
                PayeeColumnIndex = 1,
                AmountColumnIndex = 3,
                IsMinusOutflow = true
            };

            public static string KiwiBankLocation { get; } = "C:\\Users\\trist\\Documents\\EnvelopesImports\\Kiwibank.CSV";

            public static AccountTransactionColumnMap AmexBankMap { get; } = new AccountTransactionColumnMap {
                DateColumnIndex = 0,
                PayeeColumnIndex = 3,
                AmountColumnIndex = 2,
                IncludeFirstRow = true
            };

            public static string AmexBankLocation { get; } = "C:\\Users\\trist\\Documents\\EnvelopesImports\\Amex.csv";

            public static AccountTransactionColumnMap PurpleVisaBankMap { get; } = new AccountTransactionColumnMap {
                DateColumnIndex = 0,
                PayeeColumnIndex = 2,
                AmountColumnIndex = 4
            };

            public static string PurpleVisaBankLocation { get; } = "C:\\Users\\trist\\Documents\\EnvelopesImports\\PurpleVisa.csv";
        }
}