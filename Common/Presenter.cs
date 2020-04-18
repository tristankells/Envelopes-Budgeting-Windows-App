using System.Windows;
using System.Windows.Controls;
using Envelopes.TransactionsPage.AccountsPane;

namespace Envelopes.Common {
    /// <summary>
    /// Simple base class for presenters that connects the DataContext of a View object to a IViewModel replacing three lines with a call to base class constructor
    /// </summary>
    public abstract class Presenter {
        private FrameworkElement View { get; }
        private IViewModel ViewModel { get; }

        protected Presenter(FrameworkElement view, IViewModel viewModel) {
            View = view;
            ViewModel = viewModel;
            View.DataContext = ViewModel;
        }
    }
}