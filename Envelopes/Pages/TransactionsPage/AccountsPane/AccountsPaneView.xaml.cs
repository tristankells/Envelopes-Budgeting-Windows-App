using System.Windows;
using System.Windows.Controls;

namespace Envelopes.Pages.TransactionsPage.AccountsPane
{
    public interface IView {
        object DataContext { get; set; }
    }

    public interface IAccountsPaneView : IView {
        public event RoutedEventHandler Loaded;
        public DataGrid AccountsDataGrid { get; }
        public event RoutedEventHandler Unloaded;
    }

    /// <summary>
    /// Interaction logic for AccountsPaneView.xaml
    /// </summary>
    public partial class AccountsPaneView : IAccountsPaneView {
        public AccountsPaneView() {
            InitializeComponent();

        }

        public DataGrid AccountsDataGrid => accountsDataGrid;
    }
}
