using System.Windows.Controls;
using Envelopes.Common;

namespace Envelopes.Pages.TransactionsPage.AccountsPane {
    public interface IAccountsPaneView : IView {
        public DataGrid AccountsDataGrid { get; }
    }

    /// <summary>
    ///     Interaction logic for AccountsPaneView.xaml
    /// </summary>
    public partial class AccountsPaneView : IAccountsPaneView {
        public AccountsPaneView() {
            InitializeComponent();
        }

        public DataGrid AccountsDataGrid => accountsDataGrid;
    }
}