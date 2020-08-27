using System.Windows;
using Envelopes.Common;
using Envelopes.Pages.BudgetPage.CategoriesGrid;
using Envelopes.Pages.TransactionsPage;

namespace Envelopes.Pages.BudgetPage {
    public interface IBudgetPagePresenter {
        public BudgetPageView GetPageView();
    }

    public class BudgetPagePresenter : Presenter, IBudgetPagePresenter {
        public BudgetPagePresenter(BudgetPageView view, ITransactionsPageViewModel viewModel, ICategoriesGridPresenter categoriesGridPresenter) : base(view, viewModel) {
            this.view = view;
            this.viewModel = viewModel;
            this.categoriesGridPresenter = categoriesGridPresenter;

            BindEvents();
        }

        public BudgetPageView GetPageView() => view;

        private void BindEvents() {
            view.Loaded += View_Loaded;
        }

        private void View_Loaded(object sender, RoutedEventArgs e) {
            view.CategoriesGridControl.Content = categoriesGridPresenter.GetView();
        }

        #region Fields

        private readonly BudgetPageView view;
        private ITransactionsPageViewModel viewModel;
        private readonly ICategoriesGridPresenter categoriesGridPresenter;

        #endregion
    }
}