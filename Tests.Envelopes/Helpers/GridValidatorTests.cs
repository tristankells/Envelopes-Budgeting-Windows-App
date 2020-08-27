using Envelopes.Helpers;
using NUnit.Framework;
using System.Threading;

namespace Tests.Envelopes.Helpers {
    [Apartment(ApartmentState.STA)]
    class GridValidatorTests {
        private GridValidator gridValidator;

        [SetUp]
        public void Setup() {
            gridValidator = new GridValidator();
        }

        [Test]
        public void ParseAmountFromString_RegularNumberIsNotChanged() {
            bool ableToParseAmount = gridValidator.ParseAmountFromString("1", out decimal parsedDecimalAmount);

            Assert.IsTrue(ableToParseAmount);
            Assert.AreEqual(1M, parsedDecimalAmount);
        }

        [Test]
        public void ParseAmountFromString_SubtractionWorks() {
            gridValidator.ParseAmountFromString("$10.00 - $4.00", out decimal parsedDecimalAmount);
            Assert.AreEqual(6M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("$1000 - 8.85", out parsedDecimalAmount);
            Assert.AreEqual(991.15M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("100 - $9.12", out parsedDecimalAmount);
            Assert.AreEqual(90.88M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("7 - 9", out parsedDecimalAmount);
            Assert.AreEqual(-2M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("-9", out parsedDecimalAmount);
            Assert.AreEqual(-9M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("($9.00) - 9", out parsedDecimalAmount);
            Assert.AreEqual(-18M, parsedDecimalAmount);
        }

        [Test]
        public void ParseAmountFromString_AdditionWorks() {
            gridValidator.ParseAmountFromString("$1.00 + $1.00", out decimal parsedDecimalAmount);
            Assert.AreEqual(2M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("9 + 7", out parsedDecimalAmount);
            Assert.AreEqual(16M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("$1000 + 8.85", out parsedDecimalAmount);
            Assert.AreEqual(1008.85M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("100 + $9.12", out parsedDecimalAmount);
            Assert.AreEqual(109.12M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("+9", out parsedDecimalAmount);
            Assert.AreEqual(9M, parsedDecimalAmount);

            gridValidator.ParseAmountFromString("($9.00) + 9", out parsedDecimalAmount);
            Assert.AreEqual(0M, parsedDecimalAmount);
        }
    }
}