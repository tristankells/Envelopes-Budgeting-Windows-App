using System.Windows;
using System.Windows.Controls;
using Envelopes.CategoriesPage.CategoriesGrid;
using Envelopes.Common;
using Envelopes.Data;
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

        #endregion

        #region Constructors

        public CategoriesGridPresenter(CategoriesGridView view, ICategoriesGridViewModelBase viewModel,
            IDataService dataService) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.dataService = dataService;

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
            throw new System.NotImplementedException();
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
            var categories = dataService.GetCategories();
            foreach (var category in categories)
            {
                viewModel.AddItem(category);
            }
        }

        #endregion
    }
}