using Envelopes.Models;
using Envelopes.Models.Models;
using NUnit.Framework;

namespace Tests.Envelopes {
    public static class TestValidationHelper {
        public static void ValidateAccountsAreEqual(Account expected, Account actual) {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
        }

        public static void ValidateCategoriesAreEqual(Category expected, Category actual) {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Budgeted, actual.Budgeted);
        }

        public static void ValidateAccountTransactionsAreEqual(AccountTransaction expected, AccountTransaction actual) {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.AccountId, actual.AccountId);
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Payee, actual.Payee);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.CategoryId, actual.CategoryId);
            Assert.AreEqual(expected.Memo, actual.Memo);
            Assert.AreEqual(expected.Outflow, actual.Outflow);
            Assert.AreEqual(expected.Inflow, actual.Inflow);
        }
    }
}