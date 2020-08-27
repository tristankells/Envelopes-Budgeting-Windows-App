using System;
using System.Windows.Controls;
using Envelopes.Common;

namespace Envelopes.Pages.BudgetPage.CategoriesGrid {
    public interface ICategoriesGridView : IView {
        public DataGrid CategoriesDataGrid { get; }
        public event EventHandler<DataGridCellEditEndingEventArgs> DataGridCellEditEnding;
    }

    /// <summary>
    ///     Interaction logic for CategoriesGridView.xaml
    /// </summary>
    public partial class CategoriesGridView : ICategoriesGridView {
        public CategoriesGridView() {
            InitializeComponent();
            CategoriesDataGrid.CellEditEnding += OnCellEditEnding;
        }

        public event EventHandler<DataGridCellEditEndingEventArgs> DataGridCellEditEnding;

        public DataGrid CategoriesDataGrid => categoriesDataGrid;

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            DataGridCellEditEnding?.Invoke(sender, e);
        }
    }
}