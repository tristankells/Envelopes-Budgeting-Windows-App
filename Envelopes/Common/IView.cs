using System.Windows;

namespace Envelopes.Pages.TransactionsPage.AccountsPane {
    public interface IView {
        object DataContext { get; set; }

        public event RoutedEventHandler Loaded;
        public event RoutedEventHandler Unloaded;
    }
}