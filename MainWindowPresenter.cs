using System.Windows;
using Envelopes.Common;
using Envelopes.Data;
using Envelopes.TransactionsPage;

namespace Envelopes {
    public interface IMainWindowPresenter {
        public Window MainWindow { get; }
    }

    public class MainWindowPresenter : Presenter, IMainWindowPresenter {
        private readonly ITransactionsPagePresenter transactionsPagePresenter;
        private readonly MainWindow view;
        private readonly IMainWindowViewModel viewModel;

        public MainWindowPresenter(MainWindow view, IMainWindowViewModel viewModel,
            ITransactionsPagePresenter transactionsPagePresenter) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.transactionsPagePresenter = transactionsPagePresenter;

            BindEvents();
        }

        private void BindEvents() {
            view.Loaded += View_Loaded;
            view.Closed += View_Closed;
        }

        private async void View_Closed(object sender, System.EventArgs e) {
           await DataService.Instance.SaveBudget();
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            viewModel.CurrentPage = transactionsPagePresenter.GetPageView();
        }

        public Window MainWindow => view;
    }
}