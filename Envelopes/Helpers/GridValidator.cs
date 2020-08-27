using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace Envelopes.Helpers {
    public interface IGridValidator {
        public void ValidateNewTextBoxValueIsUniqueInColumn(TextBox updatedTextBox, IList<string> existingValues,
            string originalValue);

        public bool ParseAmountFromString(string amountAsString, out decimal amountAsDecimal);
    }

    public class GridValidator : IGridValidator {
        public void ValidateNewTextBoxValueIsUniqueInColumn(TextBox updatedTextBox, IList<string> existingValues,
            string originalValue) {
            string newAccountName = updatedTextBox.Text;
            if (!IsPropertyUnique(newAccountName, existingValues)) {
                updatedTextBox.Text = originalValue ?? string.Empty;
            }
        }

        private static bool IsPropertyUnique(string newValue, ICollection<string> existingValue) => !existingValue.Contains(newValue);

        public bool ParseAmountFromString(string amountAsString, out decimal amountAsDecimal) {
            amountAsDecimal = 0;

            var amountAsStringCleanedUp = amountAsString.Replace('(', '-')
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