using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Envelopes.Helpers {
    public interface IGridValidator {
        public string ValidateNewStringIsUniqueFromExistingStrings(string newText, IList<string> existingValues,
            string originalValue);

        public bool ParseAmountFromString(string amountAsString, out decimal amountAsDecimal);
    }

    public class GridValidator : IGridValidator {
        public string ValidateNewStringIsUniqueFromExistingStrings(string newText, IList<string> existingValues, string originalValue) {
            return existingValues.Contains(newText) ? originalValue ?? string.Empty : newText;
        }

        public bool ParseAmountFromString(string amountAsString, out decimal amountAsDecimal) {
            amountAsDecimal = 0;

            if (string.IsNullOrWhiteSpace(amountAsString)) {
                return true; // Empty string should be parsed as $0.00
            }

            string amountAsStringCleanedUp = amountAsString.Replace('(', '-')
                .Replace(")", "")
                .Replace("$", "");

            if (amountAsStringCleanedUp.Contains('-') && (!amountAsStringCleanedUp.StartsWith('-') || amountAsStringCleanedUp.Count(c => c == '-') >= 2)) {
                string[] fields = amountAsStringCleanedUp.Split('-');
                if (fields.Length > 2) {
                    fields = new[] {
                        "-" + fields[1],
                        fields[2]
                    };
                }

                for (var i = 0; i < fields.Length; i++) {
                    if (!decimal.TryParse(fields[i].Trim(), out decimal decimalResult)) {
                        return false;
                    }

                    if (i == 0) {
                        amountAsDecimal = decimalResult;
                    } else {
                        amountAsDecimal -= decimalResult;
                    }
                }

                return true;
            }

            if (amountAsStringCleanedUp.Contains('+') && !amountAsStringCleanedUp.StartsWith('+')) {
                string[] fields = amountAsStringCleanedUp.Split('+');
                foreach (string field in fields) {
                    if (!decimal.TryParse(field.Trim(), out decimal decimalResult)) {
                        return false;
                    }

                    amountAsDecimal += decimalResult;
                }

                return true;
            }


            return decimal.TryParse(amountAsStringCleanedUp.Trim(), out amountAsDecimal);
        }
    }
}