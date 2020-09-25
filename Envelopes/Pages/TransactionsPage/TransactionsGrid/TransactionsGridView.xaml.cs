using System;
using System.Windows.Controls;
using Envelopes.Common;

namespace Envelopes.Pages.TransactionsPage.TransactionsGrid {
    public interface ITransactionsGridView : IView {
        public DataGrid TransactionsGrid { get; }
        public event EventHandler<DataGridCellEditEndingEventArgs> DataGridCellEditEnding;
    }

    /// <summary>
    ///     Interaction logic for TransactionsGridView.xaml
    /// </summary>
    public partial class TransactionsGridView : ITransactionsGridView {
        public TransactionsGridView() {
            InitializeComponent();
            TransactionsGrid.CellEditEnding += OnCellEditEnding;
        }

        public event EventHandler<DataGridCellEditEndingEventArgs> DataGridCellEditEnding;
        public DataGrid TransactionsGrid => transactionsGrid;

        private void OnCellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            DataGridCellEditEnding?.Invoke(sender, e);
        }
    }
}