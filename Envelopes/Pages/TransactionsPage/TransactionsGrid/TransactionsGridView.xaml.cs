using System.Windows.Controls;
using Envelopes.Common;

namespace Envelopes.Pages.TransactionsPage.TransactionsGrid {
    public interface ITransactionsGridView : IView {
        public DataGrid TransactionsGrid { get; }
    }

    /// <summary>
    ///     Interaction logic for TransactionsGridView.xaml
    /// </summary>
    public partial class TransactionsGridView : ITransactionsGridView {
        public TransactionsGridView() {
            InitializeComponent();
        }

        public DataGrid TransactionsGrid => transactionsGrid;
    }
}