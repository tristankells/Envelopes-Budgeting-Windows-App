using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Envelopes.Data;
using Envelopes.Helpers;
using Envelopes.Models;
using Envelopes.Pages.BudgetPage.CategoriesGrid;
using Moq;
using NUnit.Framework;

namespace Tests.Envelopes.Pages.BudgetPage.CategoriesGrid {
    [Apartment(ApartmentState.STA)]
    internal class CategoriesGridPresenterTests {
        private CategoriesGridPresenter categoriesGridPresenter;
        private Mock<ICategoriesGridView> categoriesGridViewMock;
        private Mock<ICategoriesGridViewModel> categoriesGridViewModelMock;
        private Mock<IDataService> dataServiceMock;
        private Mock<IGridValidator> gridValidatorMock;
        private Category SelectedCategory;


        [SetUp]
        public void Setup() {
            categoriesGridViewModelMock = new Mock<ICategoriesGridViewModel>();
            dataServiceMock = new Mock<IDataService>();
            gridValidatorMock = new Mock<IGridValidator>();
            categoriesGridViewMock = new Mock<ICategoriesGridView>();
            SelectedCategory = new Category();

            categoriesGridViewMock.Setup(cgvm => cgvm.CategoriesDataGrid).Returns(new DataGrid());
            categoriesGridViewModelMock.Setup(cgvmm => cgvmm.SelectedItem).Returns(SelectedCategory);
            categoriesGridViewModelMock.Setup(cgvmm => cgvmm.ItemList).Returns(new ObservableCollection<Category>());

            categoriesGridPresenter = new CategoriesGridPresenter(categoriesGridViewMock.Object, categoriesGridViewModelMock.Object, dataServiceMock.Object, gridValidatorMock.Object);
        }

        [Test]
        public void OnNameCellEditEnding_CorrectlyUpdatesCategoryName() {
            // Setup
            const string initialName = "Old Name";
            const string newName = "New Name";

            SelectedCategory.Name = initialName;

            gridValidatorMock.Setup(gvm => gvm.ValidateNewStringIsUniqueFromExistingStrings(newName, It.IsAny<List<string>>(), SelectedCategory.Name)).Returns(newName);

            // Act
            RaiseCellEditEndingEvent(newName, nameof(Category.Name));

            // Assert
            Assert.AreEqual(newName, SelectedCategory.Name);
        }

        [Test]
        public void OnAvailableCellEditEnding_CorrectlyUpdatesCategoryAvailable_AndDecreasesBudgeted() {
            // Setup
            decimal newAvailable = 40M;
            const string newAvailableAsString = "$40";

            const decimal activity = -50M;
            const decimal initialBudgeted = 100;
            const decimal expectedBudgeted = 90; 

            SelectedCategory.Activity = activity;
            SelectedCategory.Budgeted = initialBudgeted;

            Assert.AreEqual(50M, SelectedCategory.Available);

            gridValidatorMock.Setup(gvm => gvm.ParseAmountFromString(newAvailableAsString, out newAvailable)).Returns(true);

            // Act
            RaiseCellEditEndingEvent(newAvailableAsString, nameof(Category.Available));

            // Assert
            Assert.AreEqual(newAvailable, SelectedCategory.Available);
            Assert.AreEqual(expectedBudgeted, SelectedCategory.Budgeted);
        }

        [Test]
        public void OnAvailableCellEditEnding_CorrectlyUpdatesCategoryAvailable_AndIncreasesBudgeted() {
            // Setup
            decimal newAvailable = 200.89M;
            const string newAvailableAsString = "200.89";

            const decimal activity = -50M;
            const decimal initialBudgeted = 150;
            const decimal expectedBudgeted = 250.89M;

            SelectedCategory.Activity = activity;
            SelectedCategory.Budgeted = initialBudgeted;

            Assert.AreEqual(100M, SelectedCategory.Available);

            gridValidatorMock.Setup(gvm => gvm.ParseAmountFromString(newAvailableAsString, out newAvailable)).Returns(true);

            // Act
            RaiseCellEditEndingEvent(newAvailableAsString, nameof(Category.Available));

            // Assert
            Assert.AreEqual(newAvailable, SelectedCategory.Available);
            Assert.AreEqual(expectedBudgeted, SelectedCategory.Budgeted);
        }

        [Test]
        public void OnAvailableCellEditEnding_CorrectlyUpdatesCategoryAvailable_AndDoesNotChangeBudgeted() {
            // Setup
            decimal newAvailable = 100M;
            const string newAvailableAsString = "$100.00";

            const decimal activity = -40M;
            const decimal initialBudgeted = 140;

            SelectedCategory.Activity = activity;
            SelectedCategory.Budgeted = initialBudgeted;

            Assert.AreEqual(100M, SelectedCategory.Available);

            gridValidatorMock.Setup(gvm => gvm.ParseAmountFromString(newAvailableAsString, out newAvailable)).Returns(true);

            // Act
            RaiseCellEditEndingEvent(newAvailableAsString, nameof(Category.Available));

            // Assert
            Assert.AreEqual(newAvailable, SelectedCategory.Available);
            Assert.AreEqual(initialBudgeted, SelectedCategory.Budgeted);
        }

        [Test]
        public void OnBudgetedCellEditEnding_CorrectlyBudgetedCategoryName() {
            // Setup
            const decimal initialBudgeted = 10;
            const string newBudgetedAsString = "20.00";
            decimal newBudgeted = 20M;

            SelectedCategory.Budgeted = initialBudgeted;

            gridValidatorMock.Setup(gvm => gvm.ParseAmountFromString(newBudgetedAsString, out newBudgeted)).Returns(true);

            // Act
            RaiseCellEditEndingEvent(newBudgetedAsString, nameof(Category.Budgeted));

            // Assert
            Assert.AreEqual(newBudgeted, SelectedCategory.Budgeted);
        }

        private void RaiseCellEditEndingEvent(string newText, string propertyName) {
            var dataGridColumn = new DataGridTextColumn {
                SortMemberPath = propertyName
            };
            var textBox = new TextBox() {
                Text = newText
            };

            categoriesGridViewMock.Setup(mock => mock.CategoriesDataGrid).Returns(new DataGrid());
            categoriesGridViewMock.Raise(cgvm => cgvm.DataGridCellEditEnding += null, new DataGridCellEditEndingEventArgs(dataGridColumn, new DataGridRow(), textBox, DataGridEditAction.Commit));
        }
    }
}