using Envelopes.Models;

namespace Envelopes.Persistence.Importer {
    /// <summary>
    ///     Stores the zero based index of column-to-field mappings for <see cref="AccountTransaction" />
    /// </summary>
    /// <example>
    ///     Example: First CSV column is Date? DateColumnIndex = 0
    /// </example>
    public class AccountTransactionColumnMap {
        public int DateColumnIndex { get; set; }
        public int PayeeColumnIndex { get; set; }
        public int AmountColumnIndex { get; set; }
        public bool IsMinusOutflow { get; set; }
        public bool IncludeFirstRow { get; set; }
    }
}