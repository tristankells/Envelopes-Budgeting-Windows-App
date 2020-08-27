using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Helpers;
using Envelopes.Models;

namespace Envelopes.Pages.BudgetPage.CategoriesGrid {
    public interface ICategoriesGridPresenter {
        public IView GetView();
    }

    public class CategoriesGridPresenter : Presenter, ICategoriesGridPresenter {
        #region Constructors

        public CategoriesGridPresenter(ICategoriesGridView view,
            ICategoriesGridViewModel viewModel,
            IDataService dataService,
            IGridValidator gridValidator) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.dataService = dataService;
            this.gridValidator = gridValidator;

            BindEvents();
            BindCommands();
        }

        #endregion

        #region Fields

        private readonly ICategoriesGridView view;
        private readonly ICategoriesGridViewModel viewModel;
        private readonly IDataService dataService;
        private readonly IGridValidator gridValidator;

        #endregion

        #region Events

        private void BindEvents() {
            view.Loaded += View_Loaded;
            view.DataGridCellEditEnding += CategoriesDataGrid_CellEditEnding;
            view.Unloaded += View_Unloaded;
        }

        private void CategoriesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            switch ((e.Column as DataGridTextColumn)?.Header) {
                case nameof(Category.Name):
                    OnNameCellEditEnding(e);
                    break;

                case nameof(Category.Available):
                    OnAvailableCellEditEnding(((TextBox) e.EditingElement).Text);
                    break;

                case nameof(Category.Budgeted):
                    OnBudgetedCellEditEnding(((TextBox) e.EditingElement).Text);
                    break;
            }
        }

        private void OnNameCellEditEnding(DataGridCellEditEndingEventArgs e) {
            gridValidator.ValidateNewTextBoxValueIsUniqueInColumn((TextBox) e.EditingElement,
                viewModel.ItemList.Select(account => account.Name).ToList(), (e.Row.Item as Account)?.Name);
        }

        private void OnBudgetedCellEditEnding(string text) {
            if (gridValidator.ParseAmountFromString(text, out decimal newBudgetedAmount)) {
                viewModel.SelectedItem.Budgeted = newBudgetedAmount;
            } else {
                viewModel.SelectedItem.Budgeted = 0
                    ;
            }
        }

        private void OnAvailableCellEditEnding(string text) {
            if (!gridValidator.ParseAmountFromString(text, out decimal newBudgetedAmount)) {
                return;
            }

            Category selectedCategory = viewModel.SelectedItem;

            if (selectedCategory.Available == newBudgetedAmount) {
                return;
            }

            decimal difference = selectedCategory.Available - newBudgetedAmount;
            selectedCategory.Budgeted -= difference;
            viewModel.SelectedItem.Available = newBudgetedAmount;
        }

        private void View_Unloaded(object sender, RoutedEventArgs e) {
            viewModel.ItemList.Clear();
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            PopulateCategoriesList();
        }

        #endregion

        #region Methods

        public IView GetView() => view;

        private void BindCommands() {
            viewModel.AddItemCommand = new DelegateCommand(ExecuteAddCategory, CanExecuteAddCategory);
            viewModel.DeleteItemCommand = new DelegateCommand(ExecuteDeleteCategory, CanExecuteDeleteCategory);
        }

        private static bool CanExecuteDeleteCategory() => true;

        private void ExecuteDeleteCategory() {
            //Confirm the user would like to delete the category
            MessageBoxResult result =
                MessageBox.Show(
                    "Are you sure you would like to delete your category? This will reset the category of all the account transactions.",
                    "Delete Account", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            switch (result) {
                case MessageBoxResult.No:
                case MessageBoxResult.Cancel:
                    // Don't delete category
                    break;
                case MessageBoxResult.Yes: // Attempt to delete category
                    DeleteCategory();
                    break;
            }
        }

        private void DeleteCategory() {
            Category selectedCategory = viewModel.SelectedItem;

            if (dataService.RemoveCategory(selectedCategory)) {
                viewModel.RemoveItem(selectedCategory);
            }
        }

        private static bool CanExecuteAddCategory() => true;

        private void ExecuteAddCategory() {
            Category newCategory = dataService.AddCategory();
            viewModel.AddItem(newCategory);
        }

        private void PopulateCategoriesList() {
            IEnumerable<Category> categories = dataService.Categories();
            foreach (Category category in categories) {
                viewModel.AddItem(category);
            }
        }

        #endregion
    }
}