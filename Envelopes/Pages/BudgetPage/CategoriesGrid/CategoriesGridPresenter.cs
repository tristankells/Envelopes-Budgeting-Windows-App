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
            IDataService dataService) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.dataService = dataService;

            BindEvents();
            BindCommands();
        }

        #endregion

        #region Fields

        private readonly ICategoriesGridView view;
        private readonly ICategoriesGridViewModel viewModel;
        private readonly IDataService dataService;

        #endregion

        #region Events

        private void BindEvents() {
            view.Loaded += OnViewLoaded;
            view.DataGridCellEditEnding += OnCategoriesGridCellEditEnding;
            view.Unloaded += OnViewUnloaded;
        }

        private void OnCategoriesGridCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e) {
            switch ((e.Column as DataGridTextColumn)?.SortMemberPath) {
                case nameof(Category.Name):
                    OnNameCellEditEnding(((TextBox) e.EditingElement).Text);
                    break;

                case nameof(Category.Available):
                    OnAvailableCellEditEnding(((TextBox) e.EditingElement).Text);
                    break;

                case nameof(Category.Budgeted):
                    OnBudgetedCellEditEnding(((TextBox) e.EditingElement).Text);
                    break;
            }
        }

        private void OnNameCellEditEnding(string newText) {
            SelectedCategory.Name = GridValidator.ValidateNewStringIsUniqueFromExistingStrings(newText, viewModel.ItemList.Select(account => account.Name).ToList(), SelectedCategory.Name);
        }

        private void OnAvailableCellEditEnding(string newText) {
            if (!GridValidator.ParseAmountFromString(newText, out decimal newBudgetedAmount)) {
                return;
            }

            decimal difference = SelectedCategory.Available - newBudgetedAmount;
            SelectedCategory.Budgeted -= difference;
        }

        private void OnBudgetedCellEditEnding(string newText) {
            viewModel.SelectedItem.Budgeted = GridValidator.ParseAmountFromString(newText, out decimal newBudgetedAmount) ? newBudgetedAmount : 0;
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e) {
            viewModel.ItemList.Clear();
        }

        private void OnViewLoaded(object sender, RoutedEventArgs e) {
            PopulateCategoriesList();
        }

        #endregion

        #region Methods

        public IView GetView() => view;

        private void BindCommands() {
            viewModel.AddItemCommand = new DelegateCommand(ExecuteAddCategory, CanExecuteAddCategory);
            viewModel.DeleteItemCommand = new DelegateCommand(ExecuteDeleteCategory, CanExecuteDeleteCategory);
            viewModel.CoverOverBudgetCommand = new DelegateCommand(ExecuteCoverOverBudget, CanExecuteCoverOverBudget);
        }

        private void ExecuteCoverOverBudget() {
        }

        private bool CanExecuteCoverOverBudget() => true;

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
            if (dataService.RemoveCategory(SelectedCategory)) {
                viewModel.RemoveItem(SelectedCategory);
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

        private Category SelectedCategory => viewModel.SelectedItem;

        #endregion
    }
}