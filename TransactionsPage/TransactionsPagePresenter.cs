using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using Envelopes.Common;
using Envelopes.TransactionsPage.AccountsPane;

namespace Envelopes.TransactionsPage {
    public interface ITransactionsPagePresenter  {
        public TransactionsPageView GetPageView();
    }

    public class TransactionsPagePresenter : Presenter, ITransactionsPagePresenter {
        #region Fields

        private TransactionsPageView view;
        private ITransactionsPageViewModel viewModel;
        private IAccountsPanePresenter accountsPanePresenter;

        #endregion

        public TransactionsPagePresenter(TransactionsPageView view, ITransactionsPageViewModel viewModel,
            IAccountsPanePresenter accountsPanePresenter) : base(view,
            viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.accountsPanePresenter = accountsPanePresenter;

            BindEvents();
        }

        private void BindEvents() {
            view.Loaded += View_Loaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            view.AccountsPaneControl.Content = accountsPanePresenter.GetView();

        }

        public TransactionsPageView GetPageView() => view;
    }
}