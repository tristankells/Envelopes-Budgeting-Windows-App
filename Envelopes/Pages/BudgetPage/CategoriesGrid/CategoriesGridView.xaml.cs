using System;
using System.Windows.Controls;
using Envelopes.Pages.TransactionsPage.AccountsPane;

namespace Envelopes.Pages.BudgetPage.CategoriesGrid {
    public interface ICategoriesGridView : IView {
        public DataGrid CategoriesDataGrid { get; }
        public event EventHandler<DataGridCellEditEndingEventArgs> DataGridCellEditEnding;
    }

    /// <summary>
    ///     Interaction logic for CategoriesGridView.xaml
    /// </summary>
    public partial class CategoriesGridView : ICategoriesGridView {
        public event EventHandler<DataGridCellEditEndingEventArgs> DataGridCellEditEnding;

        public CategoriesGridView() {
            InitializeComponent();
            CategoriesDataGrid.CellEditEnding += OnCellEditEnding;
        }

        public DataGrid CategoriesDataGrid => categoriesDataGrid;

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            DataGridCellEditEnding?.Invoke(sender, e);
        }
    }
}