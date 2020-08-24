using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.Helpers;
using Envelopes.Models;

namespace Envelopes.Pages.BudgetPage.CategoriesGrid {

    public interface ICategoriesGridPresenter {
        public CategoriesGridView GetView();
    }

    public class CategoriesGridPresenter : Presenter, ICategoriesGridPresenter {
        #region Fields

        private readonly CategoriesGridView view;
        private readonly ICategoriesGridViewModelBase viewModel;
        private readonly IDataService dataService;
        private readonly IGridValidator gridValidator;

        #endregion

        #region Constructors

        public CategoriesGridPresenter(CategoriesGridView view,
            ICategoriesGridViewModelBase viewModel,
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

        #region Events

        private void BindEvents() {
            view.Loaded += View_Loaded;
            view.CategoriesDataGrid.CellEditEnding += CategoriesDataGrid_CellEditEnding;
            view.Unloaded += View_Unloaded;
        }

        private void CategoriesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            switch ((e.Column as DataGridTextColumn)?.SortMemberPath) {
                case nameof(Category.Name):
                    gridValidator.ValidateNewTextBoxValueIsUniqueInColumn((TextBox) e.EditingElement,
                        viewModel.ItemList.Select(account => account.Name).ToList(), (e.Row.Item as Account)?.Name);
                    break;
                case nameof(Category.Available):
                    var selectCategory = viewModel.SelectedItem;
                    var textBoxText = ((TextBox) e.EditingElement).Text;

                    decimal.TryParse(textBoxText, out decimal newValue);
                   

                    if (selectCategory.Available != newValue) {
                        var difference = selectCategory.Available - newValue;
                        selectCategory.Budgeted -= difference;
                    }

                    break;
                case nameof(Category.Budgeted):
                    gridValidator.ValidateInFieldCalculations((TextBox)e.EditingElement);
                    break;
            }
        }

        private void View_Unloaded(object sender, RoutedEventArgs e) {
            viewModel.ItemList.Clear();
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            PopulateCategoriesList();
        }

        #endregion

        #region Methods

        public CategoriesGridView GetView() => view;

        private void BindCommands() {
            viewModel.AddItemCommand = new DelegateCommand(ExecuteAddCategory, CanExecuteAddCategory);
            viewModel.DeleteItemCommand = new DelegateCommand(ExecuteDeleteCategory, CanExecuteDeleteCategory);
        }

        private bool CanExecuteDeleteCategory() => true;

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
            var selectedCategory = viewModel.SelectedItem;

            if (dataService.RemoveCategory(selectedCategory)) {
                viewModel.RemoveItem(selectedCategory);
            }
        }

        private bool CanExecuteAddCategory() => true;

        private void ExecuteAddCategory() {
            var newCategory = dataService.AddCategory();
            viewModel.AddItem(newCategory);
        }

        private void PopulateCategoriesList() {
            var categories = dataService.Categories();
            foreach (var category in categories) {
                viewModel.AddItem(category);
            }
        }

        #endregion
    }
}