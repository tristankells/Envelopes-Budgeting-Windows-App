using System.Windows;
using Envelopes.Common;
using Envelopes.Pages.TransactionsPage.AccountsPane;
using Envelopes.Pages.TransactionsPage.TransactionsGrid;

namespace Envelopes.Pages.TransactionsPage {
    public interface ITransactionsPagePresenter {
        public TransactionsPageView GetPageView();
    }

    public class TransactionsPagePresenter : Presenter, ITransactionsPagePresenter {
        public TransactionsPagePresenter(TransactionsPageView view,
            ITransactionsPageViewModel viewModel,
            IAccountsPanePresenter accountsPanePresenter,
            ITransactionsGridPresenter transactionsGridPresenter) : base(view,
            viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.accountsPanePresenter = accountsPanePresenter;
            this.transactionsGridPresenter = transactionsGridPresenter;

            BindEvents();
        }

        public TransactionsPageView GetPageView() => view;

        private void BindEvents() {
            view.Loaded += View_Loaded;
            view.Unloaded += View_Unloaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            view.AccountsPaneControl.Content = accountsPanePresenter.GetView();
            view.TransactionsGridControl.Content = transactionsGridPresenter.GetView();
        }

        private void View_Unloaded(object sender, RoutedEventArgs e) {
            view.Loaded -= View_Loaded;
            view.Unloaded -= View_Unloaded;
        }

        #region Fields

        private readonly TransactionsPageView view;
        private ITransactionsPageViewModel viewModel;
        private readonly IAccountsPanePresenter accountsPanePresenter;
        private readonly ITransactionsGridPresenter transactionsGridPresenter;

        #endregion
    }
}