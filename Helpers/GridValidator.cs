using System.Collections.Generic;
using System.Windows.Controls;

namespace Envelopes.Pages.BudgetPage.CategoriesGrid {
    public interface IGridValidator {
        public void ValidateNewTextBoxValueIsUniqueInColumn(TextBox updatedTextBox, IList<string> existingValues,
            string originalValue);
    }
    public class GridValidator : IGridValidator {
        public void ValidateNewTextBoxValueIsUniqueInColumn(TextBox updatedTextBox, IList<string> existingValues,
            string originalValue) {
            var newAccountName = updatedTextBox.Text;
            if (!IsPropertyUnique(newAccountName, existingValues)) {
                updatedTextBox.Text = originalValue ?? string.Empty;
            }
        }

        private static bool IsPropertyUnique(string newValue, IList<string> existingValue) {
            return !existingValue.Contains(newValue);
        }
    }
}