using System.Windows;
using Envelopes.Common;
using Envelopes.Pages.TransactionsPage.AccountsPane;
using Envelopes.TransactionsPage;
using Envelopes.TransactionsPage.TransactionsGrid;

namespace Envelopes.Pages.TransactionsPage {
    public interface ITransactionsPagePresenter  {
        public TransactionsPageView GetPageView();
    }

    public class TransactionsPagePresenter : Presenter, ITransactionsPagePresenter {
        #region Fields

        private readonly TransactionsPageView view;
        private ITransactionsPageViewModelBase viewModelBase;
        private readonly IAccountsPanePresenter accountsPanePresenter;
        private readonly ITransactionsGridPresenter transactionsGridPresenter;

        #endregion

        public TransactionsPagePresenter(TransactionsPageView view, ITransactionsPageViewModelBase viewModelBase,
            IAccountsPanePresenter accountsPanePresenter,
            ITransactionsGridPresenter transactionsGridPresenter) : base(view,
            viewModelBase) {
            this.view = view;
            this.viewModelBase = viewModelBase;
            this.accountsPanePresenter = accountsPanePresenter;
            this.transactionsGridPresenter = transactionsGridPresenter;

            BindEvents();
        }

        private void BindEvents() {
            view.Loaded += View_Loaded;
            view.Unloaded += View_Unloaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            view.AccountsPaneControl.Content = accountsPanePresenter.GetView();
            view.TransactionsGridControl.Content = transactionsGridPresenter.GetView();
        }

        public TransactionsPageView GetPageView() => view;

        private void View_Unloaded(object sender, RoutedEventArgs e)
        {
            view.Loaded -= View_Loaded;
            view.Unloaded -= View_Unloaded;
        }
    }
}