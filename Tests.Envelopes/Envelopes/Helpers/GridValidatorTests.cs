using System.Threading;
using Envelopes.Helpers;
using NUnit.Framework;

namespace Tests.Envelopes.Envelopes.Helpers {
    [Apartment(ApartmentState.STA)]
    internal class GridValidatorTests {
        [Test]
        public void ParseAmountFromString_RegularNumberIsNotChanged() {
            GridValidator.ParseAmountFromString("1", out decimal parsedDecimalAmount);
            Assert.AreEqual(1M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("-10", out parsedDecimalAmount);
            Assert.AreEqual(-10M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("100.98", out parsedDecimalAmount);
            Assert.AreEqual(100.98M, parsedDecimalAmount);
        }

        [Test]
        public void ParseAmountFromString_SubtractionWorks() {
            GridValidator.ParseAmountFromString("$10.00 - $4.00", out decimal parsedDecimalAmount);
            Assert.AreEqual(6M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("$1000 - 8.85", out parsedDecimalAmount);
            Assert.AreEqual(991.15M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("100 - $9.12", out parsedDecimalAmount);
            Assert.AreEqual(90.88M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("7 - 9", out parsedDecimalAmount);
            Assert.AreEqual(-2M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("-9", out parsedDecimalAmount);
            Assert.AreEqual(-9M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("($9.00) - 9", out parsedDecimalAmount);
            Assert.AreEqual(-18M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("($9.00) - 9 - 5", out parsedDecimalAmount);
            Assert.AreEqual(-23M, parsedDecimalAmount);
        }

        [Test]
        public void ParseAmountFromString_AdditionWorks() {
            GridValidator.ParseAmountFromString("$1.00 + $1.00", out decimal parsedDecimalAmount);
            Assert.AreEqual(2M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("9+7", out parsedDecimalAmount);
            Assert.AreEqual(16M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("$1000 + 8.85", out parsedDecimalAmount);
            Assert.AreEqual(1008.85M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("100 + $9.12", out parsedDecimalAmount);
            Assert.AreEqual(109.12M, parsedDecimalAmount);

            GridValidator.ParseAmountFromString("($9.00) + 9", out parsedDecimalAmount);
            Assert.AreEqual(0M, parsedDecimalAmount);
        }
    }
}