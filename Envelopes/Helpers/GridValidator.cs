using System;
using System.Collections.Generic;
using NCalc;

namespace Envelopes.Helpers {
    public static class GridValidator {
        public static string ValidateNewStringIsUniqueFromExistingStrings(string newText, IList<string> existingValues, string originalValue) => existingValues.Contains(newText) ? originalValue ?? string.Empty : newText;

        public static bool ParseAmountFromString(string amountAsString, out decimal amountAsDecimal) {
            amountAsDecimal = 0.00M;

            if (string.IsNullOrWhiteSpace(amountAsString)) {
                return true; // Empty string should be parsed as $0.00
            }

            string amountAsStringCleanedUp = amountAsString
                .Replace('(', '-')
                .Replace(")", "")
                .Replace(",", "")
                .Replace("$", ""); // Removes '$', replaces instance of '(' with '-', removes instances of ')' and ','

            try {
                var expr = new Expression(amountAsStringCleanedUp);
                Func<decimal> f = expr.ToLambda<decimal>();
                amountAsDecimal = f();
            }
            catch (Exception e) {
                Console.WriteLine(e);
                return false;
            }
           
            return true;
        }
    }
}