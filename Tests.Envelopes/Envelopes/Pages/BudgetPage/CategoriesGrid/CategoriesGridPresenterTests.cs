using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;
using Envelopes.Data;
using Envelopes.Models;
using Envelopes.Pages.BudgetPage.CategoriesGrid;
using Moq;
using NUnit.Framework;

namespace Tests.Envelopes.Envelopes.Pages.BudgetPage.CategoriesGrid {
    [Apartment(ApartmentState.STA)]
    internal class CategoriesGridPresenterTests {
        private CategoriesGridPresenter categoriesGridPresenter;
        private Mock<ICategoriesGridView> categoriesGridViewMock;
        private Mock<ICategoriesGridViewModel> categoriesGridViewModelMock;
        private Mock<IDataService> dataServiceMock;
        private Category selectedCategory;


        [SetUp]
        public void Setup() {
            categoriesGridViewModelMock = new Mock<ICategoriesGridViewModel>();
            dataServiceMock = new Mock<IDataService>();
            categoriesGridViewMock = new Mock<ICategoriesGridView>();
            selectedCategory = new Category();

            categoriesGridViewMock.Setup(cgvm => cgvm.CategoriesDataGrid).Returns(new DataGrid());
            categoriesGridViewModelMock.Setup(cgvmm => cgvmm.SelectedItem).Returns(selectedCategory);
            categoriesGridViewModelMock.Setup(cgvmm => cgvmm.ItemList).Returns(new ObservableCollection<Category>());

            categoriesGridPresenter = new CategoriesGridPresenter(categoriesGridViewMock.Object, categoriesGridViewModelMock.Object, dataServiceMock.Object);
        }

        [Test]
        public void OnNameCellEditEnding_CorrectlyUpdatesCategoryName() {
            // Arrange
            const string initialName = "Old Name";
            const string newName = "New Name";

            selectedCategory.Name = initialName;

            // Act
            RaiseCellEditEndingEvent(newName, nameof(Category.Name));

            // Assert
            Assert.AreEqual(newName, selectedCategory.Name);
        }

        [Test]
        public void OnAvailableCellEditEnding_CorrectlyUpdatesCategoryAvailable_AndDecreasesBudgeted() {
            // Arrange
            const decimal newAvailable = 40M;
            const string newAvailableAsString = "$40";

            const decimal activity = -50M;
            const decimal initialBudgeted = 100;
            const decimal expectedBudgeted = 90;

            selectedCategory.Activity = activity;
            selectedCategory.Budgeted = initialBudgeted;

            Assert.AreEqual(50M, selectedCategory.Available);

            // Act
            RaiseCellEditEndingEvent(newAvailableAsString, nameof(Category.Available));

            // Assert
            Assert.AreEqual(newAvailable, selectedCategory.Available);
            Assert.AreEqual(expectedBudgeted, selectedCategory.Budgeted);
        }

        [Test]
        public void OnAvailableCellEditEnding_CorrectlyUpdatesCategoryAvailable_AndIncreasesBudgeted() {
            // Arrange
            const decimal newAvailable = 200.89M;
            const string newAvailableAsString = "200.89";

            const decimal activity = -50M;
            const decimal initialBudgeted = 150;
            const decimal expectedBudgeted = 250.89M;

            selectedCategory.Activity = activity;
            selectedCategory.Budgeted = initialBudgeted;

            Assert.AreEqual(100M, selectedCategory.Available);

            // Act
            RaiseCellEditEndingEvent(newAvailableAsString, nameof(Category.Available));

            // Assert
            Assert.AreEqual(newAvailable, selectedCategory.Available);
            Assert.AreEqual(expectedBudgeted, selectedCategory.Budgeted);
        }

        [Test]
        public void OnAvailableCellEditEnding_CorrectlyUpdatesCategoryAvailable_AndDoesNotChangeBudgeted() {
            // Arrange
            const decimal newAvailable = 100M;
            const string newAvailableAsString = "$100.00";

            const decimal activity = -40M;
            const decimal initialBudgeted = 140;

            selectedCategory.Activity = activity;
            selectedCategory.Budgeted = initialBudgeted;

            Assert.AreEqual(100M, selectedCategory.Available);

            // Act
            RaiseCellEditEndingEvent(newAvailableAsString, nameof(Category.Available));

            // Assert
            Assert.AreEqual(newAvailable, selectedCategory.Available);
            Assert.AreEqual(initialBudgeted, selectedCategory.Budgeted);
        }

        [Test]
        public void OnBudgetedCellEditEnding_CorrectlyBudgetedCategoryName() {
            // Arrange
            const decimal initialBudgeted = 10;
            const string newBudgetedAsString = "20.00";
            const decimal newBudgeted = 20M;

            selectedCategory.Budgeted = initialBudgeted;

            // Act
            RaiseCellEditEndingEvent(newBudgetedAsString, nameof(Category.Budgeted));

            // Assert
            Assert.AreEqual(newBudgeted, selectedCategory.Budgeted);
        }

        private void RaiseCellEditEndingEvent(string newText, string propertyName) {
            var dataGridColumn = new DataGridTextColumn {
                SortMemberPath = propertyName
            };
            var textBox = new TextBox {
                Text = newText
            };

            categoriesGridViewMock.Setup(mock => mock.CategoriesDataGrid).Returns(new DataGrid());
            categoriesGridViewMock.Raise(cgvm => cgvm.DataGridCellEditEnding += null, new DataGridCellEditEndingEventArgs(dataGridColumn, new DataGridRow(), textBox, DataGridEditAction.Commit));
        }
    }
}