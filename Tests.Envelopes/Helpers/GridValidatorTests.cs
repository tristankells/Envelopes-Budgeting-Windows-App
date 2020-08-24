using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Envelopes.Data;
using Envelopes.Data.Persistence;
using Envelopes.Helpers;
using NUnit.Framework;

namespace Tests.Envelopes.Helpers {
    [Apartment(ApartmentState.STA)]
    class GridValidatorTests {
        private GridValidator gridValidator;

        [SetUp]
        public void Setup() {
            gridValidator = new GridValidator();
        }

        [Test]
        public void ValidateAmountField_RegularNumberIsNotChanged() {
            var textBox = new TextBox {
                Text = "1"
            };

            gridValidator.ValidateInFieldCalculations(textBox);
            Assert.AreEqual("1", textBox.Text);
        }

        [Test]
        public void ValidateAmountField_SubtractionWorks() {
            var textBox = new TextBox {
                Text = "$10.00 - $4.00"
            };
            gridValidator.ValidateInFieldCalculations(textBox);

            Assert.AreEqual("6.00", textBox.Text);

            textBox.Text = "9 - 7";
            gridValidator.ValidateInFieldCalculations(textBox);

            Assert.AreEqual("2", textBox.Text);

            textBox.Text = "$1000 - 8.85";
            gridValidator.ValidateInFieldCalculations(textBox);

            Assert.AreEqual("991.15", textBox.Text);

            textBox.Text = "100 - $9.12";
            gridValidator.ValidateInFieldCalculations(textBox);

            Assert.AreEqual("90.88", textBox.Text);
        }

        [Test]
        public void ValidateAmountField_AdditionWorks() {
            var textBox = new TextBox {
                Text = "$1.00 + $1.00"
            };

            gridValidator.ValidateInFieldCalculations(textBox);

            Assert.AreEqual("2.00", textBox.Text);

            textBox.Text = "9 + 7";
            gridValidator.ValidateInFieldCalculations(textBox);

            Assert.AreEqual("16", textBox.Text);

            textBox.Text = "$1000 + 8.85";
            gridValidator.ValidateInFieldCalculations(textBox);

            Assert.AreEqual("1008.85", textBox.Text);

            textBox.Text = "100 + $9.12";
            gridValidator.ValidateInFieldCalculations(textBox);

            Assert.AreEqual("109.12", textBox.Text);
        }
    }
}