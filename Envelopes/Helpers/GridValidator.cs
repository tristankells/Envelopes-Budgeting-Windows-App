using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace Envelopes.Helpers {
    public interface IGridValidator {
        public void ValidateNewTextBoxValueIsUniqueInColumn(TextBox updatedTextBox, IList<string> existingValues,
            string originalValue);

        public void ValidateInFieldCalculations(TextBox updatedTextBox);
    }

    public class GridValidator : IGridValidator {
        public void ValidateNewTextBoxValueIsUniqueInColumn(TextBox updatedTextBox, IList<string> existingValues,
            string originalValue) {
            var newAccountName = updatedTextBox.Text;
            if (!IsPropertyUnique(newAccountName, existingValues)) {
                updatedTextBox.Text = originalValue ?? string.Empty;
            }
        }

        public void ValidateInFieldCalculations(TextBox updatedTextBox) {
            string newValue = updatedTextBox.Text;
            
            if (newValue.Contains('+')) {
                decimal newTextBoxValue = 0;

                string[] fields = newValue.Split('+');
                foreach (string field in fields) {
                   decimal.TryParse(field.Trim().Trim('$'), out decimal decimalResult);
                   newTextBoxValue += decimalResult;
                }

                updatedTextBox.Text = newTextBoxValue.ToString(CultureInfo.CurrentCulture);
            }  
            
            if (newValue.Contains('-')) {
                decimal newTextBoxValue = 0;

                string[] fields = newValue.Split('-');
                for (var i = 0; i < fields.Length; i++) {
                    decimal.TryParse(fields[i].Trim().Trim('$'), out decimal decimalResult);
                    if (i == 0) {
                        newTextBoxValue = decimalResult;
                    } else {
                        newTextBoxValue -= decimalResult;
                    }
                }
                updatedTextBox.Text = newTextBoxValue.ToString(CultureInfo.CurrentCulture);
            }
        }

        private static bool IsPropertyUnique(string newValue, IList<string> existingValue) {
            return !existingValue.Contains(newValue);
        }
    }
}